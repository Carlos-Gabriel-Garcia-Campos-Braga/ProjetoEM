namespace EM.Domain.Utilitarios
{
    public static class Validation
    {
        public static bool NomeEhValido(string nome) => 
            nome.Length >= 3 && nome.Length <= 100;

        public static bool NomeDaCidadeEhValido(string nomeDaCidade) => 
            nomeDaCidade.Length >= 3 && nomeDaCidade.Length <= 100;

        public static bool DataNascimentoEhValida(DateTime dataNascimento) => 
            dataNascimento.Date < DateTime.Now.Date;

        public static bool MatriculaEhValida(int matricula) => 
            matricula > 0;
    }
}

