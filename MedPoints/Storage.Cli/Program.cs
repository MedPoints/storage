using System;
using Storage.Core;
using Storage.Data.LevelDB;

namespace Storage.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var block = new Block("Genesis", "0");
            block.MineBlock(3);
            var second = new Block("Second", block.Hash);

            /*var dataBase = DatabaseContext.Open("test", new Options { CreateIfMissing = true });
            dataBase.Put(WriteOptions.Default, block.Hash.ToSlice(), block.ToSlice());
            var data = dataBase.Get(ReadOptions.Default, block.Hash.ToSlice());
            var result = data.ToString();*/

        }
    }
}
