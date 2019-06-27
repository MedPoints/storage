using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Storage.Core.Transactions;
using Storage.Utils;

namespace Storage.Core
{
    public class Wallet
    {
        public string PrivateKey { get; }
        public string PublicKey { get; }

        public Wallet()
        {
            PrivateKey = Guid.NewGuid().ToString().Base64Encode();
            PublicKey = PrivateKey.Substring(0, 5) + PrivateKey.Substring(10, 10);
        }
        
        public Wallet(string privateKey)
        {
            PrivateKey = privateKey;
            PublicKey = PrivateKey.Substring(0, 5) + PrivateKey.Substring(10, 10);
        }
        

        public decimal GetBalance(ConcurrentDictionary<String, TransactionOutput> chainUtxos)
        {
            decimal sum = 0;
            foreach (var utxo in chainUtxos)
            {
                var item = utxo.Value;
                if (item.IsMine(PublicKey))
                {
                    chainUtxos[item.Id] = item;
                    sum += item.Amount;
                }
            }
            return sum;
        }
        
        
        public static decimal GetBalanceByAddress(string address, ConcurrentDictionary<String, TransactionOutput> chainUtxos)
        {
            decimal sum = 0;
            foreach (var utxo in chainUtxos)
            {
                var item = utxo.Value;
                if (item.IsMine(address))
                {
                    chainUtxos[item.Id] = item;
                    sum += item.Amount;
                }
            }
            return sum;
        }

        public CoinTransaction Send(ConcurrentDictionary<String, TransactionOutput> chainUtxos, string recipient, decimal amount)
        {
            if (GetBalance(chainUtxos) < amount)
                return null;
            
            var inputs = new List<TransactionInput>();
            decimal sum = 0;
            foreach (var utxoItem in chainUtxos)
            {
                var utxo = utxoItem.Value;
                sum += utxo.Amount;
                inputs.Add(new TransactionInput(){TransactionOutputId = utxo.Id});
            }

            var tx = new CoinTransaction(PublicKey, recipient, amount, inputs);
            foreach (var transactionInput in inputs)
            {
                chainUtxos.TryRemove(transactionInput.TransactionOutputId, out var value);
            }

            return tx;
        }
    }
}
