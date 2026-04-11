using JamSpace.Application.Features.Posts.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Posts.Queries.GetPostById;

public sealed record GetPostByIdQuery(Guid Id) : IRequest<PostDto>;
