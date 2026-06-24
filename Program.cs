using Microsoft.EntityFrameworkCore;
using minimal_api.Infraestrutura.DB;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;
using minimal_api.Dominio.ModelView;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using minimal_api.Dominio.AdministradorLogado;
using Microsoft.AspNetCore.Authorization;

#region Builder
var builder = WebApplication.CreateBuilder(args);

// Busca a chave no appsettings.json
const string defaultKey = "ChaveSecretaSuperSeguraComPeloMenos32Caracteres";
var key = builder.Configuration.GetValue<string>("Jwt:Key")
          ?? builder.Configuration.GetValue<string>("Jwt")
          ?? defaultKey;

// Configuração da Autenticação JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key))
    };
});

builder.Services.AddAuthorization();

// Injeção de Dependência
builder.Services.AddScoped<IAdministradorServicos, AdministradorServicos>();
builder.Services.AddScoped<IVeiculoServicos, VeiculoServicos>();

// Registra o OpenAPI com o transformer de segurança JWT
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<SecurityRequirementsTransformer>();
});

// Configuração do Banco de Dados usando a string que está no appsettings.json
builder.Services.AddDbContext<DBContexto>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("UseSQLServer");
    options.UseSqlServer(connectionString);
});

var app = builder.Build();
#endregion

#region Middlewares (Swagger / OpenAPI UI)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Gera o JSON nativo do .NET 10

    // Ativa apenas a interface visual clássica do Swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Minimal API v1");
        options.RoutePrefix = "swagger"; // Permite acessar via /swagger
    });
}
#endregion

#region Home
app.MapGet("/", () => Results.Ok(new Home()));
#endregion

#region Administradores
// Geração do Token JWT
string GerarTokenJWT(Administrador administrador)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, administrador.Email),
        new Claim("Perfil", administrador.Perfil.ToString()),
        new Claim(ClaimTypes.Role, administrador.Perfil.ToString())
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddDays(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

// Login — público, sem autenticação
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServicos administradorServicos) =>
{
    var administrador = administradorServicos.Login(loginDTO);

    if (administrador != null)
    {
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
    }

    return Results.Unauthorized();
})
.WithTags("Administradores");

// Listagem de administradores — só Adm
app.MapGet("/administradores", (int? pagina, IAdministradorServicos administradorServicos) =>
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

// Cadastro de administradores — só Adm
app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServicos administradorServicos) =>
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
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil ?? Perfil.Adm
    };

    administradorServicos.Incluir(administrador);

    return Results.Created($"/administradores/{administrador.Id}", new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil.ToString()
    });
})
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Administradores");

// Busca de administrador por Id — só Adm
app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServicos administradorServicos) =>
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
.WithTags("Administradores");
#endregion

#region Veiculos
ErrosdeValidacao ValidarVeiculoDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosdeValidacao { Mensagens = new List<string>() };

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
        validacao.Mensagens.Add("O nome do veículo é obrigatório.");

    if (string.IsNullOrEmpty(veiculoDTO.Marca))
        validacao.Mensagens.Add("A marca do veículo é obrigatória.");

    if (veiculoDTO.Ano < 1950)
        validacao.Mensagens.Add("Veículo muito antigo, aceito apenas veículos a partir de 1950.");

    return validacao;
}

// Cadastro de veículo — Adm ou Editor
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServicos veiculoServicos) =>
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

// Listagem de veículos — qualquer autenticado
app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServicos veiculoServicos) =>
{
    var veiculos = veiculoServicos.Todos(pagina ?? 1);
    return Results.Ok(veiculos);
})
.RequireAuthorization()
.WithTags("Veículos");

// Busca de veículo por Id — qualquer autenticado
app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServicos veiculoServicos) =>
{
    var veiculo = veiculoServicos.BuscaPorId(new Veiculo { Id = id });
    if (veiculo == null) return Results.NotFound();
    return Results.Ok(veiculo);
})
.RequireAuthorization()
.WithTags("Veículos");

// Atualização de veículo — Adm ou Editor
app.MapPut("/veiculos/{id}", ([FromRoute] int id, [FromBody] VeiculoDTO veiculoDTO, IVeiculoServicos veiculoServicos) =>
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

// Exclusão de veículo — só Adm
app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServicos veiculoServicos) =>
{
    var veiculo = veiculoServicos.BuscaPorId(new Veiculo { Id = id });
    if (veiculo == null) return Results.NotFound();

    veiculoServicos.Apagar(veiculo);
    return Results.NoContent();
})
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Veículos");
#endregion

#region App Activation
app.UseAuthentication();
app.UseAuthorization();

app.Run();

// Transformer responsável por injetar o esquema Bearer no Swagger/OpenAPI
public class SecurityRequirementsTransformer : Microsoft.AspNetCore.OpenApi.IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        Microsoft.OpenApi.OpenApiDocument document,
        Microsoft.AspNetCore.OpenApi.OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new Microsoft.OpenApi.OpenApiComponents();

        if (document.Components.SecuritySchemes == null)
            document.Components.SecuritySchemes =
                new Dictionary<string, Microsoft.OpenApi.IOpenApiSecurityScheme>();

        var scheme = new Microsoft.OpenApi.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.SecuritySchemeType.Http,
            In = Microsoft.OpenApi.ParameterLocation.Header,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            Description = "Insira o token JWT desta maneira: Bearer {seu token aqui}"
        };

        document.Components.SecuritySchemes["Bearer"] = scheme;

        var securityRequirementKey = new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document);

        var requirement = new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            { securityRequirementKey, new List<string>() }
        };

        document.Security ??= new List<Microsoft.OpenApi.OpenApiSecurityRequirement>();
        document.Security.Add(requirement);

        return Task.CompletedTask;
    }
}
#endregion