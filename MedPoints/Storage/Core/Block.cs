﻿using System;
using System.Collections.Generic;
using System.Text;
using Storage.Utils;

namespace Storage.Core
{
    public class Block
    {
        public string Hash { get; private set; }
        public string PreviousHash { get; private set; }
        public string Data { get; private set; }
        public DateTime Time { get; private set; }
        public int Nonce { get; private set; }

        public Block(string data, string previousHash)
        {
            Data = data;
            PreviousHash = previousHash;
            Time = DateTime.UtcNow;
            Hash = CalculateHash();
        }

        public string CalculateHash()
        {
            var hash = $"{PreviousHash}{Time}{Nonce}{Data}".GetSha256Hash();
            return hash;
        }

        public void MineBlock(int difficulty)
        {
            var target = string.Empty.PadLeft(difficulty, '0');
            while (Hash.Substring(0, difficulty) != target)
            {
                Nonce++;
                Hash = CalculateHash();
            }
            Console.WriteLine($"Block Mined - {Hash}");
        }
    }
}
