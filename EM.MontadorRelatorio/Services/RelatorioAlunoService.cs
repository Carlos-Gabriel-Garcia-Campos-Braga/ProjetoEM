using EM.Domain;
using EM.MontadorRelatorio.Interface;
using EM.MontadorRelatorio.Montadores;

namespace EM.MontadorRelatorio.Services
{
    public class RelatorioAlunoService : IRelatorioService
    {
        public byte[] GeraRelatorioPDFAlunos(IEnumerable<Aluno> alunos)
        {
            MontadorRelatorioAbstrato montador = new MontadorRelatorioPDF();

            montador
                .AdicionarTitulo("RELATÓRIO DE ALUNOS")
                .AdicionarDataHora()
                .AdicionarInformacao($"Total de Alunos: {alunos.Count()}");

            if (alunos.Any())
            {
                string[] cabecalhos = ["Matrícula", "Nome", "CPF", "Data Nasc.", "Sexo", "Cidade"];
                float[] largurasColunas = [12, 28, 18, 12, 12, 18];

                montador.AdicionarTabela(
                    dados: alunos.OrderBy(a => a.Nome),
                    cabecalhos: cabecalhos,
                    largurasColunas: largurasColunas,
                    obterLinhaDados: aluno =>
                    [
                        aluno.Matricula.ToString(),
                        aluno.Nome ?? string.Empty,
                        aluno.Cpf?.CpfFormatado ?? "-",
                        aluno.DataNascimento.HasValue ? aluno.DataNascimento.Value.ToString("dd/MM/yyyy") : "-",
                        aluno.Sexo.ToString(),
                        aluno.Cidade != null ? $"{aluno.Cidade.NomeDaCidade} - {aluno.Cidade.UF}" : "-"
                    ]);
            }
            else
            {
                montador.AdicionarMensagemSemDados("Nenhum aluno cadastrado no sistema.");
            }

            montador.AdicionarRodape("Sistema de Gerenciamento Escolar - ProjetoEM");

            return montador.Gera();
        }
    }
}

