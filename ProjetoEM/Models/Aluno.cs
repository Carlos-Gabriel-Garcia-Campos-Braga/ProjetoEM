using ProjetoEM.Enums;
using ProjetoEM.ValueObjects;

namespace ProjetoEM.Models
{
    public class Aluno(string nome, int matricula, CPF cpf, Sexo sexo, Cidade cidade)
    {
        public string Nome = nome;
        public int Matricula = matricula;
        public CPF? Cpf = cpf;
        public Sexo Sexo = sexo;
        public Cidade Cidade = cidade;
    }
}
