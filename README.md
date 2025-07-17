# TaskManagementApp

Este projeto é uma API RESTful para um sistema de gerenciamento de tarefas. O objetivo é permitir que usuários organizem e monitorem suas tarefas diárias, bem como colaborem com colegas de equipe.

---

## Detalhes do App

* **Usuário**: Pessoa que utiliza o aplicativo detentor de uma conta.
* **Projeto**: Uma entidade que contém várias tarefas. Um usuário pode criar, visualizar e gerenciar vários projetos.
* **Tarefa**: Uma unidade de trabalho dentro de um projeto. Cada tarefa possui um título, uma descrição, uma data de vencimento e um status (pendente, em andamento, concluída).

---

### Funcionalidades Implementadas:

1.  **Listagem de Projetos**: Listar todos os projetos do usuário.
2.  **Visualização de Tarefas**: Visualizar todas as tarefas de um projeto específico.
3.  **Criação de Projetos**: Criar um novo projeto.
4.  **Criação de Tarefas**: Adicionar uma nova tarefa a um projeto.
5.  **Atualização de Tarefas**: Atualizar o status ou detalhes de uma tarefa.
6.  **Remoção de Tarefas**: Remover uma tarefa de um projeto.
7.  **Remoção de Projetos**: Remover um projeto.

### Principais tecnologias Utilizadas:

* **Linguagem**: C#
* **Framework**: ASP.NET Core 8
* **ORM**: Entity Framework Core
* **Banco de Dados**: SQL Server
* **Containerização**: Docker
* **Versionamento**: Git
* **Testes**: xUnit

### Como Executar o Projeto com Docker:

1.  **Pré-requisitos:**
    * Docker Desktop instalado e rodando.
    * .NET SDK 8.0 (opcional, para rodar fora do Docker ou para comandos dotnet).

2.  **Crie o arquivo de ambiente `.env`:**
    * Na raiz do projeto (`TaskManagementApp/`), crie um arquivo chamado `.env`.
    * Adicione o seguinte conteúdo a ele, substituindo `sua_senha_aqui` por uma senha forte e segura de sua escolha para o banco de dados:

    ```dotenv
    # Variáveis de ambiente para o serviço do banco de dados
    DB_SA_PASSWORD=sua_senha_aqui
    DB_PORT=1433

    # Variáveis de ambiente para o serviço da API
    API_DB_SERVER=db
    API_DB_NAME=TaskManagementDb
    API_DB_USER=sa
    API_DB_PASSWORD=sua_senha_aqui
    API_PORT_HTTP=8000
    API_PORT_HTTPS=8001
    ```
    * **IMPORTANTE:** Este arquivo `.env` já está configurado para ser ignorado pelo Git.

3.  **Navegue até a raiz do projeto:**
    ```bash
    cd TaskManagementApp
    ```

4.  **Construa as imagens Docker e inicie os serviços:**
    ```bash
    docker-compose up --build
    ```
    * Isso irá construir a imagem da sua API e iniciar o container do SQL Server, além do container da API.
    * A primeira execução pode demorar um pouco, pois o Docker fará o download das imagens base e construirá o projeto.

5.  **Acessar a API:**
    * Após a inicialização, a API estará acessível geralmente em `https://localhost:8001`.
    * A documentação Swagger estará disponível em `https://localhost:8001/swagger`.

6. **Usuário padrão**
    * Como o escopo da aplicação não contempla dados usuário, porém a aplicação nescessita de dados de usuário para operar, a aplicação gerar dados fake para dois usuário:
        * Usuario Padrao: Id - 00000000-0000-0000-0000-000000000001
        * Gerente Geral: Id - 00000000-0000-0000-0000-000000000002
    * Esses dados devem ser passados via header `X-User-Id`.

7.  **Parar os serviços:**
    ```bash
    docker-compose down
    ```

---

### Como Executar o projeto via linha de comando/VS Code:

1.  **Pré-requisitos:**
    * Docker Desktop instalado e rodando.
    * .NET SDK 8.0 (opcional, para rodar fora do Docker ou para comandos dotnet).

2.  **Crie o arquivo de settings `appsettings.Development.json`:**
    * Na raiz do projeto API (`TaskManagementApp/TaskManagementApp.Api/`), crie um arquivo chamado `appsettings.Development.json`.
    * Adicione o seguinte conteúdo, substituindo a senha `sua_senha_aqui` por uma senha forte e segura de sua escolha para o banco de dados:
    ```json
        {
            "Logging": {
            "LogLevel": {
                    "Default": "Information",
                    "Microsoft.AspNetCore": "Warning"
                }
            },
            "ConnectionStrings": {
                "DefaultConnection": "Server=localhost,1433;Database=TaskManagementDb;User ID=sa;Password=sua_senha_aqui;TrustServerCertificate=True;MultipleActiveResultSets=true;"
            }
        }
    ```

