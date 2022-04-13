using BroGymApi.Models;

namespace BroGymApi.Interfaces
{
    public interface IProdutoRepository
    {
        Task<List<Produto>> GetTodos();
        Task<Produto> GetPorId(int id);
        Task<int> PostCriar(Produto produto);
        Task<int> PostAtualizar(Produto produto);
        Task<int> PostDeletar(int id);
    }
}
