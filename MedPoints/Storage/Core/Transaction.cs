using System;
using System.Collections.Generic;
using System.Text;
using Storage.Utils;

namespace Storage.Core
{
    public class Transaction
    {
        public string Id { get; set; }
        public string Sender { get; set; }
        public string Reciepient { get; set; }
        public decimal Amount { get; set; }
        public byte[] Signature { get; set; }

        public List<TransactionInput> Inputs { get; set; } = new List<TransactionInput>();
        public List<TransactionOutput> Outputs { get; set; } = new List<TransactionOutput>();
        
        private static int Sequence { get; set; }

        public Transaction(string from, string to, decimal amount, List<TransactionInput> inputs,
            List<TransactionOutput> outputs)
        {
            Sender = from;
            Reciepient = to;
            Amount = amount;
            Inputs = inputs;
        }

        private string CalculateHash()
        {
            Sequence++;
            return $"{Sender}{Reciepient}{Amount}{Sequence}".GetSha256Hash();
        }

    }

    public class TransactionOutput
    {
    }

    public class TransactionInput
    {
    }
}
