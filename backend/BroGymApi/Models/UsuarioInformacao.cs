using System.ComponentModel.DataAnnotations;

namespace BroGymApi.Models
{
    public class UsuarioInformacao
    {
        [Key]
        public int UsuarioInformacaoId { get; set; }

        // Fk (De lá pra cá);
        public int UsuarioId { get; set; }

        public int Genero { get; set; } // 0 Homem, 1 Mulher, 2 Outro;
        public DateTime DataAniversario { get; set; }
        public string? CPF { get; set; }
        public string? Telefone { get; set; }
        public string? Rua { get; set; }
        public string? NumeroResidencia { get; set; }
        public string? CEP { get; set; }
        public string? Bairro { get; set; }

        public DateTime? DataUltimaAlteracao { get; set; }
    }
}