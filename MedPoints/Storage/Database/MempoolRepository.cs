using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Storage.Mempool;

namespace Storage.Database
{
    public class MempoolRepository
    {
        private string connectionString;
        private IDbConnection Connection => new NpgsqlConnection(connectionString);

        public MempoolRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetValue<string>("DBInfo:ConnectionString");
        }


        public void Add(MempoolTransaction tx)
        {
            tx.Id = Guid.NewGuid().ToString();
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute(
                    "INSERT INTO mempool (id, user_id, type, transaction) VALUES(@Id, @UserId, @Type, @Transaction)",
                    tx);
            }
        }

        public MempoolTransaction GetNext()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection
                    .QueryFirst("SELECT * FROM mempool LIMIT 1");
            }
        }

        public List<MempoolTransaction> GetNextList()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection
                    .QueryFirst("SELECT * FROM mempool LIMIT 10");
            }
        }


        public void Delete(string txId)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("DELETE FROM mempool WHERE id = @TxId", new {TxId = txId});
            }
        }
    }
}