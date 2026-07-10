# 📦 EstocaFácil API

API REST desenvolvida com **ASP.NET Core** para gerenciamento de estoque, permitindo o controle de produtos, movimentações, autenticação de usuários e registro de operações.

## Tecnologias Utilizadas

- C#
- ASP.NET Core 10
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- Swagger / OpenAPI
- AutoMapper
- Docker
- Repository Pattern

## Funcionalidades

- Cadastro de usuários
- Autenticação segura com JWT
- Cadastro, consulta, atualização e exclusão de produtos
- Controle de entrada e saída de estoque
- Registro de movimentações
- Sistema de logs para auditoria
- Documentação automática com Swagger
- Seed de dados para ambiente de desenvolvimento

## Arquitetura

O projeto foi desenvolvido seguindo uma arquitetura em camadas para facilitar manutenção, escalabilidade e organização do código.

```text
EstocaFacil.API
│
├── Controllers
├── Configurações
└── Endpoints

EstocaFacil.Application
│
├── Serviços
├── DTOs
└── Regras de Negócio

EstocaFacil.Domain
│
├── Entidades
├── Interfaces
└── Contratos

EstocaFacil.Infrastructure
│
├── Entity Framework Core
├── Repositórios
├── Migrations
└── Persistência de Dados
```

## Autenticação

A API utiliza autenticação baseada em JWT (JSON Web Token).

Após realizar o login, o token gerado deve ser enviado no cabeçalho das requisições protegidas:

```http
Authorization: Bearer {seu_token}
```

## Configuração

Configure a string de conexão do PostgreSQL no arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=estocafacil;Username=SEU_USUARIO;Password=SUA_SENHA"
  }
}
```

## Como Executar

### Clonar o projeto

```bash
git clone https://github.com/lisarioss/EstocaFacilAPI.git
```

### Restaurar dependências

```bash
dotnet restore
```

### Aplicar as migrations

```bash
dotnet ef database update
```

### Executar a aplicação

```bash
dotnet run --project EstocaFacil.API
```

## Documentação da API

Após iniciar a aplicação, acesse:

```text
http://localhost:5113
```

ou

```text
https://localhost:xxxx
```

A documentação interativa estará disponível através do Swagger.

## Objetivo do Projeto

Este projeto foi desenvolvido para aprimorar conhecimentos em desenvolvimento backend utilizando ASP.NET Core, Entity Framework Core, PostgreSQL, autenticação JWT e boas práticas de arquitetura de software.

## Licença

Este projeto está licenciado sob a licença MIT.

## Desenvolvedora

**Lisa Rios**

GitHub: https://github.com/lisarioss

