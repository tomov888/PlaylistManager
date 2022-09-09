using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Common.Utils;
using PlaylistManager.Core.Contracts.Models.Authentication;
using PlaylistManager.Core.Contracts.Repository;
using PlaylistManager.Core.Services.Authentication.JsonWebToken;

namespace PlaylistManager.Core.Services.Authentication;

public class UserLoginService : IUserLoginService
{
	private readonly ILogger<UserLoginService> _logger;
	private readonly IJwtService _jwtService;
	private readonly IUserRepository _userRepository;

	public UserLoginService(ILogger<UserLoginService> logger, IJwtService jwtService, IUserRepository userRepository)
	{
		_logger = logger;
		_jwtService = jwtService;
		_userRepository = userRepository;
	}

	public async Task<UserLoginResult> TryLoginAsync(string email, string password)
	{
		var userOption = await _userRepository.FindByEmailAsync(email);

		var isPasswordValid = CryptographyProcessor.AreEqual(password, userOption.Value.PasswordHash, userOption.Value.PasswordSalt);
			
		if (!isPasswordValid) throw new Exception("Invalid Password !!!");

		var authenticationTokenPair = await _jwtService.GenerateTokenPairAsync(userOption.Value, new List<string>{"ADD", "REMOVE"});

		var loginResult = new UserLoginResult 
		{ 
			Token = authenticationTokenPair.Token, 
			RefreshToken = authenticationTokenPair.RefreshToken 
		};

		return loginResult;
	}	
}