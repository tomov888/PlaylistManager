using System.Linq.Expressions;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Common.Utils;
using PlaylistManager.Infrastructure.Repository.AzureTableStorage.Entity;

namespace PlaylistManager.Infrastructure.Repository.AzureTableStorage;

public class AzureTableStorageRepository<TEntity, TModel> : 
	AzureTableStorageNativeRepository<TEntity>, 
	IAzureTableStorageNativeRepository<TEntity>,
	IAzureTableStorageRepository<TModel> 
	where TEntity: class, IAzureTableStorageEntity, new()
{
	public AzureTableStorageRepository(ILogger logger, TableServiceClient tableServiceClient) : base(logger, tableServiceClient)
	{
		
	}

	protected virtual TModel DomainModel (TEntity entity) => default;
	protected virtual TEntity Entity (TModel model) => default;
	
	protected Expression<Func<TEntity, bool>> TranslateQuery(Expression<Func<TModel, bool>> query)
	{
		return ExpressionConverter.TranslateQuery<TModel, TEntity>(query);
	}

	public async Task<Option<TModel>> FindOneAsync(string partitionKey, string rowKey)
	{
		var entityOption = (await ReadOneAsync(partitionKey, rowKey));
		var modelOption = entityOption.Map(x => DomainModel(x));

		return modelOption;
	}
	
	public async Task InsertAsync(TModel model)
	{
		var entity = Entity(model);
		await InsertAsync(entity);
	}
	
	public async Task<List<TModel>> QueryAsync(Expression<Func<TModel, bool>> filter)
	{
		var query = TranslateQuery(filter);
		var entities = await QueryAsync(query);
		return entities.Select(x => DomainModel(x)).ToList();
	}
}