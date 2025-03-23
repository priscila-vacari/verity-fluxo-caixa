# Projeto de Lançamentos e Conciliação Diária

## 📋 Visão Geral
Este projeto é uma aplicação robusta para **gerenciamento de lançamentos financeiros** e a **conciliação diária automatizada**. Ele fornece APIs seguras para a manipulação de dados de lançamento e processos diários automatizados, incluindo um serviço de **worker** que executa a conciliação periódica.

A aplicação é projetada para ser modular, escalável e segura, com recursos como:

- **Conciliação automatizada diária:** Processos agendados via Worker Service.

- **Lançamentos financeiros:** APIs para registrar, atualizar e consultar lançamentos.

- **Segurança integrada:** Proteções contra ataques de força bruta e validação rigorosa.

- **Escalabilidade:** Uso de uma arquitetura baseada em repositórios, serviços e factory pattern.

- **Logs detalhados:** Implementação de logs estruturados com Serilog e rastreamento através da correlação.

- **Padrões de Resiliência:** Proteção contra falhas e ataques, incluindo rate limiting.


## ✏️ Desenho da solução

[Veja aqui](https://github.com/priscila-vacari/verity-fluxo-caixa/blob/main/doc/desenho_tecnico.png)


## 🚀 Tecnologias Utilizadas
- **.NET 8:** Framework principal para a aplicação.

- **Entity Framework Core:** ORM para manipulação do banco de dados.

- **Serilog:** Sistema de logs estruturados.

- **JWT (JSON Web Token):** Autenticação e autorização. ❗

- **SQL Server:** Banco de dados relacional.

- **FluentValidation:** Validação de dados.

- **Rate Limiting:** Prevenção de ataques de força bruta.

- **BackgroundService:** Worker service para execução de tarefas agendadas.

## 🎯 Recursos
**1. Lançamentos Financeiros**
- Registra lançamentos financeiros com informações como data, valor e tipo do lançamento (crédito ou débito).

- Atualiza ou exclui lançamentos já existentes. ❗

- API segura com validação dos dados e autenticação via JWT. ❗
 
**2. Conciliação Diária**
- Worker service automatizado que executa a conciliação de dados financeiros diariamente.

- Possui logs detalhados de erros e sucessos.

- Configurável para rodar em intervalos diferentes (por padrão, executa a cada 1 hora).

**3. Segurança**
- Proteção contra ataques de força bruta usando rate limiting (restrição de requisições).

- Dados criptografados tanto em trânsito quanto em repouso.

- Tratamento robusto de erros e validações de entrada com FluentValidation.

## 📂 Estrutura do Projeto
Abaixo está a organização principal do projeto:
```
/src
  /FluxoCaixa.API
    /logs
        - log-api-YYYYMMDD.txt
    /controllers
        /v1
            - FinancialController.cs
  /FluxoCaixa.Application
    /interfaces
        - ILaunchService.cs
        - IConsolidationService.cs
        - IServiceFactory.cs
    /services
        - LaunchService.cs
        - ConsolidationService.cs
        - ServiceFactory.cs
    - ApplicationDependencyRegister.cs
  /FluxoCaixa.Domain
    /entities
        - Launch.cs
        - Consolidation.cs
  /FluxoCaixa.InfraEstructure
    /interfaces
        - IRepository.cs
    /repositories
        - IRepository.cs
        - Repository.cs
    - InfraDependencyRegister.cs
  /FluxoCaixa.Service
    /logs
        - log-svc-YYYYMMDD.txt
    /services
        - ConsolidationWorkerService.cs
    - ServiceDependencyRegister.cs
  /FluxoCaixa.Tests
    /api
        - FinancialControllerTests.cs
    /application
        - ConsolidatedServiceTests.cs
        - LaunchServiceTests.cs
    /infra
        - RepositoryTests.cs
    /service
        - ConsolidationWorkerServiceTests.cs
  /doc
    desenho_tecnico.png
    FluxoCaixa.postman_collection.json
FluxoCaixa.sln
readme.md
```

## 🔧 Configuração e Execução Local
Siga as etapas abaixo para configurar e executar a aplicação localmente:

**1. Pré-requisitos**

Certifique-se de que você possui os seguintes itens instalados:

- **SDK do .NET 8:** [Baixar aqui](https://dotnet.microsoft.com/pt-br/download)

- **SQL Server** (ou outro banco compatível configurado no `appsettings.json`).

- Um editor de texto, como **Visual Studio** ou **Visual Studio Code**.

**2. Clonar o Repositório**

Clone este repositório para sua máquina local:

```
bash
git clone https://github.com/priscila-vacari/verity-fluxo-caixa.git
cd verity-fluxo-caixa
```

**3. Configurar o Banco de Dados**

1. Certifique-se de que seu banco de dados SQL Server está configurado e rodando.

2. Atualize o arquivo `appsettings.json` com a string de conexão:
```
json
"ConnectionStrings": {
  "DefaultConnection": "Server=SEU_SERVIDOR;Database=SEU_BANCO;User Id=SEU_USUARIO;Password=SUA_SENHA;Encrypt=False;Pooling=true;"
}
```

3. Execute as migrações para preparar o banco de dados:

```
bash
dotnet ef database update --project FluxoCaixa.Infra --startup-project FluxoCaixa.API
```

**4. Rodar o Projeto**

1. Compile e execute a aplicação:
```
bash
dotnet build
dotnet run
```

2.  Acesse a aplicação em:

API: https://localhost:7056/api

Worker: Logs serão exibidos no terminal e no diretório `/logs`.

**5. Testar os Endpoints**

Use **Postman** ou **cURL** para testar os endpoints.

[collection postman](https://github.com/priscila-vacari/verity-fluxo-caixa/blob/main/doc/FluxoCaixa.postman_collection.json)

**Exemplo de Lançamentos**

- Rota: `POST /api/v1/financial/launch`
 
- Payload:
```
json
{
  "type": 0,
  "amount": 10.11,
  "date": "2025-03-20"
}
```

- Curl:
```
curl -X 'POST' \
  'https://localhost:7056/api/v1/Financial/launch' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "date": "2025-03-20",
  "type": 0,
  "amount": 10.11
}'
```

**Exemplo de Conciliação**

- Rota: `POST /api/v1/financial/consolidation/{date}`

- Resposta:
```
json
{
  "date": "2025-03-20T00:00:00",
  "totalCredit": 10.11,
  "totalDebit": 0,
  "balance": 10.11
}
```

- Curl:
```
curl -X 'POST' \
  'https://localhost:7056/api/v1/Financial/consolidation/2025-03-20' \
  -H 'accept: */*' \
  -d ''
```

**Relatório de Conciliação**

- Rota: `POST /api/v1/financial/consolidation`

- Resposta:
```
json
[
  {
    "date": "2025-03-20T00:00:00",
    "totalCredit": 10.11,
    "totalDebit": 0,
    "balance": 10.11
  },
  {
    "date": "2025-03-21T00:00:00",
    "totalCredit": 12,
    "totalDebit": 5,
    "balance": 7
  },
  {
    "date": "2025-03-22T00:00:00",
    "totalCredit": 125.78,
    "totalDebit": 71.24,
    "balance": 54.54
  }
]
```

- Curl:
```
curl -X 'POST' \
  'https://localhost:7056/api/v1/Financial/consolidation' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "dateStart": "2025-03-10",
  "dateEnd": "2025-03-11"
}'
```
## 🛡️ Boas Práticas e Padrões

**1. Logs Estruturados:**

- Todos os eventos importantes (erros, conciliações, lançamentos) são registrados nos arquivos de log.

- Automaticamente será salvo o `correlation_id` correspondente à requisição, a fim de rastrear o fluxo por diversas partes do sistema, facilitando a depuração e o monitoramento de problemas.

- Logs disponíveis em `/logs/`.

**2. Segurança:**

- Implementação de autenticação com JWT. ❗

- Proteção contra força bruta (rate limiting) aplicada globalmente.

**3. Validação:**

- Todas as entradas do usuário são validadas com o FluentValidation para garantir consistência e segurança.

**4. Configuração Dinâmica:**

- Parâmetros como a frequência da conciliação podem ser ajustados no `appsettings.json` do worker.

## 🧪 Testes

O projeto inclui testes unitários para todas as funcionalidades principais. Execute os testes com:

```
bash
dotnet test --collect:"XPlat Code Coverage"
reportgenerator "-reports:TestResults\**\*.cobertura.xml" "-targetdir:coveragereport" "-reporttypes:Html"
```

## 🛠️ Ferramentas Adicionais

**Swagger UI:**

- Acesse `https://localhost:7056/swagger/index.html` para explorar e testar as APIs interativamente.

**Serilog Dashboard (opcional):**

- Integre visualizações de logs com ferramentas como Seq ou Kibana para uma análise avançada.

## ❗️ Pendências

1. Implementar autenticação e autorização JWT (JSON Web Token).
2. Finalizar CRUD dos lançamentos (update, delete).
3. Finalizar CRUD da conciliação (update, delete).
4. Acertar correlation_id nos logs do Worker.
6. Adicionar mais validações de requests com datas nas rotas de launch.
8. Implementar containerização.
9. Criar um nuget package de conexão com banco de dados.
10. Aumentar cobertura de código.

## 📜 Licença

N/A.