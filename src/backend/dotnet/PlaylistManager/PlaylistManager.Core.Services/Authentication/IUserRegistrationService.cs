using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Contracts.Models.UseCases;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Services.Authentication;

public interface IUserRegistrationService
{
	Task<OperationResult<User>> RegisterUserAsync(UserRegistrationInfo userRegistrationInfo);
	Task<User> TryRegisterUserAsync(UserRegistrationInfo userRegistrationInfo);
}