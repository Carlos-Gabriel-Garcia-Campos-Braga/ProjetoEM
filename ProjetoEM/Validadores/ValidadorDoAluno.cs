namespace ProjetoEM.Validadores
{
    public static class ValidadorDoAluno
    {
        public static bool NomeEhValido(string nome) => 
                                nome.Length >= 3 && nome.Length <= 100;
    }
}
