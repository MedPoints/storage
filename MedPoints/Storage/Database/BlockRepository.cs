using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Newtonsoft.Json;
using Npgsql;
using Storage.Core;
using Storage.Core.Transactions;

namespace Storage.Database
{
    public class BlockRepository
    {
        private class BlockInternal
        {
            public string Hash { get; set; }
            public string Data { get; set; }
        }
        
        private string connectionString;

        public BlockRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private IDbConnection Connection => new NpgsqlConnection(connectionString);
        
        public string GetLastBlockHash(){
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.QueryFirstOrDefault<string>("SELECT hash FROM blocks ORDER BY id DESC LIMIT 1;");
            }
        }
        
        public List<Block> GetBlocks(){
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection
                    .Query<BlockInternal>("SELECT * FROM blocks")
                    .Select(tx => JsonConvert.DeserializeObject<Block>(tx.Data)).ToList();
            }
        }

        public void Add(Block block)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute(
                    "INSERT INTO blocks (hash, data) VALUES(@Hash,to_jsonb(@Data))",
                    new {Hash = block.Hash, Data = block.Serialize()});
            }
        }
        
        
    }
    

}