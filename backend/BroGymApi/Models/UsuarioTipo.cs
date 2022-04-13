﻿using System.ComponentModel.DataAnnotations;

namespace BroGymApi.Models
{
    public class UsuarioTipo
    {
        [Key]
        public int UsuarioTipoId { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }
        public int IsAtivo { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}