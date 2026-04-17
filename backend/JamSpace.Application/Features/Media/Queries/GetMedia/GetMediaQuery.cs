using JamSpace.Application.Common.Models;
using MediatR;

namespace JamSpace.Application.Features.Media.Queries.GetMedia;

public sealed record GetMediaQuery(string Url) : IRequest<StoredFileDownload?>;
