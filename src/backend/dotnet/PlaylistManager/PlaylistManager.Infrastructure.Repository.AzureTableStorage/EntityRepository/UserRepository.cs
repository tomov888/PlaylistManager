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

	public async Task<User> AddUserAsync(User user)
	{
		var userEntity = (UserEntity)user;

		await InsertAsync(userEntity);

		return DomainModel(userEntity);
	}
}