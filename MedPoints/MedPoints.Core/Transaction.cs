using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedPoints.Core.Cryptography;

namespace MedPoints.Core
{
    class Transaction
    {
        public decimal Amount
        {
            get
            {
                return Inputs.Sum(i => i.Output.Amount);
            }
        }

        public string Hash { get; private set; }
        public string From { get; private set; }
        public string To { get; private set; }
        public long Nonce { get; private set; }
        public TransactionType Type { get; private set; }
        public IEnumerable<TransactionInput> Inputs { get; private set; }
        public IEnumerable<TransactionOutput> Outputs { get; private set; }

        public Transaction(TransactionType type, string from, string to, IEnumerable<TransactionInput> transactionInputs)
        {
            Type = type;
            Inputs = transactionInputs;
            From = from;
            To = to;
            Nonce = Cryptography.Nonce.Generate();
            Hash = CalculateHash();
        }

        private string CalculateHash()
        {
            return Sha256.Calculate($"{From}{To}{Amount}{Nonce}");
        }
    }
}