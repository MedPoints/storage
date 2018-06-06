using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Storage.Utils
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
    }
}
