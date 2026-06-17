using Microsoft.EntityFrameworkCore;
using minimal_api.Infraestrutura.DB;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;
using minimal_api.Dominio.ModelViews;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<iAdministradorServicos, AdministradorServicos>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

// Configuração do Banco de Dados
builder.Services.AddDbContext<DBContexto>(options => {
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Json(new Home()));

// Rota de Login corrigida
app.MapPost("/login", ([FromBody]LoginDTO loginDTO, iAdministradorServicos administradorServicos) =>
{
    if (administradorServicos.Login(loginDTO))
    
         return Results.Ok("Login com sucesso!");
    else
        return Results.Unauthorized();
});

app.Run();
