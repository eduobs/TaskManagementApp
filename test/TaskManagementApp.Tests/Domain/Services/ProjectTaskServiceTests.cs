using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Enums;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Domain.Services;

namespace TaskManagementApp.Tests.Domain.Services
{
    public class ProjectTaskServiceTests
    {
        private readonly Mock<ILogger<ProjectTaskService>> _mockLogger;
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IProjectTaskRepository> _mockProjectTaskRepository;
        private readonly Mock<IProjectTaskHistoryRepository> _mockProjectTaskHistoryRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly ProjectTaskService _projectTaskService;
        private readonly User _testAssignedUser;
        private readonly User _testModifyingUser;

        public ProjectTaskServiceTests()
        {
            _mockProjectRepository = new Mock<IProjectRepository>();
            _mockProjectTaskRepository = new Mock<IProjectTaskRepository>();
            _mockLogger = new Mock<ILogger<ProjectTaskService>>();
            _mockProjectTaskHistoryRepository = new Mock<IProjectTaskHistoryRepository>();
            _mockUserRepository = new Mock<IUserRepository>();

            _testAssignedUser = new User("Usuário responsável", UserRole.Basic);
            _testAssignedUser.GetType().GetProperty("Id")?.SetValue(_testAssignedUser, 10);
            _testAssignedUser.GetType().GetProperty("ExternalId")?.SetValue(_testAssignedUser, Guid.NewGuid());

            _testModifyingUser = new User("Novo usuário", UserRole.Manager);
            _testModifyingUser.GetType().GetProperty("Id")?.SetValue(_testModifyingUser, 20);
            _testModifyingUser.GetType().GetProperty("ExternalId")?.SetValue(_testModifyingUser, Guid.NewGuid());

            _projectTaskService = new ProjectTaskService(
                _mockLogger.Object,
                _mockProjectRepository.Object,
                _mockProjectTaskRepository.Object,
                _mockProjectTaskHistoryRepository.Object,
                _mockUserRepository.Object
            );
        }

