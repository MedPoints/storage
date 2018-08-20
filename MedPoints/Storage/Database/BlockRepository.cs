using System.Data;
using Dapper;
using Npgsql;
using Storage.Core;
using Storage.Core.Transactions;

namespace Storage.Database
{
    public class BlockRepository
    {
        private string connectionString;
        private IDbConnection Connection => new NpgsqlConnection(connectionString);
        
        public string GetLastBlockHash(){
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.QueryFirst<string>("SELECT hash FROM blocks ORDER BY id DESC LIMIT 1;");
            }
        }

        public void Add(Block block)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute(
                    "INSERT INTO blocks (hash, data) VALUES(@Hash,@Data)",
                    new {Hash = block.Hash, Data = block.Serialize()});
            }
        }
        
        
    }
    

}