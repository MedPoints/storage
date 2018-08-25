using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace StorageRest.App
{
    public class SqLiteBaseRepository
    {
        public static string DbFile => Environment.CurrentDirectory + "SimpleDb.sqlite";

        public static SqliteConnection NewConnection()
        {
            return new SqliteConnection("Data Source=" + DbFile);
        }

        public static void InitDatabase()
        {
            if (File.Exists(DbFile))
                return;
            
            
            File.Create(DbFile).Dispose();
            using (var cnn = NewConnection())
            {
                cnn.Open();
                cnn.Execute(
                    @"create table Blocks
                      (
                         ID                                  integer primary key AUTOINCREMENT,
                         Hash                                varchar not null,
                         Data                                varchar not null
                      )");

                cnn.Execute(
                    @"create table Transactions
                      (
                         ID                                  integer primary key AUTOINCREMENT,
                         Hash                                varchar not null,
                         Data                                varchar not null
                      )");

                var genesis = new Block("0");
                cnn.Execute(
                    @"INSERT INTO Blocks ( Hash, Data) VALUES ( @Hash, @Data);",
                    new {genesis.Hash, Data = JsonConvert.SerializeObject(genesis)}
                );
            }
        }


        public string GetLastBlockHash()
        {
            using (IDbConnection dbConnection = NewConnection())
            {
                dbConnection.Open();
                return dbConnection
                    .QueryFirst<string>("SELECT Hash from Blocks order by ID DESC limit 1");
            }
        }

        public List<Block> GetBlocks()
        {
            using (IDbConnection dbConnection = NewConnection())
            {
                dbConnection.Open();
                return dbConnection
                    .Query("SELECT * FROM Blocks")
                    .Select(record => JsonConvert.DeserializeObject<Block>(record.Data))
                    .Cast<Block>()
                    .ToList();
            }
        }

        public void Add(Block block)
        {
            using (IDbConnection dbConnection = NewConnection())
            {
                dbConnection.Open();
                dbConnection.Execute(
                    @"INSERT INTO Blocks ( Hash, Data) VALUES ( @Hash, @Data);",
                    new {block.Hash, Data = JsonConvert.SerializeObject(block)});
            }
        }
        
        public void Add(VisitToDoctorTransaction transaction)
        {
            using (IDbConnection dbConnection = NewConnection())
            {
                dbConnection.Open();
                dbConnection.Execute(
                    @"INSERT INTO Transactions ( Hash, Data) VALUES ( @Hash, @Data);",
                    new { Hash = transaction.Id, Data = JsonConvert.SerializeObject(transaction)});
            }
        }

        public List<VisitToDoctorTransaction> GetTransactions()
        {
            using (IDbConnection dbConnection = NewConnection())
            {
                dbConnection.Open();
                return dbConnection
                    .Query("SELECT * FROM Transactions")
                    .Select(record => JsonConvert.DeserializeObject<VisitToDoctorTransaction>(record.Data))
                    .Cast<VisitToDoctorTransaction>()
                    .ToList();
            }
        }
        
        
        public void Remove(VisitToDoctorTransaction transaction)
        {
            using (IDbConnection dbConnection = NewConnection())
            {
                dbConnection.Open();
                dbConnection.Execute(
                    @"DELETE FROM Transactions WHERE Hash = @Hash;",
                    new { Hash = transaction.Id});
            }
        }
    }
}