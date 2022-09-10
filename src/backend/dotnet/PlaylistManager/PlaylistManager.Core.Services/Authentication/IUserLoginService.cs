using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Contracts.Models.Authentication;

namespace PlaylistManager.Core.Services.Authentication;

public interface IUserLoginService
{
	Task<OperationResult<UserLogin>> LoginAsync(string email, string password);
	Task<UserLogin> TryLoginAsync(string email, string password);
}