using System.Security.Cryptography;
using System.Text;

namespace PlaylistManager.Core.Common.Utils;

public static class CryptographyProcessor
{
	public static string CreateSalt(int size)
	{
		//Generate a cryptographic random number.
		RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
		byte[] buff = new byte[size];
		rng.GetBytes(buff);
		return Convert.ToBase64String(buff);
	}

	public static string GenerateHash(string input, string salt)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(input + salt);
		SHA256Managed sHA256ManagedString = new SHA256Managed();
		byte[] hash = sHA256ManagedString.ComputeHash(bytes);
		return Convert.ToBase64String(hash);
	}

	public static bool AreEqual(string plainTextInput, string hashedInput, string salt)
	{
		string newHashedPin = GenerateHash(plainTextInput, salt);
		return newHashedPin.Equals(hashedInput);
	}
		
	public static string PasswordGenerator(int passwordLength, bool strongPassword)
	{
		var random = new Random();
		var seed = random.Next(1, int.MaxValue);
			
		const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
		const string specialCharacters = @"!#$%&'()*+,-./:;<=>?@[\]_";

		var chars = new char[passwordLength];
		var rd = new Random(seed);

		for (var i = 0 ; i < passwordLength; i++)
		{
			// If we are to use special characters
			if (strongPassword && i % random.Next(3, passwordLength) == 0 )
			{
				chars[i] = specialCharacters[rd.Next(0 , specialCharacters.Length)];
			}
			else
			{
				chars[i] = allowedChars[rd.Next(0 , allowedChars.Length)];
			}
		}

		return new string(chars);
	}		
}