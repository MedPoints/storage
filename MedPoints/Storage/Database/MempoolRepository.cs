using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Storage.Core.Transactions;
using Storage.Mempool;

namespace Storage.Database
{
    public class MempoolRepository
    {
        private readonly string _connectionString;

        public MempoolRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }


        public void Add(MempoolTransaction tx)
        {
            tx.Id = Guid.NewGuid().ToString();
            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();
                dbConnection.Execute(
                    "INSERT INTO mempool (tx_hash, user_id, type, transaction) VALUES(@Id, @UserId, @Type, to_jsonb(@Transaction))",
                    new {Id = tx.Id, UserId = tx.UserId, Type = (int) tx.Type, Transaction = tx.Transaction});
            }
        }

        public MempoolTransaction GetNext()
        {
            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();
                return dbConnection
                    .QueryFirst("SELECT * FROM mempool LIMIT 1");
            }
        }

        public List<MempoolTransaction> GetNextList()
        {
            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();
                return dbConnection
                    .Query("SELECT * FROM mempool LIMIT 10").Select(x => new MempoolTransaction
                    {
                        Id = x.tx_hash,
                        UserId = x.user_id,
                        Type = (TransactionType) x.type,
                        Transaction = x.transaction
                    }).ToList();
            }
        }


        public void Delete(string txId)
        {
            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();
                dbConnection.Execute("DELETE FROM mempool WHERE tx_hash = @TxId", new {TxId = txId});
            }
        }
    }
}