using PlaylistManager.Core.Contracts.Models.Authentication;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Services.Authentication.JsonWebToken;

public interface IJwtService
{
	Task<AuthenticationTokenPair> GenerateTokenPairAsync(User user, List<string> permissions);
}