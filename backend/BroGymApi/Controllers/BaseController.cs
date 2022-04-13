using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using BroGymApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using static Biblioteca.Biblioteca;

// Como criar um BaseController: https://stackoverflow.com/questions/58735503/creating-base-controller-for-asp-net-core-to-do-logging-but-something-is-wrong-w;
// Como fazer os metódos da BaseController não bugar a API ([NonAction]): https://stackoverflow.com/questions/35788911/500-error-when-setting-up-swagger-in-asp-net-core-mvc-6-app
// Ou então usar "protected";
namespace BroGymApi.Controllers
{
    public abstract class BaseController<T> : Controller
    {
        protected async Task<bool> IsUsuarioSolicitadoMesmoDoToken(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            if (token != null)
            {
                // var nomeUsuarioSistema = User.FindFirstValue(ClaimTypes.Name);          
                // var usuarioTipoid = User.FindFirstValue(ClaimTypes.Role);
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (usuarioId != id.ToString())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        protected static async Task<string> GetAPI(string caminho, string? token)
        {
            var resultado = null as dynamic;
            string urlApi = CaminhoAPI();

            // https://www.c-sharpcorner.com/article/consuming-asp-net-web-api-rest-service-in-asp-net-mvc-using-http-client/
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(urlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Autorização com token;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                }

                // GET;
                HttpResponseMessage res = await client.GetAsync(caminho);

                // Resposta;
                if (res.IsSuccessStatusCode)
                {
                    resultado = res.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    string erro = MensagemErroAPI(res, caminho);
                    throw new Exception(erro);
                }
            }

            return resultado;
        }

        protected static async Task<string> PostAPI(string caminho, dynamic? objeto, string? token)
        {
            var resultado = null as dynamic;
            string urlApi = CaminhoAPI();

            // https://www.c-sharpcorner.com/article/consuming-asp-net-web-api-rest-service-in-asp-net-mvc-using-http-client/
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(urlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Autorização com token;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                }

                // Post;
                var objetoConvertido = JsonConvert.SerializeObject(objeto);
                var objetoConvertidoFinal = new StringContent(objetoConvertido, UnicodeEncoding.UTF8, "application/json");
                HttpResponseMessage res = await client.PostAsync(caminho, objetoConvertidoFinal);

                // Resposta;
                if (res.IsSuccessStatusCode)
                {
                    resultado = res.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    string erro = MensagemErroAPI(res, caminho);
                    throw new Exception(erro);
                }
            }

            return resultado;
        }

        protected async Task<string> GerarToken(string? nomeUsuarioSistema, string? senha, ClaimsPrincipal? userClaim, HttpRequest? request, HttpResponse? response)
        {
            // Por via de regra, o método GerarToken deve receber um parâmetro preenchido e o outro nulo;
            // ("nomeUsuarioSistema" e "senha") preenchidos e "userClaim" nulo = caso #01;
            // ("nomeUsuarioSistema" e "senha") nulos e "userClaim" preenchido = caso #02 ou #03;

            // Verificação inicial (usuário deslogado);
            if (userClaim == null)
            {
                // Caso #01 - Usuário deslogado e está logando;
                // O idUsuario deve ser passado como parâmetro para que o token seja gerado;
                string? token = await Token(nomeUsuarioSistema, senha);

                // Salvar token no Cookies;
                SalvarCookies(response, token);

                return token;
            }
            else
            {
                // Nova verificação para ver se realmente o usuário está logado;
                if (userClaim.Identity.IsAuthenticated)
                {
                    string nomeUsuarioSistemaSemToken = userClaim.FindFirst(claim => claim.Type == ClaimTypes.UserData)?.Value;
                    string senhaUsuarioSemToken = userClaim.FindFirst(claim => claim.Type == ClaimTypes.Hash)?.Value;
                    string? tokenExistente = null as dynamic;

                    // Verificar se o usuário logado já tem token;
                    if (request != null)
                    {
                        if (request.Cookies.ContainsKey("X-Access-Token"))
                        {
                            tokenExistente = request.Cookies["X-Access-Token"];
                        }
                    }

                    if (!string.IsNullOrEmpty(tokenExistente))
                    {
                        bool isTokenValido = ValidarToken(tokenExistente);

                        // Verificar se o token ainda é valido (pelo tempo de expiração);
                        if (isTokenValido)
                        {
                            // Caso #02.1 - Se o usuário tiver token e estiver válido, retorne-o;
                            return tokenExistente;
                        }
                        else
                        {
                            // Caso #02.2 - Se o usuário tiver token e não estiver válido, retorne outro;
                            string? token = await Token(nomeUsuarioSistemaSemToken, senhaUsuarioSemToken);

                            // Salvar token no Cookies;
                            SalvarCookies(response, token);

                            return token;
                        }
                    }
                    else
                    {
                        // Caso #03 - Se o usuário estiver on-line e não tiver token, gere um novo;
                        string? token = await Token(nomeUsuarioSistemaSemToken, senhaUsuarioSemToken);

                        // Salvar token no Cookies;
                        SalvarCookies(response, token);

                        return token;
                    }
                }
                else
                {
                    // Caso #04 - Bug?
                    throw new Exception("Erro ao gerar token {GerarToken()}");
                }
            }
        }

        protected static async Task<string> Token(string nomeUsuarioSistema, string senha)
        {
            // Gerar o token usando o usuário;
            string caminho = String.Format("/api/UsuariosApi/autenticar/?nomeUsuarioSistema={0}&senha={1}", nomeUsuarioSistema, senha);
            string? tokenJson = await GetAPI(caminho, null);
            string? token = JsonConvert.DeserializeObject<string>(tokenJson);

            return token;
        }

        protected static bool ValidarToken(string token)
        {
            var chave = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Chave.chave));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = chave,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }

            return true;
        }

        protected static void SalvarCookies(HttpResponse? response, string token)
        {
            // Salvar o token em memória (in-memory);
            // Sobre ser safe, resposta de Big Pumpkin: https://stackoverflow.com/questions/27067251/where-to-store-jwt-in-browser-how-to-protect-against-csrf
            response.Cookies.Delete("fluxo_jwt");
            response.Cookies.Append("fluxo_jwt", token, new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
        }
    }
}
