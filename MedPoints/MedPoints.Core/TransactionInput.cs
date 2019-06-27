using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoints.Core
{
    class TransactionInput
    {
        public TransactionOutput Output { get; private set; }

        public TransactionInput(TransactionOutput output)
        {
            Output = output;
        }
    }
}