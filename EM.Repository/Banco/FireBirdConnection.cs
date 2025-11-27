using FirebirdSql.Data.FirebirdClient;

namespace EM.Repository.Banco
{
    public class FireBirdConnection(string connectionString)
    {
        private readonly string _connectionString = connectionString;

        public FbConnection CreateConnection()
        {
            FbConnection connection = new(_connectionString);
            connection.Open();
            return connection;
        }
    }
}

