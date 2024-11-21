using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api_desafio.Domain.DTOs;
using minimal_api_desafio.Domain.Entities;
using minimal_api_desafio.Domain.Interfaces;
using minimal_api_desafio.Infra.Db;

namespace minimal_api_desafio.Domain.Services
{
    public class AdmService : IAdmService
    {
        private readonly ProjContext _dbContext;
        public AdmService(ProjContext dbContext){
            _dbContext = dbContext;
        }

        public Adm? BuscaPorId(int id)
        {
            return _dbContext.Adms.FirstOrDefault(ad => ad.Id == id);
        }

        public void Incluir(Adm adm)
        {
            _dbContext.Adms.Add(adm);
            _dbContext.SaveChanges();
        }

        public Adm? Login(LoginDTO loginDTO)
        {
            return _dbContext.Adms.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        }

        public List<Adm> Todos(int pagina = 1, string? email = null, string? perfil = null)
        {
            var query = _dbContext.Adms.AsQueryable();

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(x => x.Email.Contains(email));
            }
            if (!string.IsNullOrEmpty(perfil))
            {
                query = query.Where(x => x.Perfil.Contains(perfil));
            }

             int itemsPorPagina = 10;

            query = query.Skip((pagina - 1) * itemsPorPagina).Take(itemsPorPagina);

            return query.ToList();
        }
    }
}