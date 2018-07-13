using System;
using System.Collections.Generic;
using Storage.Core;
using Storage.Utils;

namespace Storage.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            TestChain.Test();

            /*var sender = new Wallet();
            var receipent = new Wallet();

           var transaction = new Transaction(sender.PublicKey, receipent.PublicKey, 20, null, null);
           transaction.Sign(sender);
           transaction.VerifySignature();*/

            /*var dataBase = DatabaseContext.Open("test", new Options { CreateIfMissing = true });
            dataBase.Put(WriteOptions.Default, block.Hash.ToSlice(), block.ToSlice());
            var data = dataBase.Get(ReadOptions.Default, block.Hash.ToSlice());
            var result = data.ToString();*/

        }
    }
}
