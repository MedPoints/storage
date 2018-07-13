namespace Storage.Core.Transactions
{
    public interface ITransaction
    {
        TransactionType Type { get; }
    }
}