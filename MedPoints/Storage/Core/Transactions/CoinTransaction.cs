using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Storage.Utils;

namespace Storage.Core.Transactions
{
    public class CoinTransaction : ITransaction
    {
        public string Reciepient { get; private set; }
        public decimal Amount { get; private set; }
        public string Id { get; set; }
        public string Sender { get; set; }
        public string Signature { get; set; }
        public TransactionType Type => TransactionType.Coins;

        private List<TransactionInput> Inputs { get; } = new List<TransactionInput>();
        public List<TransactionOutput> Outputs { get; } = new List<TransactionOutput>();

        private static int Sequence { get; set; }

        public CoinTransaction(
            string from, 
            string to, 
            decimal amount, 
            List<TransactionInput> inputs
        )
        {
            Sender = from;
            Reciepient = to;
            Amount = amount;
            Inputs = inputs ?? new List<TransactionInput>();
        }

        public string CalculateHash()
        {
            Sequence++;
            return $"{Sender}{Reciepient}{Amount}{Sequence}".GetSha256Hash();
        }


        public bool ProcessTransaction(ConcurrentDictionary<String, TransactionOutput> utxos)
        {
            foreach (var input in Inputs)
            {
                input.UTXO = utxos[input.TransactionOutputId];
            }

            var leftOver = GetInputsValue() - Amount;
            Id = CalculateHash();

            Outputs.Add(new TransactionOutput(reciepient: Reciepient, amount: Amount, parentTransactionId: Id));
            Outputs.Add(new TransactionOutput(reciepient: Sender, amount: leftOver, parentTransactionId: Id));

            foreach (var transactionOutput in Outputs)
            {
                utxos[transactionOutput.Id] = transactionOutput;
            }


            foreach (var transactionInput in Inputs)
            {
                if(transactionInput.UTXO == null)continue;
                utxos.TryRemove(transactionInput.UTXO.Id, out var value);
            }

            return true;
        }

        private decimal GetInputsValue()
        {
            decimal sum = 0;
            foreach (var transactionInput in Inputs)
            {
                if (transactionInput.UTXO == null) continue;
                sum += transactionInput.UTXO.Amount;
            }
            return sum;
        }

        public decimal GetOutputsValue()
        {
            decimal total = 0;
            foreach (var output in Outputs)
            {
                total += output.Amount;
            }
            return total;
        }


    }

    public class TransactionOutput
    {
        public string Id { get; }
        public string Reciepient { get; }
        public decimal Amount { get; }
        public string ParentTransactionId { get; }

        public TransactionOutput(string reciepient, decimal amount, string parentTransactionId)
        {
            Reciepient = reciepient;
            Amount = amount;
            ParentTransactionId = parentTransactionId;
            Id = $"{Reciepient}{Amount}{ParentTransactionId}".GetSha256Hash();
        }


        public bool IsMine(string publicKey)
        {
            return publicKey == Reciepient;
        }
    }

    public class TransactionInput
    {
        public string TransactionOutputId { get; set; }

        // ReSharper disable once InconsistentNaming
        public TransactionOutput UTXO { get; set; }
    }

    public static class TransactionExtensions
    {
        public static string Serialize(this Block block)
        {
            var serializerSettings =
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
            return JsonConvert.SerializeObject(block, serializerSettings);
        }
              
        public static string Serialize(this ITransaction tx)
        {
            var serializerSettings =
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
            return JsonConvert.SerializeObject(tx, serializerSettings);
        }

        public static T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}