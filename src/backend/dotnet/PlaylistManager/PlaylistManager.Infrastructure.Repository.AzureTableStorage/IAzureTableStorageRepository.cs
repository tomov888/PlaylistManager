using System.Linq.Expressions;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Infrastructure.Repository.AzureTableStorage.Entity;

namespace PlaylistManager.Infrastructure.Repository.AzureTableStorage;

public interface IAzureTableStorageRepository<TModel>
{
	Task<Option<TModel>> FindOneAsync(string partitionKey, string rowKey);
	Task InsertAsync(TModel model);
	Task<List<TModel>> QueryAsync(Expression<Func<TModel, bool>> filter);
	Task DeleteAsync(string partitionKey, string rowKey);
}