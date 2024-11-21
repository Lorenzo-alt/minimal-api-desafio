using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api_desafio.Domain.Entities;

namespace minimal_api_desafio.Infra.Db
{
    public class ProjContext : DbContext
    {
        public ProjContext(DbContextOptions<ProjContext> options) : base(options) { }
        public DbSet<Adm> Adms { get; set; } = default!;
        public DbSet<Veiculo> Veiculos { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Adm>().HasData(new Adm
            {
                Id = 1,
                Email = "adm@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            });
        }
    }
}