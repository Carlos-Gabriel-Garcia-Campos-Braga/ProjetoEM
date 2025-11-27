using EM.Domain;

namespace EM.Service.Interface
{
    public interface IRelatorioService
    {
        byte[] GerarRelatorioPDFAlunos(IEnumerable<Aluno> alunos);
    }
}