3. **Crie o container do banco de dados Sql Server:**
    * Garantir que o docker está em execução.
    * Executar o seguinte comando:
    
    ```bash
    docker run -d \
        --name taskmanagement_db_standalone \
        -p 1433:1433 \
        -e "SA_PASSWORD=sua_senha_aqui" \
        -e "ACCEPT_EULA=Y" \
        -v sqlserver_data:/var/opt/mssql \
        mcr.microsoft.com/mssql/server:2022-latest
    ``` 
    
    * Lembrando de substituir 'sua_senha_aqui' pela mesma senha configurada appsettings.Development.json

4. **Executar a migration do projeto:
    * Garante que o banco de dados está em funcionamento.
    * Na raiz do projeto (`TaskManagementApp/`) executar o comando via terminal:
    ```bash
    dotnet ef database update --project src/TaskManagementApp.Data/TaskManagementApp.Data.csproj --startup-project src/TaskManagementApp.Api/TaskManagementApp.Api.csproj
    ```

5. **Executar a aplicação**
    * No terminal dentro da raiz do projeto (`TaskManagementApp/`) execute o comando:
    ```bash
    dotnet run --project src/TaskManagementApp.Api
    ```

6. **Acessar a API:**
    * Após a inicialização, a API estará acessível geralmente em `http://localhost:5125`.
    * A documentação Swagger estará disponível em `http://localhost:5125/swagger`.

7. **Usuário padrão**
    * Como o escopo da aplicação não contempla dados usuário, porém a aplicação nescessita de dados de usuário para operar, a aplicação gerar dados fake para dois usuário:
        * Usuario Padrao: Id - 00000000-0000-0000-0000-000000000001
        * Gerente Geral: Id - 00000000-0000-0000-0000-000000000002
    * Esses dados devem ser passados via header `X-User-Id`.

---

## Refinamento próximas atividades

Listo abaixo as perguntas que seriam direcionadas ao PO visando o refinamento das funcionalidades existentes e o planejamento de novas implementações ou melhorias.

### 1. Autenticação e Autorização (Prioridade Alta)

* **1.1. Estratégia de Autenticação e Autorização:**
    * Como os usuários serão autenticados no sistema? (OAuth 2.0 / OpenID Connect, JWT, Azure AD B2C, Identity Server, provedor de identidade externo como Google/Microsoft?).
    * Como será feita a gestão de usuários (criação, edição, desativação) se a API atual não é responsável pelo CRUD de usuários? Haverá um sistema externo para isso?
    * Com base nisso, como o `UserId` será informado nas requisições para consumo interno da API (via Token JWT em um cabeçalho `Authorization` em vez do `X-User-Id` atual)?

* **1.2. Papéis e Permissões:**
    * Além de "Básico" e "Gerente", haverá outros papéis de usuário no futuro? Quais funcionalidades cada papel poderá acessar/modificar?
    * As permissões serão granulares ("pode editar sua própria tarefa", "pode ver tarefas de todos os projetos", "pode apagar qualquer tarefa") ou baseadas apenas em papéis?

### 2. Colaboração e Relacionamentos entre Entidades

* **2.1. Atribuição de Tarefas:**
    * Uma tarefa pode ser atribuída a múltiplos usuários? Atualmente é apenas um.
    * A atribuição de uma tarefa pode mudar ao longo do tempo? Se sim, a mudança de usuário também deve ser registrada no histórico da tarefa?
* **2.2. Projetos e Usuários:**
    * Um projeto pode ter múltiplos usuários associados que podem visualizar e/ou gerenciar suas tarefas?
    * Haverá times ou equipes que podem ser associados a projetos ou tarefas?
* **2.3. Hierarquia de Tarefas:**
    * Existe a necessidade de subtarefas ou de dependências entre tarefas (uma tarefa só pode começar/terminar depois de outra)?

### 3. Regras de Negócio Existentes

* **3.1. Prioridades de Tarefas:**
    * A prioridade é um campo estático (baixa, média, alta) ou poderá ser customizável no futuro?
    * Existe algum limite ou regra de negócio para a prioridade (x tarefas de alta prioridade por projeto)?
