using BroGymApi.Data;
using BroGymApi.Interfaces;
using BroGymApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BroGymApi.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        public readonly Context _context;

        public ProdutoRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<Produto>> GetTodos()
        {
            var itens = await _context.Produtos.
                OrderBy(n => n.Nome).AsNoTracking().ToListAsync();

            return itens;
        }

        public async Task<Produto> GetPorId(int id)
        {
            var item = await _context.Produtos.
                Where(p => p.ProdutoId == id).AsNoTracking().FirstOrDefaultAsync();

            return item;
        }

        public async Task<int> PostCriar(Produto Produto)
        {
            _context.Add(Produto);
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }

        public async Task<int> PostAtualizar(Produto Produto)
        {
            int isOk;

            try
            {
                _context.Update(Produto);
                isOk = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return isOk;
        }

        public async Task<int> PostDeletar(int id)
        {
            var dados = await _context.Produtos.FindAsync(id);

            if (dados == null)
            {
                throw new Exception("Registro com o id " + id + " não foi encontrado");
            }

            _context.Produtos.Remove(dados);
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }

        private async Task<bool> IsExiste(int id)
        {
            return await _context.Produtos.AnyAsync(p => p.ProdutoId == id);
        }
    }
}
