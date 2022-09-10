using System.Linq.Expressions;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Infrastructure.Repository.AzureTableStorage.Entity;

namespace PlaylistManager.Infrastructure.Repository.AzureTableStorage;

public interface IAzureTableStorageRepository<TModel>
{
	public Task<Option<TModel>> FindOneAsync(string partitionKey, string rowKey);
	public Task InsertAsync(TModel model);
	public Task<List<TModel>> QueryAsync(Expression<Func<TModel, bool>> filter);
	Task DeleteAsync(string partitionKey, string rowKey);
}