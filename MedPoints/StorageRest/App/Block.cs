using System;
using System.Collections.Generic;
using StorageRest.Utils;

namespace StorageRest.App
{
    public class Block
    {
        public string Hash { get; private set; }
        public string PreviousHash { get; private set; }
        public DateTime Time { get; private set; }
        public int Nonce { get; private set; }
        public List<VisitToDoctorTransaction> Transactions { get;} = new List<VisitToDoctorTransaction>(); 

        public Block( string previousHash)
        {
            PreviousHash = previousHash;
            Time = DateTime.UtcNow;
            Hash = CalculateHash();
        }

        public string CalculateHash()
        {
            var hash = $"{PreviousHash}{Time}".GetSha256Hash();
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
