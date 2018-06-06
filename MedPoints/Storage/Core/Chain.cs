using System;
using System.Collections.Generic;
using System.Text;

namespace Storage.Core
{
    public class Chain
    {
        public static List<Block> Blockchain = new List<Block>();

        public static bool IsChainValid()
        {
            for (int i = 1; i < Blockchain.Count; i++)
            {
                var currentBlock = Blockchain[i];
                var previousBlock = Blockchain[i - 1];
                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    Console.WriteLine("Current Hashes not equal");
                    return false;
                }

                if (previousBlock.Hash != currentBlock.PreviousHash)
                {
                    Console.WriteLine("Previous Hashes not equal");
                    return false;
                }
            }
            return true;
        }


    }
}
