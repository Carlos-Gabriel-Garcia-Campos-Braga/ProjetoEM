using FirebirdSql.Data.FirebirdClient;
using EM.Domain;
using EM.Domain.Interface;
using EM.Domain.Utilitarios;
using EM.Repository.Banco;

namespace EM.Repository
{
    public class RepositorioAluno(FireBirdConnection connection) : IAlunoRepository
    {
        private FireBirdConnection _connection = connection;
        
        public Aluno? OtenhaAlunoPorMatricula(int matricula)
        {
            using FbConnection connection = _connection.CreateConnection();
            using FbCommand command = new FbCommand(
                @"SELECT A.MATRICULA, A.NOME, A.SEXO, A.CPF, A.DATA_NASCIMENTO,
                         A.CIDADE_ID, C.ID, C.NOME_CIDADE, C.UF
                         FROM TBALUNO A
                         LEFT JOIN TBCIDADE C ON A.CIDADE_ID = C.ID
                         WHERE A.MATRICULA = @MATRICULA", connection);
            
            command.Parameters.CreateParameter("@MATRICULA", matricula);
            
            using FbDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapAluno(reader);
            }
            
            return null;
        }

        public Aluno? OtenhaAlunoPorCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
            {
                return null;
            }

            string cpfLimpo = CPF.RemoverFormatacao(cpf);
            
            if (string.IsNullOrWhiteSpace(cpfLimpo))
            {
                return null;
            }

            using FbConnection connection = _connection.CreateConnection();
            using FbCommand command = new FbCommand(
                @"SELECT A.MATRICULA, A.NOME, A.SEXO, A.CPF, A.DATA_NASCIMENTO,
                         A.CIDADE_ID, C.ID, C.NOME_CIDADE, C.UF
                         FROM TBALUNO A
                         LEFT JOIN TBCIDADE C ON A.CIDADE_ID = C.ID
                         WHERE TRIM(A.CPF) = TRIM(@CPF)", connection);
            
            command.Parameters.CreateParameter("@CPF", cpfLimpo);
            
            using FbDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapAluno(reader);
            }
            
            return null;
        }

        public List<Aluno> OtenhaAlunosPorSexo(int sexoIdentificador)
        {
            List<Aluno> alunos = new List<Aluno>();
            
            using FbConnection connection = _connection.CreateConnection();
            using FbCommand command = new FbCommand(
                @"SELECT A.MATRICULA, A.NOME, A.SEXO, A.CPF, A.DATA_NASCIMENTO,
                         A.CIDADE_ID, C.ID, C.NOME_CIDADE, C.UF
                         FROM TBALUNO A
                         LEFT JOIN TBCIDADE C ON A.CIDADE_ID = C.ID
                         WHERE A.SEXO = @SEXO
                         ORDER BY A.NOME", connection);
            
            command.Parameters.CreateParameter("@SEXO", sexoIdentificador);
            
            using FbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                alunos.Add(MapAluno(reader));
            }
            
            return alunos;
        }

        public List<Aluno> OtenhaAlunos()
        {
            List<Aluno> alunos = new List<Aluno>();
            
            using FbConnection connection = _connection.CreateConnection();
            using FbCommand command = new FbCommand(
                @"SELECT A.MATRICULA, A.NOME, A.SEXO, A.CPF, A.DATA_NASCIMENTO,
                         A.CIDADE_ID, C.ID, C.NOME_CIDADE, C.UF
                         FROM TBALUNO A
                         LEFT JOIN TBCIDADE C ON A.CIDADE_ID = C.ID
                         ORDER BY A.NOME", connection);
            
            using FbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                alunos.Add(MapAluno(reader));
            }
            
            return alunos;
        }

