using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using BroGymApi.Interfaces;
using BroGymApi.Models;
using BroGymApi.Services;
using BroGymApi.Controllers;

namespace Usuarios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : BaseController<UsuariosController>
    {
        private readonly IUsuarioRepository _usuarios;

        public UsuariosController(IUsuarioRepository usuarioRepository)
        {
            _usuarios = usuarioRepository;
        }

        [HttpGet("autenticar")]
        public async Task<ActionResult<string>> Autenticar(string nomeUsuarioSistema, string senha)
        {
            // Verificar se o usuário existe;
            string caminho = String.Format("/api/Usuarios/verificarEmailSenha?nomeUsuarioSistema={0}&senha={1}", nomeUsuarioSistema, senha);
            var resultado = await GetAPI(caminho, null);
            Usuario? usu = JsonConvert.DeserializeObject<Usuario>(resultado);

            // Verifica se o usuário existe;
            if (usu == null)
            {
                return NotFound("Usuário ou senha inválidos");
            }

            // Gera o Token;
            var token = TokenService.ServicoGerarToken(usu.UsuarioId, usu.NomeUsuarioSistema, usu.UsuarioTipoId);

            return token;
        }

        [HttpGet("todos")]
        [Authorize]
        public async Task<ActionResult<List<Usuario>>> GetTodos()
        {
            var itens = await _usuarios.GetTodos();

            // Esconder alguns atributos;
            foreach (var item in itens)
            {
                item.Senha = "";
            }

            return itens;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetPorId(int id)
        {
            var item = await _usuarios.GetPorId(id);

            if (item == null)
            {
                return NotFound();
            }

            // Esconder alguns atributos;
            item.Senha = "";

            return item;
        }

        [HttpGet("verificarEmailSenha")]
        public async Task<ActionResult<Usuario>> GetVerificarEmailSenha(string nomeUsuarioSistema, string senha)
        {
            var usuarioBd = await _usuarios.GetVerificarEmailSenha(nomeUsuarioSistema, senha);

            if (usuarioBd != null)
            {
                Usuario usu = new()
                {
                    UsuarioId = usuarioBd.UsuarioId,
                    NomeCompleto = usuarioBd.NomeCompleto,
                    NomeUsuarioSistema = usuarioBd.NomeUsuarioSistema,
                    Email = usuarioBd.Email,
                    UsuarioTipoId = usuarioBd.UsuarioTipoId,
                    Foto = usuarioBd.Foto,
                    DataOnline = usuarioBd.DataOnline
                };

                return usu;
            }
            else
            {
                return null;
            }
        }
    }
}
