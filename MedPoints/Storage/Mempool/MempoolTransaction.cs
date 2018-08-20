using Storage.Core.Transactions;

namespace Storage.Mempool
{
    public class MempoolTransaction
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public TransactionType Type { get; set; }
        public string Transaction { get; set; }
    }
}