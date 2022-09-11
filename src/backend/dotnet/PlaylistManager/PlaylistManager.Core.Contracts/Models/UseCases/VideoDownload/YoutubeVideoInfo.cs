namespace PlaylistManager.Core.Contracts.Models.UseCases.VideoDownload;

public record YoutubeVideoInfo(string Title, int LengthInSeconds, long SizeInBytes, string Url);
