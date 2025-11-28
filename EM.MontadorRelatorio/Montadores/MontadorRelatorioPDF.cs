using iTextSharp.text;
using iTextSharp.text.pdf;

namespace EM.MontadorRelatorio.Montadores
{
    public class MontadorRelatorioPDF : MontadorRelatorioAbstrato
    {
        private const float MARGEM_SUPERIOR = 36f;
        private const float MARGEM_INFERIOR = 36f;
        private const float MARGEM_ESQUERDA = 36f;
        private const float MARGEM_DIREITA = 36f;

        private const int TAMANHO_FONTE_TITULO = 16;
        private const int TAMANHO_FONTE_CABECALHO = 10;
        private const int TAMANHO_FONTE_NORMAL = 10;
        private const int TAMANHO_FONTE_RODAPE = 8;

        private static readonly BaseColor COR_CABECALHO_TABELA = new(238, 238, 238);

        private const float PADDING_CELULA = 5f;
        private const int PERCENTUAL_LARGURA_TABELA = 100;

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
            _document = new Document(PageSize.A4, MARGEM_SUPERIOR, MARGEM_INFERIOR, MARGEM_ESQUERDA, MARGEM_DIREITA);
            _writer = PdfWriter.GetInstance(_document, _stream);
            
            _fonteTitulo = new Font(Font.HELVETICA, TAMANHO_FONTE_TITULO, Font.BOLD);
            _fonteCabecalho = new Font(Font.HELVETICA, TAMANHO_FONTE_CABECALHO, Font.BOLD);
            _fonteNormal = new Font(Font.HELVETICA, TAMANHO_FONTE_NORMAL, Font.NORMAL);
            _fonteRodape = new Font(Font.HELVETICA, TAMANHO_FONTE_RODAPE, Font.NORMAL);
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
                WidthPercentage = PERCENTUAL_LARGURA_TABELA
            };
            tabela.SetWidths(largurasColunas);

            foreach (string cabecalho in cabecalhos)
            {
                AdicionarCelulaTabela(tabela, cabecalho, _fonteCabecalho, true);
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

        public override byte[] Gera()
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
                cell.BackgroundColor = COR_CABECALHO_TABELA;
            }
            cell.Padding = PADDING_CELULA;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            tabela.AddCell(cell);
        }
    }
}
