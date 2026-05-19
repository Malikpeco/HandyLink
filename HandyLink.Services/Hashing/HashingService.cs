using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Hashing
{
    public class HashingService : IHashingService
    {
        private const int iterations = 10000;
        private const int HashByteSize = 20;

        public string GenerateSalt()
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
            return Convert.ToBase64String(saltBytes);
        }
        public string HashText(string text, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(text, saltBytes, iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(HashByteSize);
                return Convert.ToBase64String(hash);
            }
        }

        public bool Verify(string hash, string salt, string providedText)
        {
            var generatedHash = HashText(providedText, salt);
            return generatedHash == hash;
        }
    }
}
