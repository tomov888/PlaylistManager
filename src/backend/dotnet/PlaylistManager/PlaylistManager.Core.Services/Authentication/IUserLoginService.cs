using PlaylistManager.Core.Contracts.Models.Authentication;

namespace PlaylistManager.Core.Services.Authentication;

public interface IUserLoginService
{
	Task<UserLoginResult> TryLoginAsync(string email, string password);
}