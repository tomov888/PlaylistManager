using PlaylistManager.Core.Common.Utils;
using PlaylistManager.Core.Contracts.Repository;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Services.Authentication;

	public class UserRegistrationService : IUserRegistrationService
	{
		private readonly IUserRepository _userRepository;
		
		public UserRegistrationService(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task<User> RegisterUserAsync(string email, string userName, string password, DateTime dateOfBirth, string photoUrl)
		{
			var userOption = await _userRepository.FindByEmailAsync(email);
			if (userOption.HasValue) throw new Exception($"User with email {email} already exists");

			var passwordSalt = CryptographyProcessor.CreateSalt(email.Length);
			var passwordHash = CryptographyProcessor.GenerateHash(password, passwordSalt);

			var user = await _userRepository.TryRegisterUserAsync(email, userName, passwordHash, passwordSalt, dateOfBirth, photoUrl, AuthProvider.SELF);
			
			return user;
		}

		// public async Task<RegisterUserResult> RegisterViaGoogleAsync(string firstName, string lastName, string email, string photoUrl)
		// {
		// 	var userWithEmailAlreadyExists = await _userRepository.UserWithEmailExistsAsync(email);
		// 	if (userWithEmailAlreadyExists) return new RegisterUserResult { Errors = new[] { $"User with {email} email already exists!" } };
		// 	
		// 	var password = CryptographyProcessor.PasswordGenerator(16, true);
		// 	var passwordSalt = CryptographyProcessor.CreateSalt(email.Length);
		// 	var passwordHash = CryptographyProcessor.GenerateHash(password, passwordSalt);
		//
		// 	var user = new User
		// 	{
		// 		FirstName = firstName,
		// 		LastName = lastName,
		// 		Email = email,
		// 		Username = email,
		// 		PasswordSalt = passwordSalt,
		// 		PasswordHash = passwordHash,
		// 		IsDeleted = false,
		// 		RegisteredAtUtc = DateTime.UtcNow,
		// 		Activated = false,
		// 		PhotoUrl = photoUrl,
		// 		Role = RoleType.FREE_USER,
		// 		AuthProvider = AuthProvider.GOOGLE,
		// 	};
		//
		// 	var newUser = await _userRepository.AddAsync(user);
		//
		// 	return new RegisterUserResult { Success = true, User = newUser};
		// }		

	}