namespace PlaylistManager.Core.Domain.Enums;

public enum Permission
{
	NONE = 0,

	VIEW_SONGS = 101,
	ADD_SONGS = 102,
	DELETE_SONGS = 103,
	UPDATE_SONGS = 104,
	
	VIEW_PLAYLISTS = 201,
	ADD_PLAYLISTS = 202,
	DELETE_PLAYLISTS = 203,
	UPDATE_PLAYLISTS = 204,
	
}