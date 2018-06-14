using System;
using System.Collections.Generic;
using System.Text;
using Storage.Utils;

namespace Storage.Core
{
    public class Transaction
    {
        public string Id { get; set; }
        public string Sender { get; set; }
        public string Reciepient { get; set; }
        public decimal Amount { get; set; }
        public string Signature { get; set; }

        public List<TransactionInput> Inputs { get; set; }
        public List<TransactionOutput> Outputs { get; set; } = new List<TransactionOutput>();
        
        private static int Sequence { get; set; }

        public Transaction(string from, string to, decimal amount, List<TransactionInput> inputs,
            List<TransactionOutput> outputs)
        {
            Sender = from;
            Reciepient = to;
            Amount = amount;
            Inputs = inputs;
        }

        private string CalculateHash()
        {
            Sequence++;
            return $"{Sender}{Reciepient}{Amount}{Sequence}".GetSha256Hash();
        }

        public void Sign(Wallet wallet)
        {
            var data = $"{Sender}{Reciepient}{Amount}";
            Signature = wallet.SignMessage(data);
        }

        public bool VerifySignature()
        {
            var data = $"{Sender}{Reciepient}{Amount}";
            return Wallet.VerifyMessage(data, Signature, Sender);
        }


        /*
         * public void generateSignature(PrivateKey privateKey) {
	            String data = StringUtil.getStringFromKey(sender) + StringUtil.getStringFromKey(reciepient) + Float.toString(value)	;
	            signature = StringUtil.applyECDSASig(privateKey,data);		
            }
            //Verifies the data we signed hasnt been tampered with
            public boolean verifiySignature() {
	            String data = StringUtil.getStringFromKey(sender) + StringUtil.getStringFromKey(reciepient) + Float.toString(value)	;
	            return StringUtil.verifyECDSASig(sender, data, signature);
            }*/

    }

    public class TransactionOutput
    {
        public string Id { get; set; }
        public string Reciepient { get; set; }
        public decimal Amount { get; set; }
        public string ParentTransactionId { get; set; }

        public bool IsMine(string publicKey)
        {
            return publicKey == Reciepient;
        }
    }

    public class TransactionInput
    {
        public string TransactionOutputId { get; set; }
        // ReSharper disable once InconsistentNaming
        public TransactionOutput UTXO { get; set; }
    }
}
