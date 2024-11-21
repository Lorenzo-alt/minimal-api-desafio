using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api_desafio.Domain.Enuns;

namespace minimal_api_desafio.Domain.DTOs
{
    public class AdmistradorDTO
    {
        public string Email { get; set; } = default!;
        public string Senha { get; set; } = default!;
        public Perfil Perfil { get; set; } = default!;
    }
}