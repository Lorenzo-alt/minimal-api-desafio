using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api_desafio.Domain.DTOs;
using minimal_api_desafio.Domain.Entities;

namespace minimal_api_desafio.Domain.Interfaces
{
    public interface IVeiculoService
    {
        List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null);
        Veiculo? BuscaPorId(int id);
        void Incluir(Veiculo veiculo);
        void Atualizar(Veiculo veiculo);
        void Apagar(Veiculo veiculo);
    }
}