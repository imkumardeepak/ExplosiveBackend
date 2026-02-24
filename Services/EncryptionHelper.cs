using Peso_Baseed_Barcode_Printing_System_API.Interface;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Peso_Baseed_Barcode_Printing_System_API.Services
{
	public class EncryptionHelper
	{
		private static readonly string Key = "Peso@Based@#2024"; // 16 characters for AES-128

		public static string Encrypt(string plaintext)
		{
			using (Aes aes = Aes.Create())
			{
				aes.Key = Encoding.UTF8.GetBytes(Key);
				aes.IV = new byte[16]; // Initialization Vector (IV) can be all zero for simplicity

				using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
				using (var ms = new MemoryStream())
				{
					using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
					using (var writer = new StreamWriter(cs))
					{
						writer.Write(plaintext);
					}

					return Convert.ToBase64String(ms.ToArray());
				}
			}
		}

		public static string Decrypt(string ciphertext)
		{
			using (Aes aes = Aes.Create())
			{
				aes.Key = Encoding.UTF8.GetBytes(Key);
				aes.IV = new byte[16]; // Same IV used during encryption

				using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
				using (var ms = new MemoryStream(Convert.FromBase64String(ciphertext)))
				using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
				using (var reader = new StreamReader(cs))
				{
					return reader.ReadToEnd();
				}
			}
		}
	}

}

