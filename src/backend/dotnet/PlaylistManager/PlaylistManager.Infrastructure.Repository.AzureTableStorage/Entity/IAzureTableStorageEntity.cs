using Azure.Data.Tables;

namespace PlaylistManager.Infrastructure.Repository.AzureTableStorage.Entity;

public interface IAzureTableStorageEntity : ITableEntity
{
	public string Id { get; init; }
	public DateTime CreatedAtUtc { get; init; }
	public DateTime UpdatedAtUtc { get; init; }
}