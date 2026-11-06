using System.Security.Cryptography;
using System.Text;

namespace EDXS.Shared.Common.Security;

/// <summary>
/// The legacy app's password format (Cryption.EncryptPassword): Rijndael with key/IV
/// "password", zero padding, lowercase hex. utusers.password keeps this format so users
/// created in EDXS can still log into the legacy app; the BCrypt hash goes to utusers.passwordhash.
/// </summary>
public static class LegacyPasswordCipher
{
	public static string Encrypt(string value)
	{
		var key = new byte[24];
		var iv = new byte[16];
		Encoding.ASCII.GetBytes("password").CopyTo(key, 0);
		Encoding.ASCII.GetBytes("password").CopyTo(iv, 0);

#pragma warning disable SYSLIB0022 // the legacy format needs Rijndael's 128-bit block with zero padding
		using var rijndael = new RijndaelManaged { BlockSize = 128, KeySize = 192, Padding = PaddingMode.Zeros };
#pragma warning restore SYSLIB0022
		using var memStream = new MemoryStream();
		using (var cryptoStream = new CryptoStream(memStream, rijndael.CreateEncryptor(key, iv), CryptoStreamMode.Write))
		using (var writer = new StreamWriter(cryptoStream))
			writer.Write(value);

		return Convert.ToHexString(memStream.ToArray()).ToLowerInvariant();
	}

	public static bool IsBCryptHash(string? hash) =>
		hash != null && (hash.StartsWith("$2a$", StringComparison.Ordinal)
			|| hash.StartsWith("$2b$", StringComparison.Ordinal)
			|| hash.StartsWith("$2y$", StringComparison.Ordinal));
}
