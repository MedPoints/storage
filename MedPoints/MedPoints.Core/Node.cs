using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoints.Core
{
    public class Node
    {
        private readonly BlockChain BlockChain;
        private readonly NodeConfiguration Configuration;
        private readonly TransactionManager TransactionManager;

        public event EventHandler NewBlock;
        public event EventHandler NewTransaction;

        public Node()
        {
            Configuration = NodeConfiguration.Load();
            BlockChain = new BlockChain(Configuration.MinBlockSize);
            TransactionManager = new TransactionManager(BlockChain);
        }

        public void MakeTransaction(string from, string to, decimal amount, TransactionType type)
        {
            TransactionManager.Send(from, to, amount, type);
        }

        public void GetWalletAmount(string address)
        {

        }



        private void OnNewBlock()
        {
            NewBlock?.Invoke(this, EventArgs.Empty);
        }

        private void OnNewTransaction()
        {
            NewTransaction?.Invoke(this, EventArgs.Empty);
        }
    }
}