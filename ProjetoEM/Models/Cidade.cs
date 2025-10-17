using ProjetoEM.Enums;

namespace ProjetoEM.Models
{
    public class Cidade(string nomeDaCidade, UF uf)
    {
        public string NomeDaCidade = nomeDaCidade;
        public UF UF = uf;
    }
}
