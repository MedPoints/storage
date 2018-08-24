using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Storage.Core.Transactions;

namespace StorageRest
{
    public class Helper
    {
        public static ConcurrentDictionary<String, TransactionOutput> UTXOs;
        public const string COIN_BASE = "ZTI3YEtOTI1My00";
        public const string COIN_BASE_PASSWORD = "ZTI3YzZkMGEtOTI1My00ZTZiLWFkNzYtZjA0ZDY5ODgyMjdj";
        
        public static object _lock = new object();

        static Helper()
        {
            if (File.Exists("utxos"))
                UTXOs = JsonConvert.DeserializeObject<ConcurrentDictionary<String, TransactionOutput>>(File.ReadAllText("utxos"));
            
            if(UTXOs == null)
                UTXOs = new ConcurrentDictionary<string, TransactionOutput>();
        }

        public static void Save()
        {
            lock (_lock)
            {
                File.WriteAllText("utxos", JsonConvert.SerializeObject(UTXOs));
            }
        }
        
    }
}