using System.Net;
using System.Text;
using System.Text.Json;
using minimal_api_desafio.Domain.DTOs;
using minimal_api_desafio.Domain.Entities;
using minimal_api_desafio.Domain.ModelViews;
using Test.Helpers;

namespace Test.Requests;

[TestClass]
public class AdmRequestTest
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        Setup.ClassInit(context);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task TestarGetSetPropriedades()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            Email = "adm@teste.com",
            Senha = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        //Act
        var response = await Setup.client.PostAsync("/login", content);

        //Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var admLogado = JsonSerializer.Deserialize<AdmLogado>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(admLogado?.Email ?? "");
        Assert.IsNotNull(admLogado?.Perfil ?? "");
        Assert.IsNotNull(admLogado?.Token ?? "");

    }
}