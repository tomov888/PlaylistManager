using System.Text.RegularExpressions;
using PlaylistManager.Core.Common.Extensions;

namespace PlaylistManager.Core.Common.Utils;

public static class Slug
{
	public static string Slugify(string phrase)
	{
		string str = phrase.RemoveDiacritics().ToLower(); 
		
		str = str.Replace(".","-");
		str = Regex.Replace(str, @"[^a-z0-9\s-]", "");   
		str = Regex.Replace(str, @"\s+", " ").Trim(); 
		str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();   
		str = str.Replace(" ","");
		str = Regex.Replace(str, @"\s", "-");
		
		return str; 
	}
}