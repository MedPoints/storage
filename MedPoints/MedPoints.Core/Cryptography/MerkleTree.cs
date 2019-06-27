using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoints.Core.Cryptography
{
    static class MerkleTree
    {
        public static string GenerateMerkleRoot(IEnumerable<Transaction> transactions)
        {
            if (transactions == null || transactions.Count() == 0)
                return string.Empty;

            var hashes = transactions
                .Select(t => t.Hash)
                .ToList();

            if (hashes.Count == 1)
                return hashes[0];

            return GenerateMerkleRoot(hashes);
        }

        private static string GenerateMerkleRoot(List<string> hashes)
        {
            if (hashes.Count % 2 > 0)
                hashes.Add(hashes.Last());

            var branches = new List<string>();

            for (int i = 0; i < hashes.Count; i += 2)
            {
                var hashPair = hashes[i] + hashes[i + 1];
                branches.Add(Sha256.Calculate(Sha256.Calculate(hashPair)));
            }

            return GenerateMerkleRoot(branches);
        }
    }
}