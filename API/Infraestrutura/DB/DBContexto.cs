using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;
using Microsoft.Extensions.Configuration;
using System;

namespace minimal_api.Infraestrutura.DB;

public class DBContexto : DbContext
{
    public DBContexto() { }

    public DBContexto(DbContextOptions<DBContexto> options) : base(options) { }

    public DbSet<Administrador> Administradores { get; set; } = default!;
    public DbSet<Veiculo> Veiculos { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Administrador>()
                    .Property(p => p.Perfil)
                    .HasConversion<string>();
        modelBuilder.Entity<Administrador>().HasData(
           new Administrador
           {
               Id = 1,
               Email = "Administrador@teste.com",
               Senha = "$2a$11$l7MzSKS4ovseZOv31aMz0elKKDpswork9ACnH.k6Bsx/5SFAYV7w2",
               Perfil = minimal_api.Dominio.Enums.Perfil.Adm
           }
       );
    } 

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var stringConexao = configuration.GetConnectionString("UseSQLServer");
            optionsBuilder.UseSqlServer(stringConexao);
        }
    }
}