using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api_desafio.Domain.Entities;
using minimal_api_desafio.Domain.Interfaces;
using minimal_api_desafio.Infra.Db;

namespace minimal_api_desafio.Domain.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly ProjContext _dbContext;
        public VeiculoService(ProjContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Apagar(Veiculo veiculo)
        {
            _dbContext.Veiculos.Remove(veiculo);
            _dbContext.SaveChanges();
        }

        public void Atualizar(Veiculo veiculo)
        {
            _dbContext.Veiculos.Update(veiculo);
            _dbContext.SaveChanges();
        }

        public Veiculo? BuscaPorId(int id)
        {
            return _dbContext.Veiculos.Where(x => x.Id == id).FirstOrDefault();
        }

        public void Incluir(Veiculo veiculo)
        {
            _dbContext.Veiculos.Add(veiculo);
            _dbContext.SaveChanges();
        }

        public List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null)
        {
            var query = _dbContext.Veiculos.AsQueryable();
            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(x => x.Nome.Contains(nome));
            }
            if (!string.IsNullOrEmpty(marca))
            {
                query = query.Where(x => x.Marca.Contains(marca));
            }

            int itemsPorPagina = 10;

            query = query.Skip((pagina - 1) * itemsPorPagina).Take(itemsPorPagina);

            return query.ToList();
        }
    }
}