using System;
using System.Collections.Generic;
using StorageRest.Utils;

namespace StorageRest.App
{
    public class Block
    {
        public string Hash { get; set; }
        public string PreviousHash { get; set; }
        public DateTime Time { get; set; }
        public int Nonce { get; set; }
        public List<VisitToDoctorTransaction> Transactions { get; set; } = new List<VisitToDoctorTransaction>();

        public Block(string previousHash)
        {
            PreviousHash = previousHash;
            Time = DateTime.UtcNow;
            Hash = CalculateHash();
        }

        public string CalculateHash()
        {
            var hash = $"{PreviousHash}{Time}{Nonce}".GetSha256Hash();
            return hash;
        }

        public void MineBlock(int difficulty)
        {
            var target = string.Empty.PadLeft(difficulty, '0');
            while (Hash.Substring(0, difficulty) != target)
            {
                Nonce++;
                Hash = CalculateHash();
            }
            Console.WriteLine($"Block Mined - {Hash}");
        }
  
        public void AddTransaction(VisitToDoctorTransaction transaction)
        {
            Transactions.Add(transaction);
        }
    }
}
