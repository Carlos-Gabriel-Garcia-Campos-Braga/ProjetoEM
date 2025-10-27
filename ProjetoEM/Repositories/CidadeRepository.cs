using FirebirdSql.Data.FirebirdClient;
using ProjetoEM.DataBase;
using ProjetoEM.Interfaces;
using ProjetoEM.Models;
using ProjetoEM.Enums;

namespace ProjetoEM.Repositories;

public class CidadeRepository(FireBirdConnection connection) : ICidadeRepository
{
    private FireBirdConnection _connection = connection;
    
    public Cidade ObtenhaCidade(int cod)
    {
        using var connection = _connection.CreateConnection();
        using var command = new FbCommand(
            @"SELECT C.ID, C.NOME_CIDADE, C.UF
                      FROM TBCIDADE C
                      WHERE C.ID = @ID", connection);
        
        command.Parameters.CreateParameter("@ID", cod);
        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            return MapCidade(reader);
        }
        
        return null;
    }

    public List<Cidade> ObtenhaTodasCidades()
    {
        var cidades = new List<Cidade>();
        
        using var connection = _connection.CreateConnection();
        using var command = new FbCommand(
            @"SELECT C.ID, C.NOME_CIDADE, C.UF
              FROM TBCIDADE C
              ORDER BY C.NOME_CIDADE", connection);
        
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            cidades.Add(MapCidade(reader));
        }
        
        return cidades;
    }

    public List<Cidade> ObtenhaCidadePorEstado(int cod)
    {
        var cidades = new List<Cidade>();
        
        using var connection = _connection.CreateConnection();
        using var command = new FbCommand(
            @"SELECT C.ID, C.NOME_CIDADE, C.UF
              FROM TBCIDADE C
              WHERE C.UF = @UF
              ORDER BY C.NOME_CIDADE", connection);
        
        command.Parameters.CreateParameter("@UF", cod);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            cidades.Add(MapCidade(reader));
        }
        
        return cidades;
    }

    public Cidade AdicionarCidade(Cidade cidade)
    {
        using var connection = _connection.CreateConnection();
        
        using var command = new FbCommand(
            @"INSERT INTO TBCIDADE (NOME_CIDADE, UF)
              VALUES (@NOME_CIDADE, @UF)
              RETURNING ID", connection);
        
        command.Parameters.CreateParameter("@NOME_CIDADE", cidade.NomeDaCidade);
        command.Parameters.CreateParameter("@UF", (int)cidade.UF);
        
        var novoId = command.ExecuteScalar();
        cidade.Id = Convert.ToInt32(novoId);
        
        return cidade;
    }

    public Cidade AtualizarCidade(Cidade cidade)
    {
        using var connection = _connection.CreateConnection();
        using var command = new FbCommand(
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
        using var connection = _connection.CreateConnection();
        using var command = new FbCommand(
            @"DELETE FROM TBCIDADE
              WHERE ID = @ID", connection);
        
        command.Parameters.CreateParameter("@ID", id);
        
        var rowsAffected = command.ExecuteNonQuery();
        
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