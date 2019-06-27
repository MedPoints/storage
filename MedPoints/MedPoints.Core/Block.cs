using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoints.Core
{
    class Block
    {
        public string Hash { get; set; }
        public int Version { get; set; }
        public Block PreviousBlock { get; set; }
        public string MerkleRoot { get; set; }
        public decimal Timestamp { get; set; }
        public long Nonce { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}