using Azure;
using PlaylistManager.Core.Domain.Contracts;
using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;
using PlaylistManager.Infrastructure.Repository.AzureTableStorage.Attributes;

namespace PlaylistManager.Infrastructure.Repository.AzureTableStorage.Entity;

[TableName("user")]
public record UserEntity : AzureTableStorageEntity, IUser
{
	
	public string Email { get; init; }
	public string Username { get; init; }
	public string PasswordHash { get; init; }
	public string PasswordSalt { get; init; }
	public DateTime DateOfBirth { get; init; }
	public string PhotoUrl { get; init; }
	public UserRole Role { get; init; }
	public AuthProvider AuthProvider { get; init; }

	public static UserEntity From(IUser model)
	{
		return new UserEntity
		{
			PartitionKey = model.Email,
			RowKey = model.Email,
			Timestamp = DateTimeOffset.UtcNow,
			
			Email = model.Email,
			Id = model.Email,
			Username = model.Username,
			DateOfBirth = model.DateOfBirth,
			PhotoUrl = model.PhotoUrl,
			PasswordHash = model.PasswordHash,
			PasswordSalt = model.PasswordSalt,
			CreatedAtUtc = model.CreatedAtUtc,
			UpdatedAtUtc = model.UpdatedAtUtc,
			Role = model.Role,
			AuthProvider = model.AuthProvider
		};
	}

	public static explicit operator UserEntity(User model)
	{
		return new UserEntity
		{
			PartitionKey = model.Email,
			RowKey = model.Email,
			Timestamp = DateTimeOffset.UtcNow,
			
			Email = model.Email,
			Id = model.Email,
			Username = model.Username,
			DateOfBirth = model.DateOfBirth,
			PhotoUrl = model.PhotoUrl,
			PasswordHash = model.PasswordHash,
			PasswordSalt = model.PasswordSalt,
			CreatedAtUtc = model.CreatedAtUtc,
			UpdatedAtUtc = model.UpdatedAtUtc,
			Role = model.Role,
			AuthProvider = model.AuthProvider
		};
	}
	
	public static explicit operator User(UserEntity model)
	{
		return new User
		{
			Email = model.Email,
			Id = model.Email,
			Username = model.Username,
			DateOfBirth = model.DateOfBirth,
			PhotoUrl = model.PhotoUrl,
			PasswordHash = model.PasswordHash,
			PasswordSalt = model.PasswordSalt,
			CreatedAtUtc = model.CreatedAtUtc,
			UpdatedAtUtc = model.UpdatedAtUtc,
			Role = model.Role,
			AuthProvider = model.AuthProvider
		};
	}
	
}