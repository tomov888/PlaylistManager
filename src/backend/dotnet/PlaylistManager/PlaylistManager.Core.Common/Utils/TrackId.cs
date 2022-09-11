namespace PlaylistManager.Core.Common.Utils;

public static class TrackId
{
	public static string New()
	{
		var id = Nanoid.Nanoid.Generate();
		return id;
	}
	
	public static string New(string title)
	{
		var id = Nanoid.Nanoid.Generate();
		var slug = Slug.Slugify(title);
		return $"{id}_{slug}";
	}
}