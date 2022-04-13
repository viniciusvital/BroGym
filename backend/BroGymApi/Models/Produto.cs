using System.ComponentModel.DataAnnotations;

namespace BroGymApi.Models
{
    public class Produto
    {
        [Key]
        public int ProdutoId { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public double Preco { get; set; }
        public string? Foto { get; set; }
        public int IsAtivo { get; set; }
        public DateTime DataRegistro { get; set; }
    }
}
