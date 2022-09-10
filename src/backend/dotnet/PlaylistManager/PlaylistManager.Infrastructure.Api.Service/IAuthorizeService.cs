using Microsoft.AspNetCore.Http;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Infrastructure.Api.Service;

public interface IAuthorizeService
{
	Task<OperationResult<UserSession>> AuthorizeAsync(HttpRequest httpRequest, List<Permission> permissions);
}