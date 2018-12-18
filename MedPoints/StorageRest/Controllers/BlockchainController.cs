using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StorageRest.App;
using StorageRest.Utils;

namespace StorageRest.Controllers
{
    [Route("api/blockchain")]
    [ApiController]
    public class BlockchainController : ControllerBase
    {
        private readonly SqLiteBaseRepository _baseRepository;

        public BlockchainController(SqLiteBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }

        [HttpPost]
        [Route("wallets")]
        public Wallet CreateWallet()
        {
            return new Wallet();
        }

        [HttpGet]
        [Route("blocks")]
        public List<Block> GetBlocks()
        {
            return _baseRepository.GetBlocks();
        }

        [HttpPost]
        [Route("transactions")]
        public void CreateTransactions([FromBody] VisitToDoctorTransaction tx)
        {
            tx.Id = Guid.NewGuid().ToString().Base64Encode();
            var txs = _baseRepository.GetTransactions() ?? new List<VisitToDoctorTransaction>();

            var blockHash = _baseRepository.GetLastBlockHash();
            var newBlock = new Block(blockHash);
            txs.Add(tx);
            txs.ForEach(transaction => newBlock.AddTransaction(transaction));
            newBlock.MineBlock(3);
            _baseRepository.Add(newBlock);
            
            txs.ForEach(transaction => _baseRepository.Remove(transaction));
        }
        
        [HttpGet]
        [Route("/{userAddress}/transactions")]
        public List<VisitToDoctorTransaction> GetTransactions(string userAddress)
        {
            return _baseRepository.GetTransactionsByUserAddress(userAddress.Trim());
        }
        
    }
}
