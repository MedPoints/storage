namespace Storage.Core.Block
{
    public class Block
    {
        public int Id { get; set; }

        public Header.Header Header { get; set; }

        public string Hash { get; set; }
        public string Transactions { get; set; }
    }
}
