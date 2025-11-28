using EM.Domain.Utilitarios;
using System.ComponentModel.DataAnnotations;

namespace EM.Domain
{
    public class Cidade
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O campo Nome da Cidade é obrigatório.")]
        public string NomeDaCidade { get; set; }
        
        [Required(ErrorMessage = "O campo UF é obrigatório.")]
        public UF UF { get; set; }
        
        public Cidade()
        {
            NomeDaCidade = string.Empty;
        }
        
        public Cidade(int id, string nomeDaCidade, UF uf)
        {
            Id = id;
            NomeDaCidade = nomeDaCidade;
            UF = uf;
        }
    }
}

