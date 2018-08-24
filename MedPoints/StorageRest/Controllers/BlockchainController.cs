using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Storage.Core;
using Storage.Core.Transactions;
using Storage.Database;
using Storage.Mempool;

namespace StorageRest.Controllers
{
    [Route("api/blockchain")]
    [ApiController]
    public class BlockchainController : ControllerBase
    {
        private readonly Mempool _mempool;
        private readonly BlockRepository _blockRepository;

        public BlockchainController(Mempool mempool, BlockRepository blockRepository)
        {
            _mempool = mempool;
            _blockRepository = blockRepository;
        }

        [HttpPost]
        [Route("wallets")]
        public Wallet Get()
        {
            try
            {
                var wallet = new Wallet();
                var debit = new CoinTransaction(Helper.COIN_BASE, wallet.PublicKey, 100, null);
                _mempool.AddToMempool(JsonConvert.SerializeObject(debit), wallet.PublicKey, debit.Type);
                return wallet;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }                     
        }

        [HttpGet]
        [Route("wallets/balance/{address}")]
        public decimal GetBalance(string address)
        {
            return Wallet.GetBalanceByAddress(address, Helper.UTXOs);
        }
        
        
        [HttpPost]
        [Route("appointment-transactions")]
        public void Post(AppointmentToTheDoctorTransaction tx)
        {
            tx.Id = Guid.NewGuid().ToString();
            _mempool.AddToMempool(JsonConvert.SerializeObject(tx), tx.Sender, tx.Type);
        }


        [HttpGet]
        [Route("blocks")]
        public List<Block> GetBlocks()
        {
            return _blockRepository.GetBlocks();
        }
        
        [HttpPost]
        [Route("mine-block/{privateKey}")]
        public void MineBlock(string privateKey)
        {    
            _mempool.Mine(new Wallet(privateKey));
            Helper.Save();
        }
        
        [HttpPost]
        [Route("send-coins/{fromKey}/{to}/{value}")]
        public void MineBlock(string fromKey, string to, decimal value)
        {   
            var wallet = new Wallet(fromKey);
            var debit = wallet.Send(Helper.UTXOs, to, value);
            if(debit == null)
                throw new Exception();
            debit.ProcessTransaction(Helper.UTXOs);


            var aa = JsonConvert.SerializeObject(debit);
            var bb = JsonConvert.DeserializeObject<CoinTransaction>(aa);
            
            _mempool.AddToMempool(JsonConvert.SerializeObject(debit), wallet.PublicKey, debit.Type);
            _mempool.Mine(new Wallet(Helper.COIN_BASE_PASSWORD));
            Helper.Save();
        }
    }
}