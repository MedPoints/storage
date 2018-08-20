using Storage.Core;
using Storage.Core.Transactions;
using Storage.Database;

namespace Storage.Mempool
{
    public class Mempool
    {
        private readonly MempoolRepository _mempoolRepository;

        public Mempool(MempoolRepository mempoolRepository)
        {
            _mempoolRepository = mempoolRepository;
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
            var block = new Block(();)
            foreach (var tx in txs)
            {
                
            }
            
            {
                
            }
        }
    }
}