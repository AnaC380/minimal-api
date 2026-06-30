using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using minimal_api.Dominio.AdministradorLogado;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enums;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.ModelView;
using minimal_api.Dominio.Servicos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

public class Startup
{
    private string _key;

    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        _key = configuration.GetSection("Jwt:Key").Value
               ?? "ChaveSecretaSuperSeguraComPeloMenos32Caracteres";
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ConfigureServices
    // ─────────────────────────────────────────────────────────────────────────
    public void ConfigureServices(IServiceCollection services)
    {
        // Enums serializados como string no JSON ("Adm", "Editor", "User")
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // Autenticação JWT
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                                               Encoding.UTF8.GetBytes(_key))
            };
        });

        services.AddAuthorization();

        // Injeção de dependência
        services.AddDbContext<minimal_api.Infraestrutura.DB.DBContexto>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("UseSQLServer")));

        services.AddScoped<IAdministradorServicos, AdministradorServicos>();
        services.AddScoped<IVeiculoServicos, VeiculoServicos>();

        // OpenAPI nativo .NET 10 + transformer de segurança (classe declarada em arquivo próprio)
        services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer<SecurityRequirementsTransformer>();
            options.AddSchemaTransformer<IntParameterSchemaTransformer>();
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Configure — recebe WebApplication (Minimal API .NET 6+)
    // ─────────────────────────────────────────────────────────────────────────
    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        app.UseAuthentication();
        app.UseAuthorization();

            app.MapOpenApi();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "Minimal API v1");
                options.RoutePrefix = "swagger";
            });

        // Helper local: geração do Token JWT
        string GerarTokenJWT(Administrador administrador)
        {
            if (string.IsNullOrEmpty(_key)) return string.Empty;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, administrador.Email),
                new Claim("Perfil",         administrador.Perfil.ToString()),
                new Claim(ClaimTypes.Role,  administrador.Perfil.ToString())
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Helper local: validação de VeiculoDTO
        ErrosdeValidacao ValidarVeiculoDTO(VeiculoDTO veiculoDTO)
        {
            var validacao = new ErrosdeValidacao { Mensagens = new List<string>() };

            if (string.IsNullOrEmpty(veiculoDTO.Nome))
                validacao.Mensagens.Add("O nome do veículo é obrigatório.");

            if (string.IsNullOrEmpty(veiculoDTO.Marca))
                validacao.Mensagens.Add("A marca do veículo é obrigatória.");

            if (veiculoDTO.Ano < 1950)
                validacao.Mensagens.Add("Veículo muito antigo. Aceito apenas a partir de 1950.");

            return validacao;
        }

        #region Administradores

        // Login — público
        app.MapPost("/administradores/login",
            ([FromBody] LoginDTO loginDTO,
             IAdministradorServicos administradorServicos) =>
            {
                var administrador = administradorServicos.Login(loginDTO);

                if (administrador == null)
                    return Results.Unauthorized();


                string token = GerarTokenJWT(administrador);

                return Results.Ok(new
                {
                    AdministradorLogado = new AdministradorLogado
                    {
                        Email = administrador.Email,
                        Perfil = administrador.Perfil.ToString(),
                        Token = token
                    }
                });
            })
        .WithTags("Administradores");

        // Listagem — somente Adm
        app.MapGet("/administradores",
            ([FromQuery] int? pagina,
             IAdministradorServicos administradorServicos) =>
            {
                var lista = administradorServicos.Todos(pagina);

                var listModelView = lista.Select(adm => new AdministradorModelView
                {
                    Id = adm.Id,
                    Email = adm.Email,
                    Perfil = adm.Perfil.ToString()
                }).ToList();

                return Results.Ok(listModelView);
            })
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
        .WithTags("Administradores");

        // Cadastro — somente Adm
        app.MapPost("/administradores",
            ([FromBody] AdministradorDTO administradorDTO,
             IAdministradorServicos administradorServicos) =>
            {
                var validacao = new ErrosdeValidacao { Mensagens = new List<string>() };

                if (string.IsNullOrEmpty(administradorDTO.Email))
                    validacao.Mensagens.Add("O email do administrador é obrigatório.");

                if (string.IsNullOrEmpty(administradorDTO.Senha))
                    validacao.Mensagens.Add("A senha do administrador é obrigatória.");

                if (administradorDTO.Perfil == null)
                    validacao.Mensagens.Add("O perfil do administrador é obrigatório.");

                if (validacao.Mensagens.Count > 0)
                    return Results.BadRequest(validacao);

                var administrador = new Administrador
                {
                    Email = administradorDTO.Email,
                    Senha = BCrypt.Net.BCrypt.HashPassword(administradorDTO.Senha),
                    Perfil = administradorDTO.Perfil ?? Perfil.Adm
                };

                administradorServicos.Incluir(administrador);

                return Results.Created(
                    $"/administradores/{administrador.Id}",
                    new AdministradorModelView
                    {
                        Id = administrador.Id,
                        Email = administrador.Email,
                        Perfil = administrador.Perfil.ToString()
                    });
            })
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
        .WithTags("Administradores");

        // Busca por Id — somente Adm
        app.MapGet("/administradores/{id}",
            ([FromRoute] int id,
             IAdministradorServicos administradorServicos) =>
            {
                var administrador = administradorServicos.BuscaPorId(id);
                if (administrador == null) return Results.NotFound();

                return Results.Ok(new AdministradorModelView
                {
                    Id = administrador.Id,
                    Email = administrador.Email,
                    Perfil = administrador.Perfil.ToString()
                });
            })
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
        .WithTags("Administradores")
        .WithName("GetAdministradorPorId");

        #endregion

        #region Veículos

        // Cadastro — Adm ou Editor
        app.MapPost("/veiculos",
            ([FromBody] VeiculoDTO veiculoDTO,
             IVeiculoServicos veiculoServicos) =>
            {
                var validacao = ValidarVeiculoDTO(veiculoDTO);
                if (validacao.Mensagens.Count > 0) return Results.BadRequest(validacao);

                var veiculo = new Veiculo
                {
                    Nome = veiculoDTO.Nome,
                    Marca = veiculoDTO.Marca,
                    Ano = veiculoDTO.Ano
                };

                veiculoServicos.Incluir(veiculo);
                return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
            })
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
        .WithTags("Veículos");

        // Listagem — qualquer autenticado
        app.MapGet("/veiculos",
            ([FromQuery] int? pagina,
             IVeiculoServicos veiculoServicos) =>
            {
                var veiculos = veiculoServicos.Todos(pagina ?? 1);
                return Results.Ok(veiculos);
            })
        .RequireAuthorization()
        .WithTags("Veículos");

        // Busca por Id — qualquer autenticado
        app.MapGet("/veiculos/{id}",
            ([FromRoute] int id,
             IVeiculoServicos veiculoServicos) =>
            {
                var veiculo = veiculoServicos.BuscaPorId(new Veiculo { Id = id });
                if (veiculo == null) return Results.NotFound();
                return Results.Ok(veiculo);
            })
        .RequireAuthorization()
        .WithTags("Veículos")
        .WithName("GetVeiculoPorId");

        // Atualização — Adm ou Editor
        app.MapPut("/veiculos/{id}",
            ([FromRoute] int id,
             [FromBody] VeiculoDTO veiculoDTO,
             IVeiculoServicos veiculoServicos) =>
            {
                var veiculo = veiculoServicos.BuscaPorId(new Veiculo { Id = id });
                if (veiculo == null) return Results.NotFound();

                var validacao = ValidarVeiculoDTO(veiculoDTO);
                if (validacao.Mensagens.Count > 0) return Results.BadRequest(validacao);

                veiculo.Nome = veiculoDTO.Nome;
                veiculo.Marca = veiculoDTO.Marca;
                veiculo.Ano = veiculoDTO.Ano;

                veiculoServicos.Atualizar(veiculo);
                return Results.Ok(veiculo);
            })
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
        .WithTags("Veículos");

        // Exclusão — somente Adm
        app.MapDelete("/veiculos/{id}",
            ([FromRoute] int id,
             IVeiculoServicos veiculoServicos) =>
            {
                var veiculo = veiculoServicos.BuscaPorId(new Veiculo { Id = id });
                if (veiculo == null) return Results.NotFound();

                veiculoServicos.Apagar(veiculo);
                return Results.NoContent();
            })
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
        .WithTags("Veículos");

        #endregion
        // REMOVIDO: app.Run() daqui → já existe em Program.cs (top-level statements)
    }
}