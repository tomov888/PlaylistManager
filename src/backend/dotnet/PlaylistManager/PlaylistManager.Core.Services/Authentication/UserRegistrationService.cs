using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Common.Utils;
using PlaylistManager.Core.Contracts.Models.UseCases;
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

		public async Task<OperationResult<User>> RegisterUserAsync(UserRegistrationInfo userRegistrationInfo)
		{
			var userOption = await _userRepository.FindByEmailAsync(userRegistrationInfo.Email);
			
			if (userOption.HasValue) 
				return OperationResult<User>.Failure(new Exception($"User with email {userRegistrationInfo.Email} already exists"));

			var passwordSalt = CryptographyProcessor.CreateSalt(userRegistrationInfo.Email.Length);
			var passwordHash = CryptographyProcessor.GenerateHash(userRegistrationInfo.Password, passwordSalt);

			var userToCreate = new User
			{
				Email = userRegistrationInfo.Email,
				Username = userRegistrationInfo.Username,
				DateOfBirth = userRegistrationInfo.DateOfBirth,
				PhotoUrl = userRegistrationInfo.PhotoUrl,
				Id = userRegistrationInfo.Email,
				Role = userRegistrationInfo.Role,
				AuthProvider = userRegistrationInfo.AuthProvider,
				PasswordHash = passwordHash,
				PasswordSalt = passwordSalt,
				CreatedAtUtc = DateTime.UtcNow,
				UpdatedAtUtc = DateTime.UtcNow
			};

			var user = await _userRepository.AddUserAsync(userToCreate);
			
			return OperationResult<User>.Success(user);
		}
		
		public async Task<User> TryRegisterUserAsync(UserRegistrationInfo userRegistrationInfo)
		{
			var userOption = await _userRepository.FindByEmailAsync(userRegistrationInfo.Email);
			
			if (userOption.HasValue) throw new Exception($"User with email {userRegistrationInfo.Email} already exists");

			var passwordSalt = CryptographyProcessor.CreateSalt(userRegistrationInfo.Email.Length);
			var passwordHash = CryptographyProcessor.GenerateHash(userRegistrationInfo.Password, passwordSalt);

			var userToCreate = new User
			{
				Email = userRegistrationInfo.Email,
				Username = userRegistrationInfo.Username,
				DateOfBirth = userRegistrationInfo.DateOfBirth,
				PhotoUrl = userRegistrationInfo.PhotoUrl,
				Id = userRegistrationInfo.Email,
				Role = userRegistrationInfo.Role,
				AuthProvider = userRegistrationInfo.AuthProvider,
				PasswordHash = passwordHash,
				PasswordSalt = passwordSalt,
				CreatedAtUtc = DateTime.UtcNow,
				UpdatedAtUtc = DateTime.UtcNow
			};

			var user = await _userRepository.AddUserAsync(userToCreate);
			
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