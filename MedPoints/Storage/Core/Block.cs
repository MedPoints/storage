using System;
using System.Collections.Generic;
using System.Text;
using Storage.Core.Transactions;
using Storage.Utils;

namespace Storage.Core
{
    public class Block
    {
        public string Hash { get; private set; }
        public string PreviousHash { get; private set; }
        public DateTime Time { get; private set; }
        public int Nonce { get; private set; }
        public string MerkleRoot { get; private set; }
        public List<CoinTransaction> CoinTransactions { get;} = new List<CoinTransaction>();
        public List<ITransaction> OtherTransactions { get;} = new List<ITransaction>(); 

        public Block( string previousHash)
        {
            PreviousHash = previousHash;
            Time = DateTime.UtcNow;
            Hash = CalculateHash();
        }

        public string CalculateHash()
        {
            var hash = $"{PreviousHash}{Time}{Nonce}{MerkleRoot}".GetSha256Hash();
            return hash;
        }

        public void MineBlock(int difficulty)
        {
            MerkleRoot = CoinTransactions.GenerateMerkleRoot();
            var target = string.Empty.PadLeft(difficulty, '0');
            while (Hash.Substring(0, difficulty) != target)
            {
                Nonce++;
                Hash = CalculateHash();
            }
            Console.WriteLine($"Block Mined - {Hash}");
        }

        public void AddTransaction(Dictionary<String, TransactionOutput> utxos, CoinTransaction coinTransaction)
        {
            if (PreviousHash != "0")
            {
                if(!coinTransaction.ProcessTransaction(utxos))
                    throw new Exception();
            }
            CoinTransactions.Add(coinTransaction);
        }
        
        public void AddTransaction(ITransaction transaction)
        {
            OtherTransactions.Add(transaction);
        }
    }
}
