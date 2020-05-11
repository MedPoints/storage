using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Storage.Core.Transactions;

namespace Storage.Database
{
    public class AppointmentToTheDoctorRepository
    {
        private class TxInternal
        {
            public string TxHash { get; set; }
            public string UserId { get; set; }
            public TransactionType Type { get; set; }
            public string Transaction { get; set; }
        }
        
        private string _connectionString;

        public AppointmentToTheDoctorRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Add(string userId, AppointmentToTheDoctorTransaction tx)
        {
            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();
                dbConnection.Execute(
                    "INSERT INTO transactions (tx_hash, user_id, type, transaction) VALUES(@TxHash, @UserId,@Type,@Transaction)",
                    new TxInternal{TxHash = tx.Id, UserId = userId, Type = tx.Type, Transaction = tx.Serialize()});
            }
 
        }
        
        public List<AppointmentToTheDoctorTransaction> GetByDoctorId(string doctorId)
        {
            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();
                return dbConnection
                    .Query<TxInternal>("SELECT * FROM transactions where type == @Type and transaction->>'doctorId' = '@DoctorId';",
                        new {DoctorId = doctorId, Type = TransactionType.VisitToTheDoctor})
                    .Select(tx => TransactionExtensions.Deserialize<AppointmentToTheDoctorTransaction>(tx.Transaction)).ToList();
            }
        }


        public List<AppointmentToTheDoctorTransaction> GetByUserId(string userId)
        {
            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();
                return dbConnection
                    .Query<TxInternal>("SELECT * FROM transactions where user_id=@UserId and type=@Type",
                        new TxInternal {UserId = userId, Type = TransactionType.VisitToTheDoctor})
                    .Select(tx => TransactionExtensions.Deserialize<AppointmentToTheDoctorTransaction>(tx.Transaction)).ToList();
            }
        }
    }
}