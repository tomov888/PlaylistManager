namespace PlaylistManager.Core.Contracts.Models.UseCases.YoutubeTrackDownload;

public record YoutubeVideoInfo(string Title, int LengthInSeconds, long SizeInBytes, string Url, string DownloadLocationPath);
