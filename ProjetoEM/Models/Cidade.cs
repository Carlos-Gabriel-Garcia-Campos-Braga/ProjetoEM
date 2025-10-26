using ProjetoEM.Enums;

namespace ProjetoEM.Models
{
    public class Cidade
    {
        public int Id { get; set; }
        public string NomeDaCidade { get; set; }
        public UF UF { get; set; }
        
        public Cidade(int id, string nomeDaCidade, UF uf)
        {
            Id = id;
            NomeDaCidade = nomeDaCidade;
            UF = uf;
        }
    }
}
