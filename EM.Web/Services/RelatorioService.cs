using EM.Domain;

namespace EM.Web.Services
{
    public class RelatorioService
    {
        public byte[] GerarRelatorioPDFAlunos(IEnumerable<Aluno> alunos)
        {
            using MemoryStream stream = new();
            iTextSharp.text.Document doc = new(iTextSharp.text.PageSize.A4, 36, 36, 36, 36);
            iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, stream);
            doc.Open();

            iTextSharp.text.Font fonteTitulo = new(iTextSharp.text.Font.HELVETICA, 16, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fonteCabecalho = new(iTextSharp.text.Font.HELVETICA, 10, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fonteNormal = new(iTextSharp.text.Font.HELVETICA, 10, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fonteRodape = new(iTextSharp.text.Font.HELVETICA, 8, iTextSharp.text.Font.NORMAL);

            iTextSharp.text.Paragraph paragrafoTitulo = new("RELATÓRIO DE ALUNOS", fonteTitulo)
            {
                Alignment = iTextSharp.text.Element.ALIGN_CENTER
            };
            doc.Add(paragrafoTitulo);
            doc.Add(new iTextSharp.text.Paragraph(" "));

            iTextSharp.text.Paragraph paragrafoData = new($"Data: {DateTime.Now:dd/MM/yyyy HH:mm}", fonteNormal)
            {
                Alignment = iTextSharp.text.Element.ALIGN_RIGHT
            };
            doc.Add(paragrafoData);
            doc.Add(new iTextSharp.text.Paragraph($"Total de Alunos: {alunos.Count()}", fonteNormal));
            doc.Add(new iTextSharp.text.Paragraph(" "));

            if (alunos.Any())
            {
                iTextSharp.text.pdf.PdfPTable tabela = new(6) { WidthPercentage = 100 };
                tabela.SetWidths(new float[] { 12, 28, 18, 12, 12, 18 });

                void AdicionarCelula(string texto, iTextSharp.text.Font fonte, bool ehCabecalho = false)
                {
                    iTextSharp.text.pdf.PdfPCell cell = new(new iTextSharp.text.Phrase(texto, fonte));
                    if (ehCabecalho)
                    {
                        cell.BackgroundColor = new iTextSharp.text.BaseColor(238, 238, 238);
                    }
                    cell.Padding = 5f;
                    cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    tabela.AddCell(cell);
                }

                void AdicionarCelulaTexto(string texto, iTextSharp.text.Font fonte, int alinhamento = iTextSharp.text.Element.ALIGN_LEFT)
                {
                    iTextSharp.text.pdf.PdfPCell cell = new(new iTextSharp.text.Phrase(texto, fonte))
                    {
                        Padding = 5f,
                        HorizontalAlignment = alinhamento
                    };
                    tabela.AddCell(cell);
                }

                iTextSharp.text.Font fonteHeader = new(
                    iTextSharp.text.Font.HELVETICA,
                    10,
                    iTextSharp.text.Font.BOLD,
                    new iTextSharp.text.BaseColor(0, 0, 0));

                AdicionarCelula("Matrícula", fonteHeader, true);
                AdicionarCelula("Nome", fonteHeader, true);
                AdicionarCelula("CPF", fonteHeader, true);
                AdicionarCelula("Data Nasc.", fonteHeader, true);
                AdicionarCelula("Sexo", fonteHeader, true);
                AdicionarCelula("Cidade", fonteHeader, true);

                foreach (Aluno aluno in alunos.OrderBy(a => a.Nome))
                {
                    string matricula = aluno.Matricula.ToString();
                    string nome = aluno.Nome ?? string.Empty;
                    string cpf = aluno.Cpf?.CpfFormatado ?? "-";
                    string dataNasc = aluno.DataNascimento.HasValue ? aluno.DataNascimento.Value.ToString("dd/MM/yyyy") : "-";
                    string sexo = aluno.Sexo.ToString();
                    string cidadeTexto = aluno.Cidade != null
                        ? $"{aluno.Cidade.NomeDaCidade} - {aluno.Cidade.UF}"
                        : "-";
                    
                    AdicionarCelulaTexto(matricula, fonteNormal, iTextSharp.text.Element.ALIGN_CENTER);
                    AdicionarCelulaTexto(nome, fonteNormal, iTextSharp.text.Element.ALIGN_CENTER);
                    AdicionarCelulaTexto(cpf, fonteNormal, iTextSharp.text.Element.ALIGN_CENTER);
                    AdicionarCelulaTexto(dataNasc, fonteNormal, iTextSharp.text.Element.ALIGN_CENTER);
                    AdicionarCelulaTexto(sexo, fonteNormal, iTextSharp.text.Element.ALIGN_CENTER);
                    AdicionarCelulaTexto(cidadeTexto, fonteNormal, iTextSharp.text.Element.ALIGN_CENTER);
                }

                doc.Add(tabela);
            }
            else
            {
                iTextSharp.text.Paragraph paragrafoSemDados = new("Nenhum aluno cadastrado no sistema.", fonteNormal)
                {
                    Alignment = iTextSharp.text.Element.ALIGN_CENTER
                };
                doc.Add(paragrafoSemDados);
            }

            doc.Add(new iTextSharp.text.Paragraph(" "));
            iTextSharp.text.Paragraph paragrafoRodape = new("Sistema de Gerenciamento Escolar - ProjetoEM", fonteRodape)
            {
                Alignment = iTextSharp.text.Element.ALIGN_CENTER
            };
            doc.Add(paragrafoRodape);

            doc.Close();
            writer.Close();

            return stream.ToArray();
        }
    }
}


