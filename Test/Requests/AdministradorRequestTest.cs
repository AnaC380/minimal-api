using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;
using Test.Helpers;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.ModelView;
using minimal_api.Dominio.AdministradorLogado;


namespace Test.Requests;
public class LoginResponse
{
    public AdministradorLogado AdministradorLogado { get; set; } = default!;
}

[TestClass]
public class AdministradorRequestTest
{
    [ClassInitialize]

    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]

    public static void ClassCleanup(TestContext testContext)
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task TestGetSetPropriedades()
    {
        // Arrange - Todas as variáveis que irá criar para fazer as validações
        var loginDTO = new LoginDTO
        {
            Email = "adm@teste.com",
            Password = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");



        // Act - Ação que irá fazer para validar
        var response = await Setup.Client.PostAsync("/administradores/login", content);
        // Assert - Validação do que foi feito
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.IsNotNull(admLogado?.Email ?? "");
        Assert.IsNotNull(admLogado?.Perfil ?? "");
        Assert.IsNotNull(admLogado?.Token ?? "");
    }
}
