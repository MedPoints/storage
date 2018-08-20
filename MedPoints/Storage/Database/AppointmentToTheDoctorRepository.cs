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
            public string UserId { get; set; }
            public TransactionType Type { get; set; }
            public string Transaction { get; set; }
        }
        
        private string connectionString;
        private IDbConnection Connection => new NpgsqlConnection(connectionString);
        
        public AppointmentToTheDoctorRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetValue<string>("DBInfo:ConnectionString");
        }


        public void Add(string userId, AppointmentToTheDoctorTransaction tx)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute(
                    "INSERT INTO transactions (user_id, type, transaction) VALUES(@UserId,@Type,@Transaction)",
                    new TxInternal{UserId = userId, Type = tx.Type, Transaction = tx.Serialize()});
            }
 
        }
        
        public List<AppointmentToTheDoctorTransaction> GetByDoctorId(string doctorId)
        {
            using (IDbConnection dbConnection = Connection)
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
            using (IDbConnection dbConnection = Connection)
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