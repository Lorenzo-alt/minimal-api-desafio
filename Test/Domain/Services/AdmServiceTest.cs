using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api_desafio.Domain.Entities;
using minimal_api_desafio.Domain.Services;
using minimal_api_desafio.Infra.Db;

namespace Test.Domain.Entities;

[TestClass]
public class AdmServiceTest
{

  [TestMethod]
  public void TestarSaveAdm()
  {
    // Arrange
    var context = Utils.CriarContextoTeste();
    context.Database.ExecuteSqlRaw("TRUNCATE TABLE Adms");
    var adm = new Adm();
    adm.Email = "teste1@teste.com";
    adm.Senha = "123456";
    adm.Perfil = "Adm";
    var admService = new AdmService(context);

    //Act
    admService.Incluir(adm);

    //Assert
    Assert.AreEqual(1, admService.Todos().Count());
  }

  [TestMethod]
  public void TestarBuscaIdAdm()
  {
    // Arrange
    var context = Utils.CriarContextoTeste();
    context.Database.ExecuteSqlRaw("TRUNCATE TABLE Adms");
    var adm = new Adm();
    adm.Email = "teste1@teste.com";
    adm.Senha = "123456";
    adm.Perfil = "Adm";
    var admService = new AdmService(context);

    //Act
    admService.Incluir(adm);
    var admBusca = admService.BuscaPorId(adm.Id);

    //Assert
    Assert.AreEqual(1, admBusca.Id);
  }
}