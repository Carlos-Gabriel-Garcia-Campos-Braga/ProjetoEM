using ProjetoEM.Models;

namespace ProjetoEM.Interfaces;

public interface IAlunoRepository
{
    Aluno OtenhaAlunoPorMatricula(int matricula);

    List<Aluno> OtenhaAlunosPorSexo(int sexoIdentificador);

    List<Aluno> OtenhaAlunos();
    
    List<Aluno> OtenhaAlunosPorCidade(int cidadeIdentificador);
}