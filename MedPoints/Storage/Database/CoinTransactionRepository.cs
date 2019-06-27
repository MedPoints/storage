using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Storage.Core;
using Storage.Core.Transactions;

namespace Storage.Database
{
    public class CoinTransactionRepository
    {
        private class TxInternal
        {
            public string TxHash { get; set; }
            public string UserId { get; set; }
            public TransactionType Type { get; set; }
            public string Transaction { get; set; }
        }
        
        private string _connectionString;

        public CoinTransactionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public void Add(string userId, CoinTransaction tx)
        {
            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();
                dbConnection.Execute(
                    "INSERT INTO transactions (tx_hash, user_id, type, transaction) VALUES(@TxHash, @UserId,@Type,to_jsonb(@Transaction))",
                    new TxInternal{TxHash = tx.Id, UserId = userId, Type = tx.Type, Transaction = tx.Serialize()});
            }
 
        }


        public List<CoinTransaction> GetByUserId(string userId)
        {
            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();
                return dbConnection
                    .Query<TxInternal>("SELECT * FROM transactions where user_id=@UserId and type=@Type",
                        new TxInternal {UserId = userId, Type = TransactionType.Coins})
                    .Select(tx => TransactionExtensions.Deserialize<CoinTransaction>(tx.Transaction)).ToList();
            }
        }
    }
}