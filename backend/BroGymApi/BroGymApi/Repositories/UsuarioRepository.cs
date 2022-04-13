using Microsoft.EntityFrameworkCore;
using BroGymApi.Data;
using BroGymApi.Interfaces;
using BroGymApi.Models;
using static Biblioteca.Biblioteca;

namespace BroGymApi.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        public readonly Context _context;

        public UsuarioRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<Usuario>> GetTodos()
        {
            var itens = await _context.Usuarios.
                Include(ut => ut.UsuarioTipos).
                Include(ui => ui.UsuariosInformacoes).
                OrderBy(ui => ui.UsuarioId).AsNoTracking().ToListAsync();

            return itens;
        }

        public async Task<Usuario> GetPorId(int id)
        {
            var item = await _context.Usuarios.
                Include(ut => ut.UsuarioTipos).
                Include(ui => ui.UsuariosInformacoes).
                Where(ui => ui.UsuarioId == id).AsNoTracking().FirstOrDefaultAsync();

            return item;
        }

        public async Task<Usuario> GetVerificarEmailSenha(string nomeUsuarioSistema, string senha)
        {
            string senhaCriptografada = Criptografar(senha);

            var usuarioBd = await _context.Usuarios.
                Include(ui => ui.UsuariosInformacoes).
                AsNoTracking().
                FirstOrDefaultAsync(l => (l.NomeUsuarioSistema == nomeUsuarioSistema || l.Email == nomeUsuarioSistema) && l.Senha == senhaCriptografada);

            return usuarioBd;
        }
    }
}
