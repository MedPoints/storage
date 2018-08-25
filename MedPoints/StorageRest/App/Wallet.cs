using System;
using StorageRest.Utils;

namespace StorageRest.App
{
    public class Wallet
    {
        public string PrivateKey { get; }
        public string PublicKey { get; }

        public Wallet()
        {
            PrivateKey = Guid.NewGuid().ToString().Base64Encode();
            PublicKey = PrivateKey.Substring(0, 5) + PrivateKey.Substring(10, 10);
        }
        
        public Wallet(string privateKey)
        {
            PrivateKey = privateKey;
            PublicKey = PrivateKey.Substring(0, 5) + PrivateKey.Substring(10, 10);
        }
    }
}
