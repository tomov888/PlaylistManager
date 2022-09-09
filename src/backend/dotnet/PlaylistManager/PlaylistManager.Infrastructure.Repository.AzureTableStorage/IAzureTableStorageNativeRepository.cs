using System.Linq.Expressions;
using Azure.Data.Tables;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Infrastructure.Repository.AzureTableStorage.Entity;

namespace PlaylistManager.Infrastructure.Repository.AzureTableStorage;

public interface IAzureTableStorageNativeRepository<T> where T : class, IAzureTableStorageEntity, new()
{
	Task<Option<T>> ReadOneAsync(string partitionKey, string rowKey);
	public Task InsertAsync(T entity);
	Task UpsertAsync(T entity);
	Task<List<T>> QueryAsync(Expression<Func<T, bool>> filter);
	public Task DeleteAsync(string partitionKey, string rowKey);
}