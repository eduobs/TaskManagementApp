using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementApp.Application.ProjectTasks;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Enums;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Tests.Application.ProjectTasks
{
    public class CreateProjectTaskServiceTests
    {
        private readonly Mock<IProjectTaskService> _mockProjectTaskDomainService;
        private readonly Mock<ILogger<CreateProjectTaskService>> _mockLogger;
        private readonly CreateProjectTaskService _createProjectTaskService;

        private readonly User _testAssignedUser;

        public CreateProjectTaskServiceTests()
        {
            _mockProjectTaskDomainService = new Mock<IProjectTaskService>();
            _mockLogger = new Mock<ILogger<CreateProjectTaskService>>();

            _createProjectTaskService = new CreateProjectTaskService(
                _mockProjectTaskDomainService.Object,
                _mockLogger.Object
            );

            _testAssignedUser = new User("Assigned User Test", UserRole.Basic);
            _testAssignedUser.GetType().GetProperty("Id")?.SetValue(_testAssignedUser, 10);
            _testAssignedUser.GetType().GetProperty("ExternalId")?.SetValue(_testAssignedUser, Guid.NewGuid());
        }

        [Fact(DisplayName = @"DADO uma requisição de criação de uma tarefa
                            QUANDO criar a tarefa com sucesso
                            ENTÃO deve retornar um tarefa criada")]
        public async Task ExecuteAsync_RequisicaoValidaEProjetoExiste_DeveRetornarComSucesso()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();
            var request = new CreateProjectTaskRequest
            {
                Title = "Tarefa",
                Description = "Descrição",
                Deadline = DateTime.Today.AddDays(7),
                Priority = Models.Enums.ProjectTaskPriority.Medium
            };

            var assignedToUserExternalId = _testAssignedUser.ExternalId;

            var projectTask = new ProjectTask(
                request.Title,
                request.Description,
                request.Deadline,
                (ProjectTaskPriority)request.Priority,
                1,
                _testAssignedUser.Id
            );

            projectTask.GetType().GetProperty("ExternalId")?.SetValue(projectTask, Guid.NewGuid());

            var project = new Project("Projeto", "Descrição", 1);
            project.GetType().GetProperty("ExternalId")?.SetValue(project, projectExternalId);
            projectTask.GetType().GetProperty("Project")?.SetValue(projectTask, project);
            projectTask.GetType().GetProperty("AssignedToUser")?.SetValue(projectTask, _testAssignedUser);

            _mockProjectTaskDomainService
                .Setup(s => s.CreateProjectTaskAsync(
                    projectExternalId,
                    request.Title,
                    request.Description,
                    request.Deadline,
                    (ProjectTaskPriority)request.Priority,
                    assignedToUserExternalId
                ))
                .ReturnsAsync(projectTask);

            // Act
            var result = await _createProjectTaskService.ExecuteAsync(projectExternalId, request, assignedToUserExternalId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(projectTask.ExternalId);
            result.Title.Should().Be(request.Title);
            result.Description.Should().Be(request.Description);
            result.Deadline.Should().Be(request.Deadline);
            result.Priority.Should().Be(request.Priority);
            result.Status.Should().Be(Models.Enums.ProjectTaskStatus.Pending);
            result.ProjectId.Should().Be(projectExternalId);
            result.AssignedToUserId.Should().Be(assignedToUserExternalId);
            result.AssignedToUserName.Should().Be(_testAssignedUser.Name);

            _mockProjectTaskDomainService.Verify(s => s.CreateProjectTaskAsync(
                projectExternalId,
                request.Title,
                request.Description,
                request.Deadline,
                (ProjectTaskPriority)request.Priority,
                assignedToUserExternalId
            ), Times.Once());
        }

        [Fact(DisplayName = @"DADO uma requisição de criação de tarefa inválida
                            QUANDO o serviço lançar uma exceção
                            ENTÃO deve lançar exceção")]
        public async Task ExecuteAsync_ServicoLancaArgumentException_DeveRetornarExcecao()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();
            var request = new CreateProjectTaskRequest
            {
                Title = "",
                Description = "Descrição",
                Deadline = DateTime.Today.AddDays(7),
                Priority = Models.Enums.ProjectTaskPriority.Low
            };
            var expectedException = new ArgumentException("O título da tarefa não pode ser nulo ou vazio.");
            var assignedToUserExternalId = _testAssignedUser.ExternalId;

            _mockProjectTaskDomainService
                .Setup(s => s.CreateProjectTaskAsync(
                    projectExternalId,
                    request.Title,
                    request.Description,
                    request.Deadline,
                    (ProjectTaskPriority)request.Priority,
                    assignedToUserExternalId
                ))
                .ThrowsAsync(expectedException);

            // Act & Assert
            Func<Task> act = async () => await _createProjectTaskService.ExecuteAsync(projectExternalId, request, assignedToUserExternalId);

            await act.Should().ThrowAsync<ArgumentException>()
                       .WithMessage(expectedException.Message);

            _mockProjectTaskDomainService.Verify(s => s.CreateProjectTaskAsync(
                projectExternalId,
                request.Title,
                request.Description,
                request.Deadline,
                (ProjectTaskPriority)request.Priority,
                assignedToUserExternalId
            ), Times.Once());
        }

        [Fact(DisplayName = @"DADO uma requisição de criação de tarefa
                            QUANDO excede o limite de tarefas do projeto
                            ENTÃO deve lançar exceção")]
        public async Task ExecuteAsync_ExcedeLimiteDeTarefasDoProjeto_DeveLancarExcecao()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();
            var request = new CreateProjectTaskRequest
            {
                Title = "Tarefa",
                Description = "Descrição",
                Deadline = DateTime.Today.AddDays(7),
                Priority = Models.Enums.ProjectTaskPriority.High
            };

            var expectedException = new InvalidOperationException("O projeto 'Nome do Projeto' atingiu o limite máximo de 20 tarefas.");
            var assignedToUserExternalId = _testAssignedUser.ExternalId;

            _mockProjectTaskDomainService
                .Setup(s => s.CreateProjectTaskAsync(
                    projectExternalId,
                    request.Title,
                    request.Description,
                    request.Deadline,
                    (ProjectTaskPriority)request.Priority,
                    assignedToUserExternalId
                ))
                .ThrowsAsync(expectedException);

            // Act & Assert
            Func<Task> act = async () => await _createProjectTaskService.ExecuteAsync(projectExternalId, request, assignedToUserExternalId);

            await act.Should().ThrowAsync<InvalidOperationException>()
                       .WithMessage(expectedException.Message);

            _mockProjectTaskDomainService.Verify(s => s.CreateProjectTaskAsync(
                projectExternalId,
                request.Title,
                request.Description,
                request.Deadline,
                (ProjectTaskPriority)request.Priority,
                assignedToUserExternalId
            ), Times.Once());
        }

        [Fact(DisplayName = @"DADO uma requisição de criação de tarefa
                            QUANDO o serviço lançar uma exceção genérica
                            ENTÃO deve retornar exceção")]
        public async Task ExecuteAsync_ServicoLancaExcecaoGenerica_DeveRetornarExcecao()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();
            var request = new CreateProjectTaskRequest
            {
                Title = "Tarefa",
                Description = "Descrição",
                Deadline = DateTime.Today.AddDays(7),
                Priority = Models.Enums.ProjectTaskPriority.Low
            };
            var expectedException = new Exception("Erro inesperado durante a criação da tarefa.");
            var assignedToUserExternalId = _testAssignedUser.ExternalId;

            _mockProjectTaskDomainService
                .Setup(s => s.CreateProjectTaskAsync(
                    projectExternalId,
                    request.Title,
                    request.Description,
                    request.Deadline,
                    (ProjectTaskPriority)request.Priority,
                    assignedToUserExternalId
                ))
                .ThrowsAsync(expectedException);

            // Act & Assert
            Func<Task> act = async () => await _createProjectTaskService.ExecuteAsync(projectExternalId, request, assignedToUserExternalId);

            await act.Should().ThrowAsync<Exception>()
                       .WithMessage(expectedException.Message);

            _mockProjectTaskDomainService.Verify(s => s.CreateProjectTaskAsync(
                projectExternalId,
                request.Title,
                request.Description,
                request.Deadline,
                (ProjectTaskPriority)request.Priority,
                assignedToUserExternalId
            ), Times.Once());
        }
    }
}
