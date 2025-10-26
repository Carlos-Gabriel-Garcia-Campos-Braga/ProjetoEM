using ProjetoEM.Models;
namespace ProjetoEM.Interfaces;

public interface ICidadeRepository
{
    Cidade ObtenhaCidade(int cod);
    
    List<Cidade> ObtenhaCidadePorEstado(int cod);
}