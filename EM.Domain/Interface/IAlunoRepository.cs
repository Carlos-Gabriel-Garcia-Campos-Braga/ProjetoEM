using EM.Domain;

namespace EM.Domain.Interface
{
    public interface IAlunoRepository
    {
        Aluno? OtenhaAlunoPorMatricula(int matricula);

        Aluno? OtenhaAlunoPorCpf(string cpf);

        List<Aluno> OtenhaAlunosPorSexo(int sexoIdentificador);

        List<Aluno> OtenhaAlunos();
        
        List<Aluno> OtenhaAlunosPorCidade(int cidadeIdentificador);
        
        Aluno AdicionarAluno(Aluno aluno);
        
        Aluno AtualizarAluno(Aluno aluno);
        
        bool DeletarAluno(int matricula);
    }
}

