using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Common.Utils;
using PlaylistManager.Core.Contracts.Models.Authentication;
using PlaylistManager.Core.Contracts.Repository;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Services.Authentication.JsonWebToken;

namespace PlaylistManager.Core.Services.Authentication;

public class UserLoginService : IUserLoginService
{
	private readonly IJwtService _jwtService;
	private readonly ILogger<UserLoginService> _logger;
	private readonly ITokenHandler _tokenHandler;
	private readonly IUserRepository _userRepository;

	public UserLoginService(ILogger<UserLoginService> logger, IJwtService jwtService, IUserRepository userRepository,
		ITokenHandler tokenHandler)
	{
		_logger = logger;
		_jwtService = jwtService;
		_userRepository = userRepository;
		_tokenHandler = tokenHandler;
	}

	public async Task<OperationResult<UserLogin>> LoginAsync(string email, string password)
	{
		var userOption = await _userRepository.FindByEmailAsync(email);

		if (userOption.Empty)
			return OperationResult<UserLogin>.Failure(new Exception($"User with email: {email} does not exist"));

		var isPasswordValid =
			CryptographyProcessor.AreEqual(password, userOption.Value.PasswordHash, userOption.Value.PasswordSalt);

		if (!isPasswordValid) return OperationResult<UserLogin>.Failure(new Exception("Invalid password !"));

		var authenticationTokenPair =
			await _jwtService.GenerateTokenPairAsync(userOption.Value, new List<string> { "ADD", "REMOVE" });

		var loginResult = new UserLogin
		{
			Token = authenticationTokenPair.Token,
			RefreshToken = authenticationTokenPair.RefreshToken
		};

		return OperationResult<UserLogin>.Success(loginResult);
	}

	public async Task<UserLogin> TryLoginAsync(string email, string password)
	{
		var userOption = await _userRepository.FindByEmailAsync(email);

		if (userOption.Empty) throw new Exception($"User with email: {email} does not exist");

		var isPasswordValid = CryptographyProcessor.AreEqual(password, userOption.Value.PasswordHash, userOption.Value.PasswordSalt);

		if (!isPasswordValid) throw new Exception("Invalid password !");

		var authenticationTokenPair = await _jwtService.GenerateTokenPairAsync(userOption.Value, new List<string> { "ADD", "REMOVE" });
		

		var loginResult = new UserLogin
		{
			Token = authenticationTokenPair.Token,
			RefreshToken = authenticationTokenPair.RefreshToken
		};

		var userWithRefreshToken = userOption.Value with { RefreshToken = loginResult.RefreshToken };
		await _userRepository.UpsertUserAsync(userWithRefreshToken);

		return loginResult;
	}

	public async Task<UserLogin> TryRefreshTokenAsync(string token, string refreshToken)
	{
		// check if token is valid (except expiration date, that should be invalid) and extract claims principal
		// we are deliberately ignoring lifetime check since token should be expired on refresh call
		var validatedToken = _tokenHandler.ValidateTokenOnRefreshToken(token);
		if (validatedToken == null) throw new Exception("Invalid Token !!!");

		// extract data from claims
		var expiryDateUnix = long.Parse(validatedToken.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Exp).Value);
		var userId = validatedToken.FindFirst(claim => claim.Type == AuthorizationClaimTypes.UserId).Value;
		var userEmail = validatedToken.FindFirst(claim => claim.Type == AuthorizationClaimTypes.Email).Value;
		var jsonWebTokenId = validatedToken.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

		var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix); // have to do this because expiration date in token is in epoch/unix time
		if (expiryDateTimeUtc > DateTime.UtcNow) throw new Exception("This token has not expired yet !!!");

		var userFromDbOption = await _userRepository.FindByEmailAsync(userEmail);
		if (userFromDbOption.Empty) throw new Exception("User for whom this token was issued does not exist !!!");
		
		if(userFromDbOption.Value.RefreshToken != refreshToken) throw new Exception("Refresh Token does is invalid !!!");

		var authenticationTokenPair = await _jwtService.GenerateTokenPairAsync(userFromDbOption.Value, new List<string> { "ADD", "REMOVE" });

		var refreshTokenResult = new UserLogin
		{
			Token = authenticationTokenPair.Token,
			RefreshToken = authenticationTokenPair.RefreshToken
		};

		return refreshTokenResult;
	}
}