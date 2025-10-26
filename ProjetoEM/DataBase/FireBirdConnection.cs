using FirebirdSql.Data.FirebirdClient;

namespace ProjetoEM.DataBase
{
    public class FireBirdConnection(string connectionString)
    {
        private readonly string _connectionString = connectionString;

        public FbConnection CreateConnection()
        {
            var connection = new FbConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
