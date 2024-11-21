
using minimal_api_desafio.Domain.DTOs;
using minimal_api_desafio.Domain.Entities;
using minimal_api_desafio.Domain.Interfaces;

namespace Test.Mocks;

public class AdministradorServicoMock : IAdmService
{
    private static List<Adm> administradores = new List<Adm>(){
        new Adm{
            Id = 1,
            Email = "adm@teste.com",
            Senha = "123456",
            Perfil = "Adm"
        },
        new Adm{
            Id = 2,
            Email = "editor@teste.com",
            Senha = "123456",
            Perfil = "Editor"
        }
    };

    public Adm? BuscaPorId(int id)
    {
        return administradores.Find(a => a.Id == id);
    }

    public void Incluir(Adm administrador)
    {
        administrador.Id = administradores.Count() + 1;
        administradores.Add(administrador);
    }

    public Adm? Login(LoginDTO loginDTO)
    {
        return administradores.Find(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha);
    }

    public List<Adm> Todos(int? pagina)
    {
        return administradores;
    }

    public List<Adm> Todos(int pagina = 1, string? email = null, string? perfil = null)
    {
        throw new NotImplementedException();
    }
}