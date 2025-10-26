using FirebirdSql.Data.FirebirdClient;
using ProjetoEM.DataBase;
using ProjetoEM.Enums;
using ProjetoEM.Interfaces;
using ProjetoEM.Models;
using ProjetoEM.ValueObjects;

namespace ProjetoEM.Repositories;

public class AlunoRepository(FireBirdConnection connection) : IAlunoRepository
{
    private FireBirdConnection _connection = connection;
    
    public Aluno OtenhaAlunoPorMatricula(int matricula)
    {
        using var connection = _connection.CreateConnection();
        using var command = new FbCommand(
            @"SELECT A.MATRICULA, A.NOME, A.SEXO, A.CPF,
                     A.CIDADE_ID, C.ID, C.NOME_CIDADE, C.UF
                     FROM TBALUNO A
                     LEFT JOIN TBCIDADE C ON A.CIDADE_ID = C.ID
                     WHERE A.MATRICULA = @MATRICULA", connection);
        
        command.Parameters.CreateParameter("@MATRICULA", matricula);
        
        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            return MapAluno(reader);
        }
        
        return null;
    }

    public List<Aluno> OtenhaAlunosPorSexo(int sexoIdentificador)
    {
        var alunos = new List<Aluno>();
        
        using var connection = _connection.CreateConnection();
        using var command = new FbCommand(
            @"SELECT A.MATRICULA, A.NOME, A.SEXO, A.CPF,
                     A.CIDADE_ID, C.ID, C.NOME_CIDADE, C.UF
                     FROM TBALUNO A
                     LEFT JOIN TBCIDADE C ON A.CIDADE_ID = C.ID
                     WHERE A.SEXO = @SEXO
                     ORDER BY A.NOME", connection);
        
        command.Parameters.CreateParameter("@SEXO", sexoIdentificador);
        
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            alunos.Add(MapAluno(reader));
        }
        
        return alunos;
    }

    public List<Aluno> OtenhaAlunos()
    {
        var alunos = new List<Aluno>();
        
        using var connection = _connection.CreateConnection();
        using var command = new FbCommand(
            @"SELECT A.MATRICULA, A.NOME, A.SEXO, A.CPF,
                     A.CIDADE_ID, C.ID, C.NOME_CIDADE, C.UF
                     FROM TBALUNO A
                     LEFT JOIN TBCIDADE C ON A.CIDADE_ID = C.ID
                     ORDER BY A.NOME", connection);
        
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            alunos.Add(MapAluno(reader));
        }
        
        return alunos;
    }

    public List<Aluno> OtenhaAlunosPorCidade(int cidadeIdentificador)
    {
        var alunos = new List<Aluno>();
        
        using var connection = _connection.CreateConnection();
        using var command = new FbCommand(
            @"SELECT A.MATRICULA, A.NOME, A.SEXO, A.CPF,
                     A.CIDADE_ID, C.ID, C.NOME_CIDADE, C.UF
                     FROM TBALUNO A
                     LEFT JOIN TBCIDADE C ON A.CIDADE_ID = C.ID
                     WHERE A.CIDADE_ID = @CIDADE_ID
                     ORDER BY A.NOME", connection);
        
        command.Parameters.CreateParameter("@CIDADE_ID", cidadeIdentificador);
        
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            alunos.Add(MapAluno(reader));
        }
        
        return alunos;
    }
    
    private static Aluno MapAluno(FbDataReader reader)
    {
        Aluno aluno = new Aluno
        {
            Matricula = reader.GetInt32(0),
            Nome = reader.GetString(1),
            Sexo = (Sexo)reader.GetInt32(2),
            Cpf = reader.IsDBNull(3) ? null : new CPF(reader.GetString(3)),
            CidadeId = reader.GetInt32(4)
        };

        if (!reader.IsDBNull(5))
        {
            aluno.Cidade = new Cidade(
                id: reader.GetInt32(5),
                nomeDaCidade: reader.GetString(6),
                uf: (UF)reader.GetInt32(7)
            );
        }

        return aluno;
    }
}