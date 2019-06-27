using MedPoints.Core.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoints.Core
{
    class BlockChain
    {
        private List<Block> blocks;
        private List<Transaction> unbindedTransactions;
        private Block lastBlock;
        private int minBlockSize;

        public BlockChain(int minBlockSize)
        {
            blocks = new List<Block>();
            unbindedTransactions = new List<Transaction>();
            this.minBlockSize = minBlockSize;
        }

        public Block CreateBlock(Block previousBlock)
        {
            var block = new Block
            {
                Nonce = Cryptography.Nonce.Generate(),
                MerkleRoot = MerkleTree.GenerateMerkleRoot(unbindedTransactions),
                PreviousBlock = lastBlock
            };

            lastBlock = block;

            unbindedTransactions = new List<Transaction>();

            return lastBlock;
        }

        public void AddTransaction(Transaction transaction)
        {
            unbindedTransactions.Add(transaction);
        }
    }
}