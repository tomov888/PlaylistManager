using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Contracts.Repository;

public interface IUserRepository
{
	Task<Option<User>> FindByEmailAsync(string email);
	Task<User> AddUserAsync(User user);
}