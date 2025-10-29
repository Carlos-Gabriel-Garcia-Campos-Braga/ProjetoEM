using ProjetoEM.Enums;
using ProjetoEM.ValueObjects;

namespace ProjetoEM.Models
{
    public class Aluno
    {
        public int Matricula { get; set; }
        public string Nome { get; set; }
        public CPF? Cpf { get; set; }
        public DateTime? DataNascimento { get; set; }
        public Sexo Sexo { get; set; }
        public int CidadeId { get; set; }
        public Cidade? Cidade { get; set; }

        public Aluno()
        {
            Nome = string.Empty;
        }

        public Aluno(string nome, int matricula, CPF cpf, Sexo sexo, Cidade cidade)
        {
            Nome = nome;
            Matricula = matricula;
            Cpf = cpf;
            Sexo = sexo;
            Cidade = cidade;
            CidadeId = cidade?.Id ?? 0;
        }

        public Aluno(int matricula, string nome, CPF? cpf, Sexo sexo, int cidadeId, Cidade? cidade = null)
        {
            Matricula = matricula;
            Nome = nome;
            Cpf = cpf;
            Sexo = sexo;
            CidadeId = cidadeId;
            Cidade = cidade;
        }
    }
}
