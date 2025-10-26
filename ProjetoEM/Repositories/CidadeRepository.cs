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
    
    private static Cidade MapCidade(FbDataReader reader)
    {
        return new Cidade(
            id: reader.GetInt32(0),
            nomeDaCidade: reader.GetString(1),
            uf: (UF)reader.GetInt32(2)
        );
    }
}