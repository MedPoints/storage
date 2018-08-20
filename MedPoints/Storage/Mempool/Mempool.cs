using System;
using System.Collections.Generic;
using Storage.Core;
using Storage.Core.Transactions;
using Storage.Database;

namespace Storage.Mempool
{
    public class Mempool
    {
        private readonly MempoolRepository _mempoolRepository;
        private readonly BlockRepository _blockRepository;
        private static Dictionary<String, TransactionOutput> UTXOs = new Dictionary<String, TransactionOutput>();

        public Mempool(MempoolRepository mempoolRepository, BlockRepository blockRepository)
        {
            _mempoolRepository = mempoolRepository;
            _blockRepository = blockRepository;
        }


        public void AddToMempool(string data, string userId, TransactionType type)
        {
            _mempoolRepository.Add(new MempoolTransaction()
            {
                Transaction = data,
                UserId = userId,
                Type = type
            });
        }

        public void Mine(Wallet wallet)
        {
            var txs = _mempoolRepository.GetNextList();
            var lastBlockHash = _blockRepository.GetLastBlockHash();
            if (lastBlockHash == null)
                lastBlockHash = InsertGenesis(wallet);
            
            var newBlock = new Block(lastBlockHash);
            foreach (var tx in txs)
            {
                switch (tx.Type)
                {
                    case TransactionType.Coins:
                        break;
                    case TransactionType.VisitToTheDoctor:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            

            var coinBase = new Wallet();
            var mainedBlockMoney = new CoinTransaction(coinBase.PublicKey, wallet.PublicKey, 100, null);
            mainedBlockMoney.Sign(coinBase);
            mainedBlockMoney.Id = Guid.NewGuid().ToString();
            mainedBlockMoney.Outputs.Add(
                new TransactionOutput(mainedBlockMoney.Reciepient, mainedBlockMoney.Amount, mainedBlockMoney.Id)
            );
            UTXOs[mainedBlockMoney.Outputs[0].Id] = mainedBlockMoney.Outputs[0];
            
            newBlock.AddTransaction(UTXOs, mainedBlockMoney);
                
            newBlock.MineBlock(new Random().Next(0, 5));


            _blockRepository.Add(newBlock);
        }

        private string InsertGenesis(Wallet wallet)
        {
            var coinBase = new Wallet();
            var genesisCoinTransaction = new CoinTransaction(coinBase.PublicKey, wallet.PublicKey, 100, null);
            genesisCoinTransaction.Sign(coinBase);
            genesisCoinTransaction.Id = "0";
            genesisCoinTransaction.Outputs.Add(
                new TransactionOutput(genesisCoinTransaction.Reciepient, genesisCoinTransaction.Amount, genesisCoinTransaction.Id)
            );
            UTXOs[genesisCoinTransaction.Outputs[0].Id] = genesisCoinTransaction.Outputs[0];

            var genesis = new Block("0");
            genesis.AddTransaction(UTXOs, genesisCoinTransaction);
            
            _blockRepository.Add(genesis);
            return _blockRepository.GetLastBlockHash();
        }
    }
}