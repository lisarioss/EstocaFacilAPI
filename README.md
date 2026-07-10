# 📦 EstocaFácil API

API REST desenvolvida em **C# e ASP.NET Core** para gerenciamento de estoque, permitindo o controle de produtos, movimentações e autenticação de usuários.

---

## Tecnologias Utilizadas

```
🔹 C#
🔹 ASP.NET Core
🔹 Entity Framework Core
🔹 PostgreSQL
🔹 JWT Authentication
🔹 AutoMapper
🔹 Swagger/OpenAPI
🔹 Docker

```

---

## Funcionalidades 

- Cadastro, edição e exclusão de produtos
- Controle de movimentações de estoque
- Registro de logs das operações
- Autenticação via JWT
- Documentação automática com Swagger
- Arquitetura em camadas (API, Application, Domain e Infrastructure)


---

## Arquitetura

```
EstocaFacil.API → Endpoints e configuração da API

EstocaFacil.Application → Regras de negócio

EstocaFacil.Domain → Entidades e contratos

EstocaFacil.Infrastructure → Persistência e serviços
```

---

## Como Executar

1. Clone o repositório
2. Configure a string de conexão com o PostgreSQL
3. Execute:

dotnet restore

dotnet ef database update

dotnet run

4. Acesse:

/swagger

para visualizar a documentação da API.

---

## Objetivo do Projeto

Este projeto foi desenvolvido com foco em aprendizado e prática de conceitos de desenvolvimento backend utilizando ASP.NET Core, autenticação JWT, Entity Framework Core e boas práticas de arquitetura.
 
---

## Licença

Este projeto está sob a licença MIT. Veja [LICENSE](LICENSE) para detalhes.

---

**Desenvolvedor:** [@lisarioss](https://github.com/lisarioss)

---

