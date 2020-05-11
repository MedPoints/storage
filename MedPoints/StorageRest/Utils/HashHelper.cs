using System;
using System.Security.Cryptography;
using System.Text;

namespace StorageRest.Utils
{
    public static class HashHelper
    {
        public static string GetSha256Hash(this string value)
        {
            var sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
        
        public static string Base64Encode(this string plainText) {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