        public List<Aluno> OtenhaAlunosPorCidade(int cidadeIdentificador)
        {
            List<Aluno> alunos = new List<Aluno>();
            
            using FbConnection connection = _connection.CreateConnection();
            using FbCommand command = new FbCommand(
                @"SELECT A.MATRICULA, A.NOME, A.SEXO, A.CPF, A.DATA_NASCIMENTO,
                         A.CIDADE_ID, C.ID, C.NOME_CIDADE, C.UF
                         FROM TBALUNO A
                         LEFT JOIN TBCIDADE C ON A.CIDADE_ID = C.ID
                         WHERE A.CIDADE_ID = @CIDADE_ID
                         ORDER BY A.NOME", connection);
            
            command.Parameters.CreateParameter("@CIDADE_ID", cidadeIdentificador);
            
            using FbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                alunos.Add(MapAluno(reader));
            }
            
            return alunos;
        }
        
        public Aluno AdicionarAluno(Aluno aluno)
        {
            using FbConnection connection = _connection.CreateConnection();
            using FbCommand command = new FbCommand(
                @"INSERT INTO TBALUNO (MATRICULA, NOME, SEXO, CPF, DATA_NASCIMENTO,
                                       CIDADE_ID)
                          VALUES (@MATRICULA, @NOME, @SEXO, @CPF, @DATA_NASCIMENTO,
                                       @CIDADE_ID)", connection);
            
            command.Parameters.CreateParameter("@MATRICULA", aluno.Matricula);
            command.Parameters.CreateParameter("@NOME", aluno.Nome);
            command.Parameters.CreateParameter("@SEXO", (int)aluno.Sexo);
            command.Parameters.CreateParameter("@CPF", aluno.Cpf?.Value ?? (object)DBNull.Value);
            command.Parameters.CreateParameter("@DATA_NASCIMENTO", aluno.DataNascimento.HasValue ? aluno.DataNascimento.Value.Date : (object)DBNull.Value);
            command.Parameters.CreateParameter("@CIDADE_ID", aluno.CidadeId);
            
            command.ExecuteNonQuery();
            
            return aluno;
        }

        public Aluno AtualizarAluno(Aluno aluno)
        {
            using FbConnection connection = _connection.CreateConnection();
            using FbCommand command = new FbCommand(
                @"UPDATE TBALUNO
                          SET NOME = @NOME, 
                              SEXO = @SEXO, 
                              CPF = @CPF,
                              DATA_NASCIMENTO = @DATA_NASCIMENTO,
                              CIDADE_ID = @CIDADE_ID
                          WHERE MATRICULA = @MATRICULA", connection);
            
            command.Parameters.CreateParameter("@MATRICULA", aluno.Matricula);
            command.Parameters.CreateParameter("@NOME", aluno.Nome);
            command.Parameters.CreateParameter("@SEXO", (int)aluno.Sexo);
            command.Parameters.CreateParameter("@CPF", aluno.Cpf?.Value ?? (object)DBNull.Value);
            command.Parameters.CreateParameter("@DATA_NASCIMENTO", aluno.DataNascimento.HasValue ? aluno.DataNascimento.Value.Date : (object)DBNull.Value);
            command.Parameters.CreateParameter("@CIDADE_ID", aluno.CidadeId);
            
            command.ExecuteNonQuery();
            
            return aluno;
        }

        public bool DeletarAluno(int matricula)
        {
            using FbConnection connection = _connection.CreateConnection();
            using FbCommand command = new FbCommand(
                @"DELETE FROM TBALUNO
                          WHERE MATRICULA = @MATRICULA", connection);
            
            command.Parameters.CreateParameter("@MATRICULA", matricula);
            
            int rowsAffected = command.ExecuteNonQuery();
            
            return rowsAffected > 0;
        }
        
        private static Aluno MapAluno(FbDataReader reader)
        {
            Aluno aluno = new Aluno
            {
                Matricula = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Sexo = (Sexo)reader.GetInt32(2),
                Cpf = reader.IsDBNull(3) ? null : new CPF(reader.GetString(3)),
                DataNascimento = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                CidadeId = reader.GetInt32(5)
            };

            if (!reader.IsDBNull(6))
            {
                aluno.Cidade = new Cidade(
                    id: reader.GetInt32(6),
                    nomeDaCidade: reader.GetString(7),
                    uf: (UF)reader.GetInt32(8)
                );
            }

            return aluno;
        }
    }
}

