using System.Linq.Expressions;
using PlaylistManager.Core.Common.Models;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Contracts.Repository;

public interface ITrackRepository
{
	Task InsertAsync(Track model);
	Task<List<Track>> QueryAsync(Expression<Func<Track, bool>> filter);
	Task DeleteAsync(string partitionKey, string rowKey);
	Task<Option<Track>> FindOneAsync(string partitionKey, string rowKey);
	Task<List<Track>> GetTracksAsync(string userEmail);
}