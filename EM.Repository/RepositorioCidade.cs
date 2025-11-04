using FirebirdSql.Data.FirebirdClient;
using EM.Domain;
using EM.Domain.Interface;
using EM.Domain.Utilitarios;
using EM.Repository.Banco;

namespace EM.Repository
{
    public class RepositorioCidade(FireBirdConnection connection) : ICidadeRepository
    {
        private FireBirdConnection _connection = connection;
        
        public Cidade? ObtenhaCidade(int cod)
        {
            using FbConnection connection = _connection.CreateConnection();
            using FbCommand command = new FbCommand(
                @"SELECT C.ID, C.NOME_CIDADE, C.UF
                          FROM TBCIDADE C
                          WHERE C.ID = @ID", connection);
            
            command.Parameters.CreateParameter("@ID", cod);
            using FbDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapCidade(reader);
            }
            
            return null;
        }

        public List<Cidade> ObtenhaTodasCidades()
        {
            List<Cidade> cidades = new List<Cidade>();
            
            using FbConnection connection = _connection.CreateConnection();
            using FbCommand command = new FbCommand(
                @"SELECT C.ID, C.NOME_CIDADE, C.UF
                  FROM TBCIDADE C
                  ORDER BY C.NOME_CIDADE", connection);
            
            using FbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                cidades.Add(MapCidade(reader));
            }
            
            return cidades;
        }

        public List<Cidade> ObtenhaCidadePorEstado(int cod)
        {
            List<Cidade> cidades = new List<Cidade>();
            
            using FbConnection connection = _connection.CreateConnection();
            using FbCommand command = new FbCommand(
                @"SELECT C.ID, C.NOME_CIDADE, C.UF
                  FROM TBCIDADE C
                  WHERE C.UF = @UF
                  ORDER BY C.NOME_CIDADE", connection);
            
            command.Parameters.CreateParameter("@UF", cod);
            using FbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                cidades.Add(MapCidade(reader));
            }
            
            return cidades;
        }

        public Cidade AdicionarCidade(Cidade cidade)
        {
            using FbConnection connection = _connection.CreateConnection();
            
            using FbCommand command = new FbCommand(
                @"INSERT INTO TBCIDADE (NOME_CIDADE, UF)
                  VALUES (@NOME_CIDADE, @UF)
                  RETURNING ID", connection);
            
            command.Parameters.CreateParameter("@NOME_CIDADE", cidade.NomeDaCidade);
            command.Parameters.CreateParameter("@UF", (int)cidade.UF);
            
            object novoId = command.ExecuteScalar();
            cidade.Id = Convert.ToInt32(novoId);
            
            return cidade;
        }

        public Cidade AtualizarCidade(Cidade cidade)
        {
            using FbConnection connection = _connection.CreateConnection();
            using FbCommand command = new FbCommand(
                @"UPDATE TBCIDADE
                  SET NOME_CIDADE = @NOME_CIDADE, UF = @UF
                  WHERE ID = @ID", connection);
            
            command.Parameters.CreateParameter("@ID", cidade.Id);
            command.Parameters.CreateParameter("@NOME_CIDADE", cidade.NomeDaCidade);
            command.Parameters.CreateParameter("@UF", (int)cidade.UF);
            
            command.ExecuteNonQuery();
            
            return cidade;
        }

        public bool DeletarCidade(int id)
        {
            using FbConnection connection = _connection.CreateConnection();
            using FbCommand command = new FbCommand(
                @"DELETE FROM TBCIDADE
                  WHERE ID = @ID", connection);
            
            command.Parameters.CreateParameter("@ID", id);
            
            int rowsAffected = command.ExecuteNonQuery();
            
            return rowsAffected > 0;
        }
        
        private static Cidade MapCidade(FbDataReader reader)
        {
            return new Cidade(
                id: reader.GetInt32(0),
                nomeDaCidade: reader.GetString(1),
                uf: (UF)reader.GetInt32(2)
            );
        }
    }
}

