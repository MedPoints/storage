namespace Storage.Core.Block.Transaction
{
    public interface ITransaction
    {
        TransactionType Type { get; set; }
        string From { get; set; }
        string To { get; set; }
    }
}
