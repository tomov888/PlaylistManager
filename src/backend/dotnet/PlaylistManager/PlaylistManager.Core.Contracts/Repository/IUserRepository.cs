using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Contracts.Repository;

public interface IUserRepository
{
	Task<Option<User>> FindByEmailAsync(string email);
	Task<User> TryRegisterUserAsync(string email, string username, string passwordHash, string passwordSalt, DateTime dateOfBirth, string photoUrl, AuthProvider authProvider);
}