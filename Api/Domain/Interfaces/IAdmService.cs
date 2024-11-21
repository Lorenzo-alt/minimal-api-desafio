using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api_desafio.Domain.DTOs;
using minimal_api_desafio.Domain.Entities;

namespace minimal_api_desafio.Domain.Interfaces
{
    public interface IAdmService
    {
        Adm? Login(LoginDTO loginDTO);
        void Incluir(Adm adm);
        List<Adm> Todos(int pagina = 1, string? email = null, string? perfil = null);
        Adm? BuscaPorId(int id);
    }
}