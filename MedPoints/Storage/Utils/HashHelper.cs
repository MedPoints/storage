using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Storage.Core;
using Storage.Core.Transactions;

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

        public static string GenerateMerkleRoot(this List<CoinTransaction> transactions)
        {
            int count = transactions.Count;
            var previousTreeLayer = new List<string>();
            foreach (var transaction in transactions)
            {
                previousTreeLayer.Add(transaction.Id);
            }
            var treeLayer = previousTreeLayer;
            while (count > 1)
            {
                treeLayer = new List<string>();
                for (int i = 1; i < previousTreeLayer.Count; i++)
                {
                    var hash = $"{previousTreeLayer[i - 1]}{previousTreeLayer[i]}".GetSha256Hash();
                    treeLayer.Add(hash);
                }
                count = treeLayer.Count;
                previousTreeLayer = treeLayer;
            }

            return treeLayer.Count == 1 ? treeLayer[0] : "";
        }
    }
}
