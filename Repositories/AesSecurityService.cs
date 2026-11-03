using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace systemapps.Repositories
{
    public interface ISecurityService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }

    public class AesSecurityService : ISecurityService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AesSecurityService(string secretKey)
        {
            // Ensure key is 32 bytes for AES-256
            using var sha256 = SHA256.Create();
            _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(secretKey));
            
            // In a real production app, IV should be unique per encryption 
            // and stored with the ciphertext. For this refactor, we use a fixed IV.
            _iv = new byte[16]; 
            Array.Copy(_key, _iv, 16);
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            
            return sr.ReadToEnd();
        }
    }
}
