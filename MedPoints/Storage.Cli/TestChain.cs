using System;
using System.Collections.Generic;
using System.Text;
using Storage.Core;
using Storage.Core.Transactions;

namespace Storage.Cli
{
    public class TestChain
    {
        private static List<Block> Blocks = new List<Block>();
        private static Dictionary<String, TransactionOutput> UTXOs = new Dictionary<String, TransactionOutput>();

        private static int Difficulty = 3;
        private static Wallet WalletA;
        private static Wallet WalletB;
        private static CoinTransaction _genesisCoinTransaction;

        public static void Test()
        {
            WalletA = new Wallet();
            WalletB = new Wallet();
            var coinBase = new Wallet();

            _genesisCoinTransaction = new CoinTransaction(coinBase.PublicKey, WalletA.PublicKey, 100, null);
            _genesisCoinTransaction.Sign(coinBase);
            _genesisCoinTransaction.Id = "0";
            _genesisCoinTransaction.Outputs.Add(
                new TransactionOutput(_genesisCoinTransaction.Reciepient, _genesisCoinTransaction.Amount, _genesisCoinTransaction.Id)
                );
            UTXOs[_genesisCoinTransaction.Outputs[0].Id] = _genesisCoinTransaction.Outputs[0];

            var genesis = new Block("0");
            genesis.AddTransaction(UTXOs, _genesisCoinTransaction);
            AddBlock(genesis);

            var balance = WalletA.GetBalance(UTXOs);
            var block1 = new Block(genesis.Hash);
            block1.AddTransaction(UTXOs, WalletA.Send(UTXOs, WalletB.PublicKey, 40));
            AddBlock(block1);

            balance = WalletA.GetBalance(UTXOs);
            var block2 = new Block(block1.Hash);
            block2.AddTransaction(UTXOs, WalletA.Send(UTXOs, WalletB.PublicKey, 1000));
            AddBlock(block2);

            balance = WalletA.GetBalance(UTXOs);
            var block3 = new Block(block2.Hash);
            block3.AddTransaction(UTXOs, WalletB.Send(UTXOs, WalletA.PublicKey,20));
            AddBlock(block3);

            balance = WalletA.GetBalance(UTXOs);
        }

        public static void AddBlock(Block newBlock)
        {
            newBlock.MineBlock(Difficulty);
            Blocks.Add(newBlock);
        }

    }
}
