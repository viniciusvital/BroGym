using BroGymApi.Models;
using static Biblioteca.Biblioteca;

namespace BroGymApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(Context context)
        {
            // Exclui o esquema, copia as queries, cria esquema/tabelas, popula o BD;
            bool resetarBd = false;
            if (resetarBd)
            {
                context.Database.EnsureDeleted(); // Excluir o esquema e as tabelas;
                // string sqlErro = context.Database.GenerateCreateScript(); // Query para criar as tabelas;
                context.Database.EnsureCreated(); // Recriar o esquema e as tabelas;

                Seed(context);
            }
        }

        public static void Seed(Context context)
        {
            // Hora atual;
            DateTime dataAgora = HorarioBrasilia();

            #region seed_usuarios
            if (!context.UsuariosTipos.Any())
            {
                context.UsuariosTipos.Add(new UsuarioTipo() { UsuarioTipoId = 1, Tipo = "Administrador", Descricao = "Administrador do sistema", IsAtivo = 1, DataCriacao = dataAgora });
                context.UsuariosTipos.Add(new UsuarioTipo() { UsuarioTipoId = 2, Tipo = "Usuário", Descricao = "Usuário comum", IsAtivo = 1, DataCriacao = dataAgora });
            }

            if (!context.Usuarios.Any())
            {
                context.Usuarios.Add(new Usuario() { UsuarioId = 1, NomeCompleto = "Adm do BroGym", Email = "adm@Hotmail.com", NomeUsuarioSistema = "adm", Senha = Criptografar("123"), DataCriacao = dataAgora, UsuarioTipoId = 1, Foto = "", IsAtivo = 1, IsVerificado = 1 });
                context.Usuarios.Add(new Usuario() { UsuarioId = 2, NomeCompleto = "Junior", Email = "junior@Hotmail.com", NomeUsuarioSistema = "junioranheu", Senha = Criptografar("123"), DataCriacao = dataAgora, UsuarioTipoId = 2, Foto = "", IsAtivo = 1, IsVerificado = 1 });
                context.Usuarios.Add(new Usuario() { UsuarioId = 3, NomeCompleto = "Vinícius Vital", Email = "vital@Hotmail.com", NomeUsuarioSistema = "usuario", Senha = Criptografar("123"), DataCriacao = dataAgora, UsuarioTipoId = 2, Foto = "", IsAtivo = 1, IsVerificado = 1 });
            }

            if (!context.UsuariosInformacoes.Any())
            {
                context.UsuariosInformacoes.Add(new UsuarioInformacao()
                {
                    UsuarioInformacaoId = 1,
                    UsuarioId = 2,
                    Genero = 1,
                    DataAniversario = dataAgora,
                    CPF = "44571955880",
                    Telefone = "12 98271-3939",
                    Rua = "José Benedito Ferrari",
                    NumeroResidencia = "480",
                    CEP = "12605-110",
                    Bairro = "Vila Passos",
                    DataUltimaAlteracao = null
                });
            }
            #endregion

            #region seed_produtos
            if (!context.Produtos.Any())
            {
                string descCamisaDosDeuses = @"Camisa dos deuses!!!";

                string descBarraProteina = @"Todas as barras de proteína naturais que você pode comprar em um pedido mensal simples e recorrente.<br/>
                                            • 25g de proteína por porção<br/>
                                            • Sem Soja e Sem Glúten<br/>
                                            • Sem cores, adoçantes ou sabores artificiais<br/>
                                            • Zero adição de açúcar";

                string descCamisaPreta = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam posuere lorem feugiat volutpat finibus. Sed non tellus in ex aliquet pulvinar sit amet quis elit.<br/>
                                            • 100% algodão orgânico<br/>
                                            • Cor preta sem estampa";

                string descCamisaBranca = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam posuere lorem feugiat volutpat finibus. Sed non tellus in ex aliquet pulvinar sit amet quis elit.<br/>
                                            • 100% algodão orgânico<br/>
                                            • Cor branca com estampa";

                string descProteina = @"Todo pó de proteína natural que você pode comprar em um pedido mensal simples e recorrente.<br/>
                                        • 25g de proteína por porção<br/>
                                        • Alimentado com capim, não OGM<br/>
                                        • Sou Livre e Sem Glúten<br/>
                                        • Processamento a frio<br/>
                                        • Sem cores, adoçantes ou sabores artificiais<br/>
                                        • Zero adição de açúcar";

                string descGarrafinha = @"Compre nosso pacote de variedades prensadas a frio.<br/>
                                        • Vegano<br/>
                                        • Sem glúten<br/>
                                        • Sem adição de açúcar";

                string descGiftCard = @"A compra deste cartão-presente digital cria um código exclusivo. O destinatário do cartão-presente pode inserir este código na finalização da compra para subtrair o valor do cartão-presente do total do pedido.<br/>
                                        Este vale-presente nunca expira.";

                context.Produtos.Add(new Produto() { ProdutoId = 1, Nome = "Camiseta com estampa dos deuses", Descricao = descCamisaDosDeuses, Preco = 89.90, Foto = "1.webp", IsAtivo = 1, DataRegistro = dataAgora });
                context.Produtos.Add(new Produto() { ProdutoId = 2, Nome = "Barra de proteína", Descricao = descBarraProteina, Preco = 2.90, Foto = "2.webp", IsAtivo = 1, DataRegistro = dataAgora });
                context.Produtos.Add(new Produto() { ProdutoId = 3, Nome = "Camiseta preta sem estampa", Descricao = descCamisaPreta, Preco = 24.90, Foto = "3.webp", IsAtivo = 1, DataRegistro = dataAgora });
                context.Produtos.Add(new Produto() { ProdutoId = 4, Nome = "Camiseta branca com estampa", Descricao = descCamisaBranca, Preco = 24.90, Foto = "4.webp", IsAtivo = 1, DataRegistro = dataAgora });
                context.Produtos.Add(new Produto() { ProdutoId = 5, Nome = "Proteína do soro do leite", Descricao = descProteina, Preco = 79.49, Foto = "5.webp", IsAtivo = 1, DataRegistro = dataAgora });
                context.Produtos.Add(new Produto() { ProdutoId = 6, Nome = "Garrafinha com suco zika", Descricao = descGarrafinha, Preco = 25.00, Foto = "6.webp", IsAtivo = 1, DataRegistro = dataAgora });
                context.Produtos.Add(new Produto() { ProdutoId = 7, Nome = "Gift card", Descricao = descGiftCard, Preco = 50.00, Foto = "7.webp", IsAtivo = 1, DataRegistro = dataAgora });
            }
            #endregion

            context.SaveChanges();
        }
    }
}



