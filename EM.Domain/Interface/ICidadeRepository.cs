namespace EM.Domain.Interface
{
    public interface ICidadeRepository
    {
        Cidade? ObtenhaCidade(int cod);
        
        List<Cidade> ObtenhaTodasCidades();
        
        List<Cidade> ObtenhaCidadePorEstado(int cod);
        
        Cidade AdicionarCidade(Cidade cidade);
        
        Cidade AtualizarCidade(Cidade cidade);
        
        bool DeletarCidade(int id);
    }
}