        [Fact(DisplayName = @"DADO uma requisição de criação de tarefa válida
                            QUANDO o projeto existir 
                            E limite de tarefas não for atingido
                            ENTÃO deve criar a tarefa com sucesso")]
        public async Task CreateProjectTaskAsync_DadosValidosENaoAtingiuLimite_DeveCriarTarefa()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();
            var projectIdInterno = 1;
            var projectName = "Projeto 1";
            var projectDescription = "Descrição do projeto.";

            var project = new Project(projectName, projectDescription, 1);

            project.GetType().GetProperty("Id")?.SetValue(project, projectIdInterno);
            project.GetType().GetProperty("ExternalId")?.SetValue(project, projectExternalId);

            var title = "Nova Tarefa";
            var description = "Descrição da tarefa.";
            var deadline = DateTime.Today.AddDays(10);
            var priority = ProjectTaskPriority.High;

            _mockProjectRepository
                .Setup(r => r.GetByIdAsync(projectExternalId))
                .ReturnsAsync(project);

            _mockUserRepository
                .Setup(r => r.GetByExternalIdAsync(_testAssignedUser.ExternalId))
                .ReturnsAsync(_testAssignedUser);
            
            _mockProjectTaskRepository
                .Setup(r => r.CountTasksByProjectIdAsync(projectIdInterno))
                .ReturnsAsync(5);

            _mockProjectTaskRepository
                .Setup(r => r.AddAsync(It.IsAny<ProjectTask>()))
                .Returns(Task.CompletedTask);

            _mockProjectTaskRepository
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _projectTaskService.CreateProjectTaskAsync(
                projectExternalId,
                title,
                description,
                deadline,
                priority,
                _testAssignedUser.ExternalId
            );

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be(title);
            result.Description.Should().Be(description);
            result.Deadline.Should().Be(deadline);
            result.Priority.Should().Be(priority);
            result.Status.Should().Be(ProjectTaskStatus.Pending);
            result.ProjectId.Should().Be(projectIdInterno);
            result.AssignedToUserId.Should().Be(_testAssignedUser.Id);
            result.ExternalId.Should().NotBe(Guid.Empty);

            _mockProjectRepository.Verify(r => r.GetByIdAsync(projectExternalId), Times.Once());
            _mockUserRepository.Verify(r => r.GetByExternalIdAsync(_testAssignedUser.ExternalId), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.CountTasksByProjectIdAsync(projectIdInterno), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.AddAsync(It.Is<ProjectTask>(pt =>
                pt.Title == title &&
                pt.Description == description &&
                pt.Deadline == deadline &&
                pt.Priority == priority &&
                pt.ProjectId == projectIdInterno &&
                pt.AssignedToUserId == _testAssignedUser.Id
            )), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once());
        }

        [Fact(DisplayName = @"DADO uma requisição de criação de tarefa
                            QUANDO o projeto pai não for encontrado
                            ENTÃO deve lançar ArgumentException")]
        public async Task CreateProjectTaskAsync_ProjetoNaoEncontrado_DeveLancarArgumentException()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();
            var userId = _testAssignedUser.ExternalId;

            _mockProjectRepository
                .Setup(r => r.GetByIdAsync(projectExternalId))
                .ReturnsAsync((Project?)null);

            // Act
            Func<Task> act = async () => await _projectTaskService.CreateProjectTaskAsync(
                projectExternalId,
                "Título", "Descrição",
                DateTime.Today.AddDays(1),
                ProjectTaskPriority.Low,
                userId
            );

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                       .WithMessage($"Projeto com ID {projectExternalId} não encontrado.");

            _mockProjectRepository.Verify(r => r.GetByIdAsync(projectExternalId), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.CountTasksByProjectIdAsync(It.IsAny<int>()), Times.Never());
            _mockProjectTaskRepository.Verify(r => r.AddAsync(It.IsAny<ProjectTask>()), Times.Never());
            _mockProjectTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never());
        }

        [Fact(DisplayName = @"DADO uma requisição
                            QUANDO o usuário atribuído não for encontrado
                            ENTÃO deve lançar uma ArgumentException")]
        public async Task CreateProjectTaskAsync_AssignedUserNaoEncontrado_DeveLancarArgumentException()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();
            var projectIdInterno = 1;
            var project = new Project("Projeto Teste", "Desc", 1);
            project.GetType().GetProperty("Id")?.SetValue(project, projectIdInterno);
            project.GetType().GetProperty("ExternalId")?.SetValue(project, projectExternalId);

            var invalidAssignedUserId = Guid.NewGuid();

            _mockProjectRepository.Setup(r => r.GetByIdAsync(projectExternalId)).ReturnsAsync(project);
            _mockUserRepository.Setup(r => r.GetByExternalIdAsync(invalidAssignedUserId)).ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _projectTaskService.CreateProjectTaskAsync(
                projectExternalId, "Titulo", "Desc", DateTime.Today.AddDays(1), ProjectTaskPriority.Low, invalidAssignedUserId);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                       .WithMessage($"Usuário com ID {invalidAssignedUserId} não encontrado para atribuição da tarefa.");

            _mockProjectRepository.Verify(r => r.GetByIdAsync(projectExternalId), Times.Once());
            _mockUserRepository.Verify(r => r.GetByExternalIdAsync(invalidAssignedUserId), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.CountTasksByProjectIdAsync(It.IsAny<int>()), Times.Never());
            _mockProjectTaskRepository.Verify(r => r.AddAsync(It.IsAny<ProjectTask>()), Times.Never());
        }

        [Fact(DisplayName = @"DADO uma requisição de criação de tarefa
                            QUANDO o limite de 20 tarefas for atingido
                            ENTÃO deve lançar uma InvalidOperationException")]
        public async Task CreateProjectTaskAsync_LimiteDeTarefasAtingido_DeveLancarInvalidOperationException()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();
            var projectIdInterno = 2;
            var projectName = "Projeto Cheio";
            var project = new Project(projectName, "Descrição", 1);
            project.GetType().GetProperty("Id")?.SetValue(project, projectIdInterno);
            project.GetType().GetProperty("ExternalId")?.SetValue(project, projectExternalId);

            _mockProjectRepository.Setup(r => r.GetByIdAsync(projectExternalId)).ReturnsAsync(project);
            _mockUserRepository.Setup(r => r.GetByExternalIdAsync(_testAssignedUser.ExternalId)).ReturnsAsync(_testAssignedUser);
            _mockProjectTaskRepository.Setup(r => r.CountTasksByProjectIdAsync(projectIdInterno)).ReturnsAsync(20);

            // Act
            Func<Task> act = async () => await _projectTaskService.CreateProjectTaskAsync(
                projectExternalId, "Tarefa Excedente", "Desc", DateTime.Today.AddDays(1), ProjectTaskPriority.Low, _testAssignedUser.ExternalId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                       .WithMessage($"O projeto '{projectName}' atingiu o limite máximo de 20 tarefas.");

            _mockProjectRepository.Verify(r => r.GetByIdAsync(projectExternalId), Times.Once());
            _mockUserRepository.Verify(r => r.GetByExternalIdAsync(_testAssignedUser.ExternalId), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.CountTasksByProjectIdAsync(projectIdInterno), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.AddAsync(It.IsAny<ProjectTask>()), Times.Never());
        }
        
        [Fact(DisplayName = @"DADO um id de tarefa válido
                            QUANDO a tarefa for encontrada
                            ENTÃO deve retornar a ProjectTask")]
        public async Task GetProjectTaskByExternalIdAsync_TarefaEncontrada_DeveRetornarComSucesso()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var projectTask = new ProjectTask("Título", "Descrição", DateTime.Today.AddDays(1), ProjectTaskPriority.Low, 1, 1);

            projectTask.GetType().GetProperty("ExternalId")?.SetValue(projectTask, taskExternalId);

            _mockProjectTaskRepository
                .Setup(r => r.GetByIdAsync(taskExternalId))
                .ReturnsAsync(projectTask);

            // Act
            var result = await _projectTaskService.GetProjectTaskByExternalIdAsync(taskExternalId);

            // Assert
            result.Should().NotBeNull();
            result.ExternalId.Should().Be(taskExternalId);

            _mockProjectTaskRepository.Verify(r => r.GetByIdAsync(taskExternalId), Times.Once());
        }

        [Fact(DisplayName = @"DADO um id válido
                            QUANDO a tarefa não for encontrada
                            ENTÃO deve retornar null")]
        public async Task GetProjectTaskByExternalIdAsync_TarefaNaoEncontrada_DeveRetornarNull()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();

            _mockProjectTaskRepository
                .Setup(r => r.GetByIdAsync(taskExternalId))
                .ReturnsAsync((ProjectTask?)null);

            // Act
            var result = await _projectTaskService.GetProjectTaskByExternalIdAsync(taskExternalId);

            // Assert
            result.Should().BeNull();

            _mockProjectTaskRepository.Verify(r => r.GetByIdAsync(taskExternalId), Times.Once());
        }

        [Fact(DisplayName = @"DADO um id de projeto
                            QUANDO existirem tarefas associadas
                            ENTÃO deve retornar todas as tarefas")]
        public async Task GetAllProjectTasksByProjectIdAsync_TarefasExistem_DeveRetornarTarefasDoProjeto()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();
            var tasks = new List<ProjectTask>
            {
                new ("Título 1", "Descrição 1", DateTime.Today.AddDays(1), ProjectTaskPriority.Low, 1, 1),
                new ("Título 2", "Descrição 2", DateTime.Today.AddDays(2), ProjectTaskPriority.Medium, 1, 1)
            };

            tasks[0].GetType().GetProperty("ExternalId")?.SetValue(tasks[0], Guid.NewGuid());
            tasks[1].GetType().GetProperty("ExternalId")?.SetValue(tasks[1], Guid.NewGuid());

            _mockProjectTaskRepository
                .Setup(r => r.GetAllByProjectIdAsync(projectExternalId))
                .ReturnsAsync(tasks);

            // Act
            var result = await _projectTaskService.GetAllProjectTasksByProjectIdAsync(projectExternalId);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(tasks);

            _mockProjectTaskRepository.Verify(r => r.GetAllByProjectIdAsync(projectExternalId), Times.Once());
        }

        [Fact(DisplayName = @"DADO um id de projeto
                            QUANDO não existirem tarefas associadas
                            ENTÃO deve retornar uma lista vazia")]
        public async Task GetAllProjectTasksByProjectIdAsync_NenhumaTarefaExiste_DeveRetornarListaVazia()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();

            _mockProjectTaskRepository
                .Setup(r => r.GetAllByProjectIdAsync(projectExternalId))
                .ReturnsAsync([]);

            // Act
            var result = await _projectTaskService.GetAllProjectTasksByProjectIdAsync(projectExternalId);

            // Assert
            result.Should().NotBeNull().And.BeEmpty();

            _mockProjectTaskRepository.Verify(r => r.GetAllByProjectIdAsync(projectExternalId), Times.Once());
        }

        [Fact(DisplayName = @"DADO uma requisição de atualização de detalhes de tarefa
                            QUANDO a tarefa for encontrada 
                            E dados válidos
                            ENTÃO deve atualizar com sucesso")]
        public async Task UpdateProjectTaskDetailsAsync_DadosValidos_DeveAtualizarERetornaTrue()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var projectIdOriginal = Guid.NewGuid();
            var projectIdInterno = 1;
            var modifiedByUserId = Guid.NewGuid();

            var oldTitle = "Título anterior";
            var oldDescription = "Descrição anterior";
            var oldDeadline = DateTime.Today.AddDays(5);
            var oldStatus = ProjectTaskStatus.InProgress;

            var existingTask = new ProjectTask(
                oldTitle,
                oldDescription,
                oldDeadline, ProjectTaskPriority.Low,
                projectIdInterno,
                _testAssignedUser.Id
            );
            existingTask.GetType().GetProperty("ExternalId")?.SetValue(existingTask, taskExternalId);
            existingTask.GetType().GetProperty("Id")?.SetValue(existingTask, 1);
            existingTask.UpdateStatus(oldStatus);

            var parentProject = new Project("Projeto Pai", "Descrição do projeto pai", 1);
            parentProject.GetType().GetProperty("ExternalId")?.SetValue(parentProject, projectIdOriginal);
            parentProject.GetType().GetProperty("Id")?.SetValue(parentProject, 1);
            existingTask.GetType().GetProperty("Project")?.SetValue(existingTask, parentProject);


            var newTitle = "Novo título";
            var newDescription = "Nova descrição";
            var newDeadline = DateTime.Today.AddDays(15);

            _mockProjectTaskRepository.Setup(r => r.GetByIdAsync(taskExternalId)).ReturnsAsync(existingTask);
            _mockProjectTaskRepository.Setup(r => r.Update(It.IsAny<ProjectTask>()));
            _mockProjectTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            _mockProjectTaskHistoryRepository.Setup(r => r.AddAsync(It.IsAny<ProjectTaskHistory>())).Returns(Task.CompletedTask);
            _mockProjectTaskHistoryRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _projectTaskService.UpdateProjectTaskDetailsAsync(taskExternalId, newTitle, newDescription, newDeadline, modifiedByUserId);

            // Assert
            result.Should().BeTrue();
            existingTask.Title.Should().Be(newTitle);
            existingTask.Description.Should().Be(newDescription);
            existingTask.Deadline.Should().Be(newDeadline);

            _mockProjectTaskRepository.Verify(r => r.GetByIdAsync(taskExternalId), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.Update(existingTask), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once());

            _mockProjectTaskHistoryRepository.Verify(
                r => r.AddAsync(It.Is<ProjectTaskHistory>(h =>
                    h.ProjectTaskId == existingTask.Id &&
                    h.PropertyName == "Title" &&
                    h.OldValue == oldTitle &&
                    h.NewValue == newTitle &&
                    h.ModifiedByUserId == modifiedByUserId)),
                Times.Once()
            );

            _mockProjectTaskHistoryRepository.Verify(
                r => r.AddAsync(It.Is<ProjectTaskHistory>(h =>
                    h.ProjectTaskId == existingTask.Id &&
                    h.PropertyName == "Description" &&
                    h.OldValue == oldDescription &&
                    h.NewValue == newDescription &&
                    h.ModifiedByUserId == modifiedByUserId)),
                Times.Once()
            );

            _mockProjectTaskHistoryRepository.Verify(
                r => r.AddAsync(It.Is<ProjectTaskHistory>(h =>
                    h.ProjectTaskId == existingTask.Id &&
                    h.PropertyName == "Deadline" &&
                    h.OldValue == oldDeadline.ToString("yyyy-MM-dd") &&
                    h.NewValue == newDeadline.ToString("yyyy-MM-dd") &&
                    h.ModifiedByUserId == modifiedByUserId)),
                Times.Once()
            );

            _mockProjectTaskHistoryRepository.Verify(r => r.SaveChangesAsync(), Times.Once());
        }

        [Fact(DisplayName = @"DADO uma requisição de atualização
                            QUANDO o serviço for chamado
                            ENTÃO deve atualizar a tarefa, mas NÃO REGISTRAR HISTÓRICO")]
        public async Task UpdateProjectTaskDetailsAsync_SemAlteracoes_NaoDeveRegistrarHistorico()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var projectIdInterno = 1;
            var modifiedByUserId = Guid.NewGuid();

            var title = "Mesmo Título";
            var description = "Mesma Descrição";
            var deadline = DateTime.Today.AddDays(5);

            var existingTask = new ProjectTask(title, description, deadline, ProjectTaskPriority.Low, projectIdInterno, 1);
            existingTask.GetType().GetProperty("ExternalId")?.SetValue(existingTask, taskExternalId);
            existingTask.GetType().GetProperty("Project")?.SetValue(existingTask, new Project("Parent Project", "Desc", 1));


            _mockProjectTaskRepository.Setup(r => r.GetByIdAsync(taskExternalId)).ReturnsAsync(existingTask);
            _mockProjectTaskRepository.Setup(r => r.Update(It.IsAny<ProjectTask>()));
            _mockProjectTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            _mockProjectTaskHistoryRepository.Setup(r => r.AddAsync(It.IsAny<ProjectTaskHistory>())).Returns(Task.CompletedTask);
            _mockProjectTaskHistoryRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _projectTaskService.UpdateProjectTaskDetailsAsync(taskExternalId, title, description, deadline, modifiedByUserId);

            // Assert
            result.Should().BeTrue();

            _mockProjectTaskRepository.Verify(r => r.GetByIdAsync(taskExternalId), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.Update(existingTask), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once());

            _mockProjectTaskHistoryRepository.Verify(r => r.AddAsync(It.IsAny<ProjectTaskHistory>()), Times.Never());
            _mockProjectTaskHistoryRepository.Verify(r => r.SaveChangesAsync(), Times.Never());
        }

        [Fact(DisplayName = @"DADO uma requisição de atualização
                            QUANDO a tarefa não for encontrada
                            ENTÃO deve retornar false")]
        public async Task UpdateProjectTaskDetailsAsync_TarefaNaoEncontrada_DeveRetornarFalse()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var userId = Guid.NewGuid();


            _mockProjectTaskRepository
                .Setup(r => r.GetByIdAsync(taskExternalId))
                .ReturnsAsync((ProjectTask?)null);

            // Act
            var result = await _projectTaskService.UpdateProjectTaskDetailsAsync(taskExternalId, "Título", "Descrição", DateTime.Today, userId);

            // Assert
            result.Should().BeFalse();

            _mockProjectTaskRepository.Verify(r => r.Update(It.IsAny<ProjectTask>()), Times.Never());
            _mockProjectTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never());
        }

        [Fact(DisplayName = @"DADO uma requisição de atualização
                            QUANDO os dados forem inválidos
                            ENTÃO deve lançar uma ArgumentException")]
        public async Task UpdateProjectTaskDetailsAsync_DadosInvalidos_DeveLancarArgumentException()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingTask = new ProjectTask("Título", "Descrição", DateTime.Today.AddDays(5), ProjectTaskPriority.Low, 1, 1);
            existingTask.GetType().GetProperty("ExternalId")?.SetValue(existingTask, taskExternalId);

            var newTitle = "Novo título";
            var newDescription = "Nova descrição";
            var newDeadline = DateTime.Today.AddDays(-1);

            _mockProjectTaskRepository
                .Setup(r => r.GetByIdAsync(taskExternalId))
                .ReturnsAsync(existingTask);

            // Act & Assert
            Func<Task> act = async () => await _projectTaskService.UpdateProjectTaskDetailsAsync(
                taskExternalId,
                newTitle,
                newDescription,
                newDeadline,
                userId
            );

            await act.Should().ThrowAsync<ArgumentException>()
                       .WithMessage("A data de vencimento não pode ser no passado.*");

            _mockProjectTaskRepository.Verify(r => r.Update(It.IsAny<ProjectTask>()), Times.Never());
            _mockProjectTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never());
        }

        [Fact(DisplayName = @"DADO uma requisição de atualização de status de tarefa
                            QUANDO a tarefa for encontrada 
                            E dados válidos
                            ENTÃO deve atualizar o status da tarefa")]
        public async Task UpdateProjectTaskStatusAsync_StatusValido_DeveAtualizarERetornarTrue()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var projectIdOriginal = Guid.NewGuid();
            var projectIdInterno = 1;
            var modifiedByUserId = Guid.NewGuid();

            var oldStatus = ProjectTaskStatus.Pending;
            var newStatus = ProjectTaskStatus.Completed;

            var existingTask = new ProjectTask("Título", "Descrição", DateTime.Today.AddDays(5), ProjectTaskPriority.Low, projectIdInterno, 1);
            existingTask.GetType().GetProperty("ExternalId")?.SetValue(existingTask, taskExternalId);
            existingTask.GetType().GetProperty("Id")?.SetValue(existingTask, 1);
            existingTask.UpdateStatus(oldStatus);

            var parentProject = new Project("Projeto", "Descrição", 1);
            parentProject.GetType().GetProperty("ExternalId")?.SetValue(parentProject, projectIdOriginal);
            existingTask.GetType().GetProperty("Project")?.SetValue(existingTask, parentProject);

            _mockProjectTaskRepository.Setup(r => r.GetByIdAsync(taskExternalId)).ReturnsAsync(existingTask);
            _mockProjectTaskRepository.Setup(r => r.Update(It.IsAny<ProjectTask>()));
            _mockProjectTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            _mockProjectTaskHistoryRepository
                .Setup(r => r.AddAsync(It.IsAny<ProjectTaskHistory>()))
                .Returns(Task.CompletedTask);
            _mockProjectTaskHistoryRepository
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _projectTaskService.UpdateProjectTaskStatusAsync(taskExternalId, newStatus, modifiedByUserId);

            // Assert
            result.Should().BeTrue();
            existingTask.Status.Should().Be(newStatus);

            _mockProjectTaskRepository.Verify(r => r.GetByIdAsync(taskExternalId), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.Update(existingTask), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once());

            _mockProjectTaskHistoryRepository.Verify(
                r => r.AddAsync(It.Is<ProjectTaskHistory>(h =>
                    h.ProjectTaskId == existingTask.Id &&
                    h.PropertyName == "Status" &&
                    h.OldValue == oldStatus.ToString() &&
                    h.NewValue == newStatus.ToString() &&
                    h.ModifiedByUserId == modifiedByUserId)),
                Times.Once()
            );

            _mockProjectTaskHistoryRepository.Verify(r => r.SaveChangesAsync(), Times.Once());
        }

        [Fact(DisplayName = @"DADO uma requisição de atualização sem alteração de valor
                            QUANDO o serviço for chamado
                            ENTÃO deve atualizar a tarefa, mas NÃO REGISTRAR HISTÓRICO")]
        public async Task UpdateProjectTaskStatusAsync_SemAlteracaoDeStatus_NaoDeveRegistrarHistorico()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var projectIdInterno = 1;
            var modifiedByUserId = Guid.NewGuid();

            var status = ProjectTaskStatus.Pending;

            var existingTask = new ProjectTask("Título", "Descrição", DateTime.Today.AddDays(5), ProjectTaskPriority.Low, projectIdInterno, 1);
            existingTask.GetType().GetProperty("ExternalId")?.SetValue(existingTask, taskExternalId);
            existingTask.UpdateStatus(status);
            existingTask.GetType().GetProperty("Project")?.SetValue(existingTask, new Project("Projeto", "Descrição", 1));

            _mockProjectTaskRepository.Setup(r => r.GetByIdAsync(taskExternalId)).ReturnsAsync(existingTask);
            _mockProjectTaskRepository.Setup(r => r.Update(It.IsAny<ProjectTask>()));
            _mockProjectTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            _mockProjectTaskHistoryRepository
                .Setup(r => r.AddAsync(It.IsAny<ProjectTaskHistory>()))
                .Returns(Task.CompletedTask);
            _mockProjectTaskHistoryRepository
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _projectTaskService.UpdateProjectTaskStatusAsync(taskExternalId, status, modifiedByUserId);

            // Assert
            result.Should().BeTrue();

            _mockProjectTaskRepository.Verify(r => r.GetByIdAsync(taskExternalId), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.Update(existingTask), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once());

            _mockProjectTaskHistoryRepository.Verify(r => r.AddAsync(It.IsAny<ProjectTaskHistory>()), Times.Never());
            _mockProjectTaskHistoryRepository.Verify(r => r.SaveChangesAsync(), Times.Never());
        }

        [Fact(DisplayName = @"DADO uma requisição de atualização
                            QUANDO a tarefa não for encontrada
                            ENTÃO deve retornar false")]
        public async Task UpdateProjectTaskStatusAsync_TarefaNaoEncontrada_DeveRetornarFalse()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _mockProjectTaskRepository
                .Setup(r => r.GetByIdAsync(taskExternalId))
                .ReturnsAsync((ProjectTask?)null);

            // Act
            var result = await _projectTaskService.UpdateProjectTaskStatusAsync(taskExternalId, ProjectTaskStatus.Completed, userId);

            // Assert
            result.Should().BeFalse();

            _mockProjectTaskRepository.Verify(r => r.Update(It.IsAny<ProjectTask>()), Times.Never());
            _mockProjectTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never());
        }

        [Fact(DisplayName = @"DADO uma requisição de exclusão de tarefa
                            QUANDO a tarefa for encontrada
                            ENTÃO deve excluir com suceso")]
        public async Task DeleteProjectTaskAsync_TarefaEncontrada_DeveExcluirERetornarTrue()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var existingTask = new ProjectTask("Título", "Descrição", DateTime.Today.AddDays(5), ProjectTaskPriority.Low, 1, 1);

            existingTask.GetType().GetProperty("ExternalId")?.SetValue(existingTask, taskExternalId);

            _mockProjectTaskRepository
                .Setup(r => r.GetByIdAsync(taskExternalId))
                .ReturnsAsync(existingTask);
            _mockProjectTaskRepository.Setup(r => r.Delete(It.IsAny<ProjectTask>()));
            _mockProjectTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _projectTaskService.DeleteProjectTaskAsync(taskExternalId);

            // Assert
            result.Should().BeTrue();

            _mockProjectTaskRepository.Verify(r => r.GetByIdAsync(taskExternalId), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.Delete(existingTask), Times.Once());
            _mockProjectTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once());
        }

        [Fact(DisplayName = @"DADO uma requisição de exclusão
                            QUANDO a tarefa não for encontrada
                            ENTÃO deve retornar false")]
        public async Task DeleteProjectTaskAsync_TarefaNaoEncontrada_DeveRetornarFalse()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();

            _mockProjectTaskRepository
                .Setup(r => r.GetByIdAsync(taskExternalId))
                .ReturnsAsync((ProjectTask?)null);

            // Act
            var result = await _projectTaskService.DeleteProjectTaskAsync(taskExternalId);

            // Assert
            result.Should().BeFalse();

            _mockProjectTaskRepository.Verify(r => r.Delete(It.IsAny<ProjectTask>()), Times.Never());
            _mockProjectTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never());
        }
    }
}
