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
    API_PORT=5001
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
    * Após a inicialização, a API estará acessível geralmente em `https://localhost:5001`.
    * A documentação Swagger estará disponível em `https://localhost:5001/swagger`.

6.  **Parar os serviços:**
    ```bash
    docker-compose down
    ```

---