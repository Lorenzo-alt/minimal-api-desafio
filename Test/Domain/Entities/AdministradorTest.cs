using minimal_api_desafio.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class AdminstradorTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        // Arrange
        var adm = new Adm();

        //Act
        adm.Id = 2;
        adm.Email = "teste1@teste.com";
        adm.Senha = "123456";
        adm.Perfil = "Adm";

        //Assert
        Assert.AreEqual(2, adm.Id);
        Assert.AreEqual("teste1@teste.com", adm.Email);
        Assert.AreEqual("123456", adm.Senha);
        Assert.AreEqual("Adm", adm.Perfil);
    }
}