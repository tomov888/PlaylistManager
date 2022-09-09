using Azure;

namespace PlaylistManager.Infrastructure.Repository.AzureTableStorage.Entity;

public record AzureTableStorageEntity: IAzureTableStorageEntity
{
	public string PartitionKey { get; set; }
	public string RowKey { get; set; }
	public DateTimeOffset? Timestamp { get; set; }
	public ETag ETag { get; set; }

	public string Id { get; init; }
	public DateTime CreatedAtUtc { get; init; }
	public DateTime UpdatedAtUtc { get; init; }
}