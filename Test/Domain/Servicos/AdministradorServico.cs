using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enums;
using minimal_api.Infraestrutura.DB;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using minimal_api.Dominio.Servicos;

[assembly: DoNotParallelize]

namespace Test.Domain.Servicos;

[TestClass]
public class AdministradorServicoTest
{
    private DBContexto CriarContextoDeTeste()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        var optionsBuilder = new DbContextOptionsBuilder<DBContexto>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("UseSQLServer"));

        return new DBContexto(optionsBuilder.Options);
    }

    [TestMethod]
    public void TestandoSalvarEBuscarAdministrador()
    {
        // Arrange
        using var context = CriarContextoDeTeste();

        // Garante que o banco existe e limpa a tabela para o teste isolado
        context.Database.EnsureCreated();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Administrador
        {
            Email = "teste@teste.com",
            Senha = "senha123",
            Perfil = Perfil.Adm
        };

        var administradorServico = new AdministradorServicos(context);

        // Act
        administradorServico.Incluir(adm);

        // Como o ID é gerado no banco, buscamos pelo ID 1 (após o TRUNCATE)
        var admDoBanco = administradorServico.BuscaPorId(1);
        var todosAdministradores = administradorServico.Todos(1); // Supondo que o parâmetro seja a página

        // Assert
        // 1. Valida se o administrador buscado por ID realmente existe e mantém as propriedades
        Assert.IsNotNull(admDoBanco, "O administrador deveria ter sido encontrado por ID.");
        Assert.AreEqual("teste@teste.com", admDoBanco.Email);

        // 2. Valida a quantidade total de registros usando CollectionAssert
        CollectionAssert.AllItemsAreNotNull(todosAdministradores);
        Assert.HasCount(1, todosAdministradores, "A paginação deveria retornar exatamente 1 registro.");
    }
}