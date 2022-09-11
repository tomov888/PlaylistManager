using System.Linq.Expressions;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Infrastructure.Repository.AzureTableStorage.Attributes;
using PlaylistManager.Infrastructure.Repository.AzureTableStorage.Entity;

namespace PlaylistManager.Infrastructure.Repository.AzureTableStorage;

public class AzureTableStorageNativeRepository<T> : IAzureTableStorageNativeRepository<T> where T : class, IAzureTableStorageEntity, new()
	{
		protected readonly ILogger _logger;
		protected readonly TableServiceClient _tableServiceClient;
		protected TableClient _tableClient;

		public AzureTableStorageNativeRepository(ILogger logger, TableServiceClient tableServiceClient)
		{
			_logger = logger;
			_tableServiceClient = tableServiceClient;
			_tableClient = _tableServiceClient.GetTableClient(GetTableName(typeof(T)));
		}
		
		private string GetTableName(Type documentType)
		{
			var customAttributesOfType = documentType.GetCustomAttributes(typeof(TableNameAttribute), true);
			return ((TableNameAttribute) customAttributesOfType.FirstOrDefault())?.TableName;
		}

		public async Task<Option<T>> ReadOneAsync(string partitionKey, string rowKey)
		{
			if (string.IsNullOrWhiteSpace(partitionKey)) throw new ArgumentException(nameof(partitionKey));
			if (string.IsNullOrWhiteSpace(rowKey)) throw new ArgumentException(nameof(rowKey));

			try
			{
				var result = await _tableClient.GetEntityAsync<T>(partitionKey, rowKey);
				return Option<T>.Of(result.Value);
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex, $"[{nameof(AzureTableStorageNativeRepository<T>)}] => Exception happened while fetching entity with PartitionKey: {partitionKey} and RowKey: {rowKey}");

				return Option<T>.None;
			}
		}

		public async Task InsertAsync(T entity)
		{
			ArgumentNullException.ThrowIfNull(entity, nameof(entity));

			try
			{
				await _tableClient.AddEntityAsync(entity);
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex, $"[{nameof(AzureTableStorageNativeRepository<T>)}] => Exception happened while inserting entity with PartitionKey: {entity.PartitionKey} and RowKey: {entity.RowKey}");
				throw;
			}
			
		}

		public async Task UpsertAsync(T entity)
		{
			ArgumentNullException.ThrowIfNull(entity, nameof(entity));

			await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace);
		}

		// https://docs.microsoft.com/en-us/azure/storage/tables/table-storage-design-for-query => Design for querying [duplication on many rowkey values is good]
		// Three types of querying :
		// 1) Point read | partition key + row key => fastest, returns single item
		// 2) Range query on row key where you provide partition key also => Second fastest, partition is known and rowkey is indexed
		// 3) Range query on non-rowkey field  with partition key provided => slowest cause it needs to do partition scan

		public async Task<List<T>> QueryAsync(Expression<Func<T, bool>> filter) 
		{
			var items = new List<T>();
			var results = _tableClient.QueryAsync<T>(filter).AsPages();

			await foreach (var item in results) 
			{
				items.AddRange(item.Values);
			}

			return items;
		}

		public async Task DeleteAsync(string partitionKey, string rowKey) 
		{
			await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
		}
	}