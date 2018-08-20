using System.Data;
using Npgsql;

namespace Storage.Database
{
    public class BlockRepository
    {
        private string connectionString;
        private IDbConnection Connection => new NpgsqlConnection(connectionString);
    }
}