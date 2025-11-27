namespace EM.MontadorRelatorio.Montadores
{
    public abstract class MontadorRelatorioAbstrato
    {
        public abstract MontadorRelatorioAbstrato AdicionarTitulo(string titulo);
        public abstract MontadorRelatorioAbstrato AdicionarDataHora();
        public abstract MontadorRelatorioAbstrato AdicionarInformacao(string informacao);
        public abstract MontadorRelatorioAbstrato AdicionarTabela<T>(
            IEnumerable<T> dados,
            string[] cabecalhos,
            float[] largurasColunas,
            Func<T, string[]> obterLinhaDados);
        public abstract MontadorRelatorioAbstrato AdicionarMensagemSemDados(string mensagem);
        public abstract MontadorRelatorioAbstrato AdicionarRodape(string textoRodape);
        public abstract byte[] Gera();
    }
}

