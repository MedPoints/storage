using System;
using System.Collections.Generic;
using System.Text;
using Storage.Core;

namespace Storage.Cli
{
    public class TestChain
    {
        public static List<Block> Blocks = new List<Block>();
        public static Dictionary<String, TransactionOutput> UTXOs = new Dictionary<String, TransactionOutput>();

        public static int Difficulty = 3;
        public static Wallet WalletA;
        public static Wallet WalletB;
        public static Transaction GenesisTransaction;

        public static void Test()
        {
            WalletA = new Wallet();
            WalletB = new Wallet();
            var coinBase = new Wallet();

            GenesisTransaction = new Transaction(coinBase.PublicKey, WalletA.PublicKey, 100, null);
            GenesisTransaction.Sign(coinBase);
            GenesisTransaction.Id = "0";
            GenesisTransaction.Outputs.Add(
                new TransactionOutput(GenesisTransaction.Reciepient, GenesisTransaction.Amount, GenesisTransaction.Id)
                );
            UTXOs[GenesisTransaction.Outputs[0].Id] = GenesisTransaction.Outputs[0];

            var genesis = new Block("0");
            genesis.AddTransaction(UTXOs, GenesisTransaction);
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
