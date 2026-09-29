# 🏎️ F1 Telemetry API

Uma API RESTful desenvolvida em **C# .NET 8** para consolidar dados de telemetria da Fórmula 1, focada em demonstrar boas práticas de Arquitetura Limpa e uma **Pirâmide de Testes** rigorosa.

![CI Status](https://github.com/SEU-USUARIO/F1Telemetry/actions/workflows/ci.yml/badge.svg)

## O Desafio de Engenharia

O foco deste projeto não é apenas entregar dados, mas demonstrar domínio sobre **orquestração de dependências em testes automatizados**. 
Através do uso de **Testcontainers**, a suíte de testes de integração provisiona instâncias efêmeras do PostgreSQL via Docker, aplica as migrações via Entity Framework Core, executa as validações contra o banco de dados real e destrói os contêineres ao final da execução. Isso garante zero dependência de infraestrutura local e evita o uso de bancos em memória (que mascaram comportamentos do SQL real).

## Tecnologias e Padrões
* **Backend:** C# .NET 8, ASP.NET Core Web API
* **Persistência:** PostgreSQL, Entity Framework Core (Code-First)
* **Arquitetura:** Clean Architecture (Domain, Application, Infrastructure, API)
* **Testes de Unidade:** xUnit, Moq, FluentAssertions
* **Testes de Integração:** Testcontainers (PostgreSQL)
* **DevOps / Infra:** Docker, Docker Compose, GitHub Actions (CI)

## 🛠️ Como executar localmente

1. Clone o repositório:
```bash
   git clone [https://github.com/SEU-USUARIO/F1Telemetry.git](https://github.com/SEU-USUARIO/F1Telemetry.git)
```

2. Suba o banco de dados via Docker Compose:
```Bash
   docker compose up -d
```

3. Aplique as Migrations e rode a API:
```Bash
   dotnet ef database update --project F1Telemetry.Infrastructure --startup-project F1Telemetry.API
   dotnet run --project F1Telemetry.API
```

4. Acesse a documentação do Swagger em http://localhost:5000/swagger.

Como rodar os testes
Os testes subirão automaticamente um contêiner Docker descartável. Basta garantir que o Docker engine está rodando e executar:

```Bash
dotnet test
```

*(Não esqueça de trocar `SEU-USUARIO` pelo seu usuário do GitHub real no link do badge e do clone)*.
