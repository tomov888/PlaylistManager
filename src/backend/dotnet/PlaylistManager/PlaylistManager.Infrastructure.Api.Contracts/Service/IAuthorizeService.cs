using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace PlaylistManager.Infrastructure.Api.Contracts.Service;

public interface IAuthorizeService
{
	Task<OperationResult<UserSession>> AuthorizeAsync(HttpRequest httpRequest, List<Permission> permissions);
}