* **3.2. Restrições de Remoção de Projetos:**
    * A regra "não remover se houver tarefas pendentes" é rígida ou haveria cenários de exceção (força um delete que move as tarefas para outro projeto, ou as marca como canceladas)?
    * Será necessário um processo de "arquivar" projetos em vez de deletar permanentemente?
* **3.3. Limite de Tarefas por Projeto (20):**
    * Esse limite é fixo ou configurável por projeto/tipo de projeto?
    * Como lidar com concorrência para o limite de 20 tarefas? (Bloqueio otimista/pessimista para evitar que dois usuários adicionem a 20ª tarefa ao mesmo tempo). Isso foi uma melhoria identificada e pode ser validado aqui.
* **3.4. Histórico de Atualizações:**
    * Será necessário expor o histórico de alterações das tarefas via API, um endpoint de histórico? Se sim, quais dados precisamos expor e de que forma?
    * O que acontece com o histórico se a tarefa for deletada?
* **3.5. Comentários nas Tarefas:**
    * Os comentários devem ser visíveis para todos os usuários de um projeto ou apenas para o autor e gerentes?

### 4. Relatórios de Desempenho (Expansão)

* **4.1. Tipos de Relatórios:**
    * Além do número médio de tarefas concluídas por usuário, quais outros relatórios são importantes (tarefas em atraso, produtividade por projeto, distribuição de prioridades, gargalos)?
    * Os relatórios terão filtros adicionais (por período customizado, por usuário específico, por tipo de tarefa)?

### 5. Outras Funcionalidades Potenciais

* **5.1. Notificações:**
    * Haverá um sistema de notificações (ex: tarefa atribuída, prazo próximo, status alterado)? Como elas seriam entregues (email, push, in-app)?
* **5.2. Anexos:**
    * As tarefas precisarão de anexos (documentos, imagens)? Onde seriam armazenados?

---

## Melhorias e Visão Futura

### 1. Arquitetura e Padrões de Projeto

* **1.1. Padrão CQRS (Command Query Responsibility Segregation):**
    * Para melhorar a performance, especialmente com cargas de leitura e escrita desiguais, o padrão CQRS poderia ser usado no projeto. Teríamos comandos para operações que alteram o estado e queries para operações de leitura.
* **1.2. Design Orientado a Domínio (Domain-Driven Design - DDD):**
    * Aprofundar o uso de conceitos de DDD, como `Aggregate Roots` (ex: `Project` como Aggregate Root para suas `ProjectTasks`), `Value Objects` (para representar conceitos como `Deadline` de forma mais rica ou `TaskPriority` como um Value Object complexo, se necessário) e `Domain Events` (para publicar eventos como "TarefaConcluida" que outros serviços podem consumir, como o serviço de notificação ou histórico de forma mais reativa).

### 2. Infraestrutura e Cloud

* **2.1. Orquestração de Containers (Kubernetes / AKS / EKS):**
    * Para ambientes de produção, migrar do `docker-compose` para uma plataforma de orquestração de containers como Kubernetes (Azure Kubernetes Service - AKS). Isso forneceria escalabilidade automática, auto-recuperação, balanceamento de carga e gerenciamento de segredos mais robusto.
* **2.2. CI/CD Pipelines (Azure DevOps / GitHub Actions):**
    * Implementar pipelines de Continuous Integration (CI) e Continuous Deployment (CD) para automatizar o build, teste, empacotamento Docker e deploy da aplicação. Dado minha experiência com Azure DevOps, seria a minha escolha natural.
* **2.3. Gerenciamento de Segredos (Azure Key Vault):**
    * Eu moveria as credenciais do banco de dados (e outras informações sensíveis) de arquivos `.env` para um serviço de gerenciamento de segredos na nuvem, evitando expor diretamente no código fonte esses dados.
* **2.4. Monitoramento e Observabilidade:**
    * Implementar soluções robustas de monitoramento (como Application Insights, Grafana) para coletar métricas de performance, logs e tracing distribuído (OpenTelemetry) para identificar gargalos e diagnosticar problemas em produção.

### 3. Qualidade do Código e Manutenibilidade

* **3.1. Validação Robusta:**
    * Embora `DataAnnotations` seja funcional, a adoção de uma biblioteca como FluentValidation permitiria uma validação mais expressiva, customizável e testável, separando as regras de validação dos DTOs.
* **3.2. Testes Integração:**
    * Expandir a cobertura de testes para incluir testes de integração.