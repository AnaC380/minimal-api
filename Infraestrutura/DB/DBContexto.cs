using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;
using Microsoft.Extensions.Configuration;

namespace minimal_api.Infraestrutura.DB;

public class DBContexto : DbContext
{
    public DBContexto() { }

    public DBContexto(DbContextOptions<DBContexto> options) : base(options) { }

    public DbSet<Administrador> Administradores { get; set; } = default!;
    public DbSet<Veiculo> Veiculos { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador
            {
                Id = 1,
                Email = "Administrador@teste.com",
                Senha = "123456",
                Perfil = "Adm"
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

            var stringConexao = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(stringConexao);
        }
    }
}
