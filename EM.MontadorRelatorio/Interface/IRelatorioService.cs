using EM.Domain;

namespace EM.MontadorRelatorio.Interface
{
    public interface IRelatorioService
    {
        byte[] GeraRelatorioPDFAlunos(IEnumerable<Aluno> alunos);
    }
}

