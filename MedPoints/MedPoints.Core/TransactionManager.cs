using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoints.Core
{
    class TransactionManager
    {
        private readonly BlockChain BlockChain;

        public TransactionManager(BlockChain blockChain)
        {
            BlockChain = blockChain;
        }

        public void Send(string from, string to, decimal amount, TransactionType type)
        {

        }
    }
}