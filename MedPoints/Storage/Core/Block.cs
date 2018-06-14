using System;
using System.Collections.Generic;
using System.Text;
using Storage.Utils;

namespace Storage.Core
{
    public class Block
    {
        public string Hash { get; private set; }
        public string PreviousHash { get; private set; }
        public string Data { get; private set; }
        public DateTime Time { get; private set; }
        public int Nonce { get; private set; }
        public string MerkleRoot { get; private set; }
        public List<Transaction> Transactions { get;} = new List<Transaction>();

        public Block(string data, string previousHash)
        {
            Data = data;
            PreviousHash = previousHash;
            Time = DateTime.UtcNow;
            Hash = CalculateHash();
        }

        public string CalculateHash()
        {
            var hash = $"{PreviousHash}{Time}{Nonce}{Data}".GetSha256Hash();
            return hash;
        }

        public void MineBlock(int difficulty)
        {
            MerkleRoot = Transactions.GenerateMerkleRoot();
            var target = string.Empty.PadLeft(difficulty, '0');
            while (Hash.Substring(0, difficulty) != target)
            {
                Nonce++;
                Hash = CalculateHash();
            }
            Console.WriteLine($"Block Mined - {Hash}");
        }

        public void AddTransaction(Dictionary<String, TransactionOutput> utxos, Transaction transaction)
        {
            if (PreviousHash != "0")
            {
                if(!transaction.ProcessTransaction(utxos))
                    throw new Exception();
            }
            Transactions.Add(transaction);
        }
    }
}
