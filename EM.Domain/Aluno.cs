using EM.Domain.Utilitarios;
using System.ComponentModel.DataAnnotations;

namespace EM.Domain
{
    public class Aluno
    {
        [Required(ErrorMessage = "O campo Matrícula é obrigatório.")]
        public int Matricula { get; set; }
        
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public string Nome { get; set; }
        
        public CPF? Cpf { get; set; }
        
        [Required(ErrorMessage = "O campo Data de Nascimento é obrigatório.")]
        public DateTime DataNascimento { get; set; }
        
        [Required(ErrorMessage = "O campo Sexo é obrigatório.")]
        public Sexo Sexo { get; set; }
        
        [Required(ErrorMessage = "O campo Cidade é obrigatório.")]
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

