import { api } from "shared/api/base";
import type {
    ConversationListItemDto,
    CursorResult,
    MessageDto,
    GetMessagesParams,
    CreateDirectConversationRequest,
    CreateDirectConversationResponse,
    ConversationDetailsDto,
} from "../model/types";

const ROOT = "/conversations";

export const getConversations = async (): Promise<ConversationListItemDto[]> => {
    const res = await api.get<ConversationListItemDto[]>(ROOT);
    return res.data ?? [];
};

export const getConversationDetails = async (
    conversationId: string
): Promise<ConversationDetailsDto> => {
    const res = await api.get<ConversationDetailsDto>(`${ROOT}/${conversationId}`);
    return res.data;
};

export const getConversationMessages = async ({
                                                  conversationId,
                                                  before,
                                                  take = 50,
                                              }: GetMessagesParams): Promise<CursorResult<MessageDto>> => {
    const searchParams = new URLSearchParams();

    if (before) searchParams.set("before", before);
    searchParams.set("take", String(take));

    const res = await api.get<CursorResult<MessageDto>>(
        `${ROOT}/${conversationId}/messages?${searchParams.toString()}`
    );

    return {
        items: res.data?.items ?? [],
        hasMore: !!res.data?.hasMore,
        nextBefore: res.data?.nextBefore ?? null,
    };
};

export const markConversationAsRead = async (conversationId: string): Promise<void> => {
    await api.post(`${ROOT}/${conversationId}/read`);
};

export const getOrCreateDirectConversation = async (
    payload: CreateDirectConversationRequest
): Promise<CreateDirectConversationResponse> => {
    const res = await api.post<CreateDirectConversationResponse>(`${ROOT}/direct`, payload);
    return res.data;
};