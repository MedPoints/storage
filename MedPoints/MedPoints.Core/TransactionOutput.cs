using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoints.Core
{
    class TransactionOutput
    {
        public decimal Amount { get; private set; }

        public TransactionOutput(decimal amount)
        {
            Amount = amount;
        }
    }
}