using JamSpace.Domain.Entities;

namespace JamSpace.Application.Common.Interfaces;

public interface IMessageRepository
{
    Task AddAsync(Message message, CancellationToken ct);
}