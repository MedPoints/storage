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
            public string UserId { get; set; }
            public TransactionType Type { get; set; }
            public string Transaction { get; set; }
        }
        
        private string connectionString;
        private IDbConnection Connection => new NpgsqlConnection(connectionString);
        
        public CoinTransactionRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetValue<string>("DBInfo:ConnectionString");
        }


        public void Add(string userId, CoinTransaction tx)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute(
                    "INSERT INTO transactions (user_id, type, transaction) VALUES(@UserId,@Type,@Transaction)",
                    new TxInternal{UserId = userId, Type = tx.Type, Transaction = tx.Serialize()});
            }
 
        }


        public List<CoinTransaction> GetByType(string userId, TransactionType type)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection
                    .Query<TxInternal>("SELECT * FROM transactions where user_id=@UserId and type=@Type",
                        new TxInternal {UserId = userId, Type = type})
                    .Select(tx => TransactionExtensions.Deserialize(tx.Transaction)).ToList();
            }
        }
    }
}