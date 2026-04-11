using JamSpace.Application.Common.Models;
using JamSpace.Application.Features.Posts.DTOs;
using MediatR;

namespace JamSpace.Application.Features.Posts.Commands.CreatePost;

public sealed record CreatePostCommand(Guid AuthorId, string? Content, FileUpload? File) : IRequest<PostDto>;