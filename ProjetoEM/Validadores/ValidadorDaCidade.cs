namespace ProjetoEM.Validadores
{
    public static class ValidadorDaCidade
    {
        public static bool NomeDaCidadeEhValido(string nomeDaCidade) => 
                                        nomeDaCidade.Length >= 3 && nomeDaCidade.Length <= 100;
    }
}
