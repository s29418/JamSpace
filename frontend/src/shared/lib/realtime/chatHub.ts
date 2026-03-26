import {
    HubConnection,
    HubConnectionBuilder,
    HubConnectionState,
    LogLevel,
    HttpTransportType,
} from "@microsoft/signalr";
import { getToken } from "shared/lib/auth/auth";
import { waitForAuthReady } from "shared/lib/auth/waitForAuthReady";
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
            .withUrl("http://localhost:5072/hubs/chat", {
                accessTokenFactory: () => getToken() ?? "",
                transport: HttpTransportType.WebSockets
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        this.connection.onreconnected(async () => {
            if (!this.activeConversationId) return;

            try {
                await this.joinConversation(this.activeConversationId);
            } catch (error) {
                console.error("Failed to rejoin conversation after reconnect", error);
            }
        });

        return this.connection;
    }

    async start() {
        await waitForAuthReady();

        const token = getToken();
        if (!token) return;

        const connection = this.ensureConnection();

        if (connection.state === HubConnectionState.Connected) return;
        if (this.startPromise) return this.startPromise;

        this.startPromise = connection
            .start()
            .catch((error) => {
                const message =
                    error instanceof Error ? error.message : String(error);

                if (message.includes("stopped during negotiation")) {
                    return;
                }

                console.error("Failed to start chat hub", error);
                throw error;
            })
            .finally(() => {
                this.startPromise = null;
            });

        return this.startPromise;
    }

    async stop() {
        const connection = this.connection;

        this.activeConversationId = null;
        this.startPromise = null;
        this.connection = null;

        if (!connection) return;

        if (connection.state !== HubConnectionState.Disconnected) {
            try {
                await connection.stop();
            } catch (error) {
                console.error("Failed to stop chat hub", error);
            }
        }
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

        return () => connection.off("message:new", handler);
    }

    onConversationUpdated(handler: (payload: ConversationUpdatedEvent) => void) {
        const connection = this.ensureConnection();

        connection.on("conversation:updated", (payload) => {
            console.log("conversation:updated EVENT", payload);
            handler(payload);
        });

        return () => {
            connection.off("conversation:updated", handler);
        };
    }

    onConversationRead(handler: (payload: ConversationReadEvent) => void): Unsubscribe {
        const connection = this.ensureConnection();
        connection.on("conversation:read", handler);

        return () => connection.off("conversation:read", handler);
    }

    onConversationTyping(handler: (payload: ConversationTypingEvent) => void): Unsubscribe {
        const connection = this.ensureConnection();
        connection.on("conversation:typing", handler);

        return () => connection.off("conversation:typing", handler);
    }
}

export const chatHub = new ChatHubClient();