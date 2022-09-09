using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Contracts.Repository;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;
using PlaylistManager.Infrastructure.Repository.AzureTableStorage.Entity;

namespace PlaylistManager.Infrastructure.Repository.AzureTableStorage.EntityRepository;

public class UserRepository: AzureTableStorageRepository<UserEntity, User>, IUserRepository
{
	public UserRepository(ILogger<UserRepository> logger, TableServiceClient tableServiceClient) : base(logger, tableServiceClient)
	{
	}

	protected override User DomainModel (UserEntity entity) => (User)entity;
	protected override UserEntity Entity (User model) => (UserEntity)model;
	
	public async Task<Option<User>> FindByEmailAsync(string email)
	{
		var user= await FindOneAsync(email, email);
		return user;
	}

	public async Task<User> TryRegisterUserAsync(string email, string username, string passwordHash, string passwordSalt, DateTime dateOfBirth, string photoUrl, AuthProvider authProvider)
	{
		var userOption = await FindOneAsync(email, email);

		if (userOption.HasValue) throw new Exception($"User with email {email} already exists");

		var userEntity = new UserEntity
		{
			Email = email,
			Id = email,
			Timestamp = DateTimeOffset.UtcNow,
			Username = username,
			PartitionKey = email,
			RowKey = email,
			PasswordHash = passwordHash,
			PasswordSalt = passwordSalt,
			PhotoUrl = photoUrl,
			CreatedAtUtc = DateTime.UtcNow,
			DateOfBirth = dateOfBirth,
			UpdatedAtUtc = DateTime.UtcNow,
			Role = UserRole.User,
			AuthProvider = authProvider
		};

		await InsertAsync(userEntity);

		return DomainModel(userEntity);
	}
}