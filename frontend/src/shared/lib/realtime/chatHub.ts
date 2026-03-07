import {
    HubConnection,
    HubConnectionBuilder,
    HubConnectionState,
    LogLevel,
} from "@microsoft/signalr";
import { getToken } from "shared/lib/utils/auth";
import type {
    ConversationReadEvent,
    ConversationTypingEvent,
    ConversationUpdatedEvent,
    MarkReadPayload,
    MessageDto,
    SendMessagePayload,
} from "entities/chat/model/types";

type Unsubscribe = () => void;

class ChatHubClient {
    private connection: HubConnection | null = null;
    private activeConversationId: string | null = null;
    private startPromise: Promise<void> | null = null;

    private ensureConnection() {
        if (this.connection) return this.connection;

        this.connection = new HubConnectionBuilder()
            .withUrl("/hubs/chat", {
                accessTokenFactory: () => getToken() ?? "",
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        this.connection.onreconnected(async () => {
            if (this.activeConversationId) {
                try {
                    await this.joinConversation(this.activeConversationId);
                } catch (error) {
                    console.error("Failed to rejoin conversation after reconnect", error);
                }
            }
        });

        return this.connection;
    }

    async start() {
        const connection = this.ensureConnection();

        if (connection.state === HubConnectionState.Connected) return;
        if (this.startPromise) return this.startPromise;

        this.startPromise = connection
            .start()
            .catch((error) => {
                console.error("Failed to start chat hub connection", error);
                throw error;
            })
            .finally(() => {
                this.startPromise = null;
            });

        return this.startPromise;
    }

    async stop() {
        if (!this.connection) return;
        if (this.connection.state === HubConnectionState.Disconnected) return;

        await this.connection.stop();
    }

    async joinConversation(conversationId: string) {
        await this.start();
        await this.connection!.invoke("JoinConversation", conversationId);
        this.activeConversationId = conversationId;
    }

    async leaveConversation(conversationId: string) {
        if (!this.connection || this.connection.state !== HubConnectionState.Connected) return;

        await this.connection.invoke("LeaveConversation", conversationId);

        if (this.activeConversationId === conversationId) {
            this.activeConversationId = null;
        }
    }

    async sendMessage(payload: SendMessagePayload) {
        await this.start();
        await this.connection!.invoke("SendMessage", payload);
    }

    async markRead(payload: MarkReadPayload) {
        await this.start();
        await this.connection!.invoke("MarkRead", payload);
    }

    async sendTyping(conversationId: string, isTyping: boolean) {
        await this.start();
        await this.connection!.invoke("Typing", conversationId, isTyping);
    }

    onMessageNew(handler: (payload: MessageDto) => void): Unsubscribe {
        const connection = this.ensureConnection();
        connection.on("message:new", handler);

        return () => {
            connection.off("message:new", handler);
        };
    }

    onConversationUpdated(handler: (payload: ConversationUpdatedEvent) => void): Unsubscribe {
        const connection = this.ensureConnection();
        connection.on("conversation:updated", handler);

        return () => {
            connection.off("conversation:updated", handler);
        };
    }

    onConversationRead(handler: (payload: ConversationReadEvent) => void): Unsubscribe {
        const connection = this.ensureConnection();
        connection.on("conversation:read", handler);

        return () => {
            connection.off("conversation:read", handler);
        };
    }

    onConversationTyping(handler: (payload: ConversationTypingEvent) => void): Unsubscribe {
        const connection = this.ensureConnection();
        connection.on("conversation:typing", handler);

        return () => {
            connection.off("conversation:typing", handler);
        };
    }
}

export const chatHub = new ChatHubClient();