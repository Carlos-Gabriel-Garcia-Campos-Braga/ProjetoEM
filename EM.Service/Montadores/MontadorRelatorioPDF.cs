using iTextSharp.text;
using iTextSharp.text.pdf;

namespace EM.Service.Montadores
{
    public class MontadorRelatorioPDF : MontadorRelatorioAbstrato
    {
        private readonly Document _document;
        private readonly PdfWriter _writer;
        private readonly MemoryStream _stream;
        private readonly Font _fonteTitulo;
        private readonly Font _fonteCabecalho;
        private readonly Font _fonteNormal;
        private readonly Font _fonteRodape;

        public MontadorRelatorioPDF()
        {
            _stream = new MemoryStream();
            _document = new Document(PageSize.A4, 36, 36, 36, 36);
            _writer = PdfWriter.GetInstance(_document, _stream);
            
            _fonteTitulo = new Font(Font.HELVETICA, 16, Font.BOLD);
            _fonteCabecalho = new Font(Font.HELVETICA, 10, Font.BOLD);
            _fonteNormal = new Font(Font.HELVETICA, 10, Font.NORMAL);
            _fonteRodape = new Font(Font.HELVETICA, 8, Font.NORMAL);
        }

        public override MontadorRelatorioAbstrato AdicionarTitulo(string titulo)
        {
            if (!_document.IsOpen())
            {
                _document.Open();
            }

            Paragraph paragrafoTitulo = new(titulo, _fonteTitulo)
            {
                Alignment = Element.ALIGN_CENTER
            };
            _document.Add(paragrafoTitulo);
            _document.Add(new Paragraph(" "));
            
            return this;
        }

        public override MontadorRelatorioAbstrato AdicionarDataHora()
        {
            Paragraph paragrafoData = new($"Data: {DateTime.Now:dd/MM/yyyy HH:mm}", _fonteNormal)
            {
                Alignment = Element.ALIGN_RIGHT
            };
            _document.Add(paragrafoData);
            
            return this;
        }

        public override MontadorRelatorioAbstrato AdicionarInformacao(string informacao)
        {
            _document.Add(new Paragraph(informacao, _fonteNormal));
            _document.Add(new Paragraph(" "));
            
            return this;
        }

        public override MontadorRelatorioAbstrato AdicionarTabela<T>(
            IEnumerable<T> dados,
            string[] cabecalhos,
            float[] largurasColunas,
            Func<T, string[]> obterLinhaDados)
        {
            if (!dados.Any())
            {
                return this;
            }

            PdfPTable tabela = new(cabecalhos.Length)
            {
                WidthPercentage = 100
            };
            tabela.SetWidths(largurasColunas);

            Font fonteHeader = new(
                Font.HELVETICA,
                10,
                Font.BOLD,
                new BaseColor(0, 0, 0));

            foreach (string cabecalho in cabecalhos)
            {
                AdicionarCelulaTabela(tabela, cabecalho, fonteHeader, true);
            }

            foreach (T item in dados)
            {
                string[] valoresLinha = obterLinhaDados(item);
                foreach (string valor in valoresLinha)
                {
                    AdicionarCelulaTabela(tabela, valor, _fonteNormal, false);
                }
            }

            _document.Add(tabela);
            
            return this;
        }

        public override MontadorRelatorioAbstrato AdicionarMensagemSemDados(string mensagem)
        {
            Paragraph paragrafoSemDados = new(mensagem, _fonteNormal)
            {
                Alignment = Element.ALIGN_CENTER
            };
            _document.Add(paragrafoSemDados);
            
            return this;
        }

        public override MontadorRelatorioAbstrato AdicionarRodape(string textoRodape)
        {
            _document.Add(new Paragraph(" "));
            Paragraph paragrafoRodape = new(textoRodape, _fonteRodape)
            {
                Alignment = Element.ALIGN_CENTER
            };
            _document.Add(paragrafoRodape);
            
            return this;
        }

        public override byte[] Gerar()
        {
            if (_document.IsOpen())
            {
                _document.Close();
            }
            _writer.Close();

            return _stream.ToArray();
        }

        private static void AdicionarCelulaTabela(PdfPTable tabela, string texto, Font fonte, bool ehCabecalho)
        {
            PdfPCell cell = new(new Phrase(texto, fonte));
            if (ehCabecalho)
            {
                cell.BackgroundColor = new BaseColor(238, 238, 238);
            }
            cell.Padding = 5f;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            tabela.AddCell(cell);
        }
    }
}

