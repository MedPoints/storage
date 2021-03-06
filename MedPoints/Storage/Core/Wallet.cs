﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Storage.Core.Transactions;
using Storage.Utils;

namespace Storage.Core
{
    public class Wallet
    {
        public string PrivateKey { get; }
        public string PublicKey { get; }

        private readonly UnicodeEncoding _encoder = new UnicodeEncoding();
        // ReSharper disable once InconsistentNaming
        public Dictionary<String, TransactionOutput> UTXOs { get; set; } = new Dictionary<String, TransactionOutput>();

        public Wallet()
        {
            using (var rsa = RSA.Create())
            {
                PrivateKey = rsa.ToJsonString(true);
                PublicKey = rsa.ToJsonString(false);
            }
        }

        public Wallet(string privateKey)
        {
            PrivateKey = privateKey;
            var rsa = new RSACryptoServiceProvider();
            rsa.FromJsonString(privateKey);
            PublicKey = rsa.ToJsonString(false);
            rsa.Dispose();
        }

        public decimal GetBalance(Dictionary<String, TransactionOutput> chainUtxos)
        {
            decimal sum = 0;
            foreach (var utxo in chainUtxos)
            {
                var item = utxo.Value;
                if (item.IsMine(PublicKey))
                {
                    UTXOs[item.Id] = item;
                    sum += item.Amount;
                }
            }
            return sum;
        }

        public CoinTransaction Send(Dictionary<String, TransactionOutput> chainUtxos, string recipient, decimal amount)
        {
            if (GetBalance(chainUtxos) < amount)
                return null;
            
            var inputs = new List<TransactionInput>();
            decimal sum = 0;
            foreach (var utxoItem in UTXOs)
            {
                var utxo = utxoItem.Value;
                sum += utxo.Amount;
                inputs.Add(new TransactionInput(){TransactionOutputId = utxo.Id});
                if(sum > amount) break;
            }

            var tx = new CoinTransaction(PublicKey, recipient, amount, inputs);
            tx.Sign(this);

            foreach (var transactionInput in inputs)
            {
                UTXOs.Remove(transactionInput.TransactionOutputId);
            }

            return tx;
        }

        public string Decrypt(string data)
        {
            var rsa = new RSACryptoServiceProvider();
            var dataArray = data.Split(new char[] { ',' });
            byte[] dataByte = new byte[dataArray.Length];
            for (int i = 0; i < dataArray.Length; i++)
            {
                dataByte[i] = Convert.ToByte(dataArray[i]);
            }

            rsa.FromJsonString(PrivateKey);
            var decryptedByte = rsa.Decrypt(dataByte, false);
            return _encoder.GetString(decryptedByte);
        }

        public string Encrypt(string data)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromJsonString(PublicKey);
            var dataToEncrypt = _encoder.GetBytes(data);
            var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
            var length = encryptedByteArray.Count();
            var item = 0;
            var sb = new StringBuilder();
            foreach (var x in encryptedByteArray)
            {
                item++;
                sb.Append(x);

                if (item < length)
                    sb.Append(",");
            }

            return sb.ToString();
        }

        public string SignMessage(string message)
        {
            string signedMessage;
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
                //Initiate a new instanse with 2048 bit key size

                rsa.FromJsonString(PrivateKey);
                // Load private key

                signedMessage = Convert.ToBase64String(rsa.SignData(Encoding.UTF8.GetBytes(message), CryptoConfig.MapNameToOID("SHA512")));
                //rsa.SignData( buffer, hash algorithm) - For signed data. Here I used SHA512 for hash. 
                //Encoding.UTF8.GetBytes(string) - convert string to byte messafe 
                //Convert.ToBase64String(string) - convert back to a string.
            }
            catch (Exception)
            {
                signedMessage = String.Empty;
            }

            return signedMessage;
        }

        public byte[] SignMessageInBytes(string message)
        {
            byte[] signedMessage;
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
                rsa.FromJsonString(PrivateKey);
                signedMessage = rsa.SignData(Encoding.UTF8.GetBytes(message), CryptoConfig.MapNameToOID("SHA512"));
            }
            catch (Exception)
            {
                signedMessage = default(byte[]);
            }

            return signedMessage;
        }

        public static bool VerifyMessage(string originalMessage, string signedMessage, string publicKey)
        {
            bool verified;
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
                rsa.FromJsonString(publicKey);
                // load public key 
                verified = rsa.VerifyData(Encoding.UTF8.GetBytes(originalMessage), CryptoConfig.MapNameToOID("SHA512"), Convert.FromBase64String(signedMessage));
            }
            catch (Exception)
            {
                verified = false;
            }

            return verified;
        }


    }
}
