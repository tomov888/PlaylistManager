namespace PlaylistManager.Core.Contracts.Models.UseCases.YoutubeTrackDownload;

public record VideoToMp3ConversionResult
{
	public string Name { get; init; }
	public long SizeInBytes { get; init; }
}