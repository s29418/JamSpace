namespace JamSpace.Application.Common.Models;

public sealed class CursorResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required bool HasMore { get; init; }
    public DateTimeOffset? NextBefore { get; init; }

    public static CursorResult<T> Create(IReadOnlyList<T> items, bool hasMore, DateTimeOffset? nextBefore)
    {
        return new CursorResult<T>
        {
            Items = items,
            HasMore = hasMore,
            NextBefore = nextBefore
        };
    }
}