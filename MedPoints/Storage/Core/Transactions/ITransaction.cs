namespace Storage.Core.Transactions
{
    public interface ITransaction
    {
        string Id { get; set; }
        string Sender { get; set; }
        string Signature { get; set; }
        TransactionType Type { get; }
    }
}