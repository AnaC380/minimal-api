# Minimal API - Gerenciamento de Administradores e Veículos

API REST desenvolvida em **ASP.NET Core Minimal API** utilizando **.NET 10**, **Entity Framework Core**, **SQL Server**, autenticação **JWT Bearer** e documentação automática com **Swagger/OpenAPI**.

O projeto implementa um sistema de gerenciamento de administradores e veículos, seguindo uma arquitetura organizada em camadas, separando domínio, infraestrutura, API e testes automatizados.

---

# Objetivos

Este projeto tem como objetivo demonstrar a construção de uma Minimal API moderna utilizando recursos atuais do ecossistema .NET, incluindo:

- ASP.NET Core Minimal API
- Entity Framework Core
- SQL Server
- JWT Authentication
- Swagger/OpenAPI
- Testes automatizados
- Organização por camadas
- Migrations
- DTOs
- Boas práticas de estruturação de projetos

---

# Tecnologias utilizadas

- .NET 10
- ASP.NET Core Minimal API
- Entity Framework Core
- SQL Server
- JWT Bearer Authentication
- BCrypt.Net
- OpenAPI / Swagger
- MSTest
- Git
- GitHub

---

# Estrutura do projeto

```
minimal-api
│
├── API
│   ├── Dominio
│   │   ├── DTOs
│   │   ├── Entidades
│   │   ├── Enums
│   │   ├── Interfaces
│   │   ├── ModelViews
│   │   └── Servicos
│   │
│   ├── Infraestrutura
│   │   ├── DB
│   │   └── OpenApi
│   │
│   ├── Migrations
│   ├── Properties
│   ├── Program.cs
│   ├── Startup.cs
│   └── minimal_api.csproj
│
├── Test
│   ├── Domain
│   ├── Helpers
│   ├── Mocks
│   ├── Requests
│   └── Test.csproj
│
└── minimal-api.slnx
```

---

# Arquitetura

O projeto foi organizado utilizando separação de responsabilidades.

## Domínio

Contém:

- Entidades
- DTOs
- Interfaces
- Serviços
- Enums
- ModelViews

Toda regra de negócio fica concentrada nesta camada.

---

## Infraestrutura

Responsável por:

- Entity Framework Core
- DBContext
- Configuração do OpenAPI
- Integração com SQL Server

---

## API

Responsável por:

- Configuração da aplicação
- Middleware
- Autenticação
- Endpoints
- Swagger
- Injeção de Dependência

---

## Testes

Projeto separado contendo:

- Testes de domínio
- Testes de serviços
- Testes de requisições
- Mocks
- Helpers

---

# Funcionalidades

## Administradores

- Login
- Cadastro
- Consulta
- Consulta por Id

---

## Veículos

- Cadastro
- Consulta
- Consulta por Id
- Atualização
- Exclusão

---

# Autenticação

A API utiliza autenticação baseada em **JWT Bearer Token**.

Fluxo:

```
Login
      ↓
JWT
      ↓
Authorization: Bearer {token}
      ↓
Endpoints protegidos
```

---

# Swagger

A documentação é gerada automaticamente pelo Swagger/OpenAPI.

Após executar a aplicação:

```
https://localhost:{porta}/swagger
```

---

# Banco de Dados

Banco utilizado:

```
SQL Server
```

A persistência é realizada através do Entity Framework Core utilizando Migrations.

---
# Configuração do Ambiente
Para rodar este projeto, certifique-se de ter o .NET SDK instalado em sua máquina.

1. Instalação de Ferramentas

O projeto utiliza o Entity Framework Core para o gerenciamento do banco de dados. Caso ainda não possua o EF Core CLI instalado globalmente, execute o comando abaixo:
```bash
dotnet tool install --global dotnet-ef
```
2. Configuração do Banco de Dados

Este projeto utiliza SQL Server LocalDB. Para criar o banco de dados e aplicar as tabelas, execute o comando na pasta raiz do projeto:
```bash
dotnet ef database update
```
Nota: Certifique-se de que a instância (localdb)\MSSQLLocalDB esteja ativa. Você pode verificar o status executando sqllocaldb info MSSQLLocalDB no seu terminal.

---
# Migrations

O projeto possui migrations versionadas.

Para atualizar o banco:

```bash
dotnet ef database update
```

Criar nova migration:

```bash
dotnet ef migrations add NomeMigration
```

---

# Executando localmente

Clone o repositório

```bash
git clone https://github.com/AnaC380/minimal-api.git
```

Entre na pasta

```bash
cd minimal-api
```

Restaure os pacotes

```bash
dotnet restore
```

Compile

```bash
dotnet build
```

Execute

```bash
dotnet run --project API/minimal_api.csproj
```

Abra o Swagger

```
https://localhost:{porta}/swagger
```

---

# Testes

Executar todos os testes

```bash
dotnet test
```

Build Release

```bash
dotnet build -c Release
```

Publicação

```bash
dotnet publish API/minimal_api.csproj \
-c Release \
-o ./publish
```

---

# Estrutura dos DTOs

O projeto utiliza DTOs para evitar exposição direta das entidades.

Exemplos:

- AdministradorDTO
- LoginDTO
- VeiculoDTO

---

# Segurança

A aplicação implementa:

- JWT Bearer Authentication
- Hash de senha utilizando BCrypt
- Endpoints protegidos
- Perfis de acesso
- Autorização baseada em Roles

---

# Organização

Durante a evolução do projeto foi realizada reorganização da estrutura para:

- separar API
- separar Testes
- melhorar namespaces
- padronizar interfaces
- centralizar infraestrutura
- facilitar manutenção

---

# Qualidade

Validações executadas antes da publicação:

- Build Release
- Publish Release
- Testes automatizados
- Swagger funcional
- Migrations
- Git versionado

---

# Publicação

A aplicação encontra-se preparada para publicação em serviços compatíveis com ASP.NET Core, como Microsoft Azure App Service.

---
---

# Solução de Problemas — Deploy no Azure (Swagger 404)

Durante a publicação no Azure App Service, o Swagger retornava **HTTP 404** em `/swagger/index.html`, mesmo após múltiplos redeploys. Veja a causa raiz e a correção.

## Causa 1 — Swagger restrito ao ambiente de desenvolvimento

Em `API/Startup.cs`, o mapeamento do OpenAPI e da UI do Swagger estava dentro de:

```csharp
if (env.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(...);
}
```

Como o Azure App Service roda em ambiente `Production` por padrão, esse bloco nunca era executado lá, e as rotas do Swagger simplesmente não existiam no servidor.

**Correção:** removida a condição `if (env.IsDevelopment())`, mantendo `MapOpenApi()` e `UseSwaggerUI()` ativos em qualquer ambiente.

## Causa 2 — Conflito de publicação (NETSDK1152)

Publicar a partir da raiz da solução (`dotnet publish -c Release -o ../../publish`) processava tanto o projeto `API` quanto o `Test`, gerando arquivos de saída duplicados (`appsettings.json`, `appsettings.Development.json`) e erro `NETSDK1152`.

**Correção:** publicar apenas o projeto da API:
```bash
dotnet publish api/minimal_api.csproj -c Release -o ./publish
```

## Causa 3 — Build antigo sendo reenviado por engano

Um comando anterior publicava em `../../publish` (a partir da pasta `api/`), o que na prática gravava em uma pasta **fora** do repositório (`AnaC380/publish`), enquanto o `Compress-Archive` lia de `minimal-api/publish` — uma pasta antiga, nunca atualizada. O zip enviado ao Azure sempre continha a build desatualizada.

**Correção:** publicar direto para `./publish` a partir da raiz do repositório, eliminando a divergência de caminhos.

## Verificação

Antes de cada deploy, validar o conteúdo do zip:
```bash
unzip -l projeto.zip | head -20
```
Os arquivos devem aparecer na raiz do zip (sem prefixo de subpasta), e `minimal_api.dll` deve ter timestamp recente.
# Licença

Este projeto foi desenvolvido para fins de estudo e demonstração de arquitetura utilizando ASP.NET Core Minimal API.
