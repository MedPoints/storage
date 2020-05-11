using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;
using Storage.Core;
using Storage.Core.Transactions;
using Storage.Database;

namespace Storage.Mempool
{
    public class Mempool
    {
        private readonly MempoolRepository _mempoolRepository;
        private readonly BlockRepository _blockRepository;
        private readonly AppointmentToTheDoctorRepository _appointmentToTheDoctorRepository;
        private readonly CoinTransactionRepository _coinTransactionRepository;
        private ConcurrentDictionary<String, TransactionOutput> _UTXOs;

        public Mempool(
            MempoolRepository mempoolRepository, 
            BlockRepository blockRepository, 
            ConcurrentDictionary<String, TransactionOutput> UTXOs, 
            AppointmentToTheDoctorRepository appointmentToTheDoctorRepository, 
            CoinTransactionRepository coinTransactionRepository
            )
        {
            _mempoolRepository = mempoolRepository;
            _blockRepository = blockRepository;
            _UTXOs = UTXOs;
            _appointmentToTheDoctorRepository = appointmentToTheDoctorRepository;
            _coinTransactionRepository = coinTransactionRepository;
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
            {
                lastBlockHash = InsertGenesis(wallet);           
            }
        
            if(txs.Count == 0)
                return;
            
            var newBlock = new Block(lastBlockHash);          
            foreach (var tx in txs)
            {
                switch (tx.Type)
                {
                    case TransactionType.Coins:
                        var ctx = JsonConvert.DeserializeObject<CoinTransaction>(tx.Transaction);
                        _coinTransactionRepository.Add(tx.UserId, ctx);
                        newBlock.CoinTransactions.Add(ctx);
                        break;
                    case TransactionType.VisitToTheDoctor:
                        var vttdTransaction =
                            JsonConvert.DeserializeObject<AppointmentToTheDoctorTransaction>(tx.Transaction);
                        _appointmentToTheDoctorRepository.Add(tx.UserId, vttdTransaction);
                        newBlock.OtherTransactions.Add(vttdTransaction);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }       
            }
            

            var coinBase = new Wallet();
            var mainedBlockMoney = new CoinTransaction(coinBase.PublicKey, wallet.PublicKey, 100, null);
            mainedBlockMoney.Id = Guid.NewGuid().ToString();
            mainedBlockMoney.Outputs.Add(
                new TransactionOutput(mainedBlockMoney.Reciepient, mainedBlockMoney.Amount, mainedBlockMoney.Id)
            );
            _UTXOs[mainedBlockMoney.Outputs[0].Id] = mainedBlockMoney.Outputs[0];
            
            newBlock.AddTransaction(_UTXOs, mainedBlockMoney);
                
            newBlock.MineBlock(new Random().Next(0, 5));


            _blockRepository.Add(newBlock);
            
            foreach (var tx in txs)
            {
                _mempoolRepository.Delete(tx.Id);     
            }
        }

        private string InsertGenesis(Wallet wallet)
        {
            var coinBase = new Wallet();
            var genesisCoinTransaction = new CoinTransaction(coinBase.PublicKey, wallet.PublicKey, 100, null);
            genesisCoinTransaction.Id = "0";
            genesisCoinTransaction.Outputs.Add(
                new TransactionOutput(genesisCoinTransaction.Reciepient, genesisCoinTransaction.Amount, genesisCoinTransaction.Id)
            );
            _UTXOs[genesisCoinTransaction.Outputs[0].Id] = genesisCoinTransaction.Outputs[0];
            genesisCoinTransaction.Sender = coinBase.PublicKey;
            
            _coinTransactionRepository.Add(wallet.PublicKey, genesisCoinTransaction);
            var genesis = new Block("0");
            genesis.AddTransaction(_UTXOs, genesisCoinTransaction);
            
            _blockRepository.Add(genesis);
            return _blockRepository.GetLastBlockHash();
        }
    }
}