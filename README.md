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

# Licença

Este projeto foi desenvolvido para fins de estudo e demonstração de arquitetura utilizando ASP.NET Core Minimal API.
