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