using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Domain.Entidades;

[TestClass]
public class AdministradorTest
{
    [TestMethod]
    public void TestGetSetPropriedades()
    {
        // Arrange - Todas as variáveis que irá criar para fazer as validações
        var adm = new Administrador
        {
            Id = 1,
            Email = "teste@teste.com",
            Senha = "senha123",
            Perfil = Perfil.Adm
        };

        // Act - Ação que irá fazer para validar
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Senha = "teste"; 
        adm.Perfil = Perfil.Adm;

        // Assert - Validação do que foi feito
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("teste@teste.com", adm.Email);
        Assert.AreEqual("teste", adm.Senha);
        Assert.AreEqual(Perfil.Adm, adm.Perfil);
        Assert.AreEqual<string>("Adm", adm.Perfil.ToString());
    }
}
