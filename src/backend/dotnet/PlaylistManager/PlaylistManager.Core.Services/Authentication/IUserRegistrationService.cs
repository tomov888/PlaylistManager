using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Services.Authentication;

public interface IUserRegistrationService
{
	Task<User> RegisterUserAsync(string email, string userName, string password, DateTime dateOfBirth, string photoUrl);
}