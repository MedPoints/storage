﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StorageRest.App;

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
            var txs = _baseRepository.GetTransactions();
            if (txs.Count != 2)
            {
                _baseRepository.Add(tx);
                return;
            }

            var blockHash = _baseRepository.GetLastBlockHash();
            var newBlock = new Block(blockHash);
            txs.Add(tx);
            txs.ForEach(transaction => newBlock.AddTransaction(transaction));
            newBlock.MineBlock(3);
            _baseRepository.Add(newBlock);
        }
    }
}