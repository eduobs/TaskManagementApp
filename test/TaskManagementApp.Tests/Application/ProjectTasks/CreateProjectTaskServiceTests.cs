using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementApp.Application.ProjectTasks;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Tests.Application.ProjectTasks
{
    public class CreateProjectTaskServiceTests
    {
        private readonly Mock<IProjectTaskService> _mockProjectTaskDomainService;
        private readonly Mock<ILogger<CreateProjectTaskService>> _mockLogger;
        private readonly CreateProjectTaskService _createProjectTaskService;

        public CreateProjectTaskServiceTests()
        {
            _mockProjectTaskDomainService = new Mock<IProjectTaskService>();
            _mockLogger = new Mock<ILogger<CreateProjectTaskService>>();

            _createProjectTaskService = new CreateProjectTaskService(
                _mockProjectTaskDomainService.Object,
                _mockLogger.Object
            );
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

            var projectTask = new ProjectTask(
                request.Title,
                request.Description,
                request.Deadline,
                (TaskManagementApp.Domain.Enums.ProjectTaskPriority)request.Priority,
                1
            );

            projectTask.GetType().GetProperty("ExternalId")?.SetValue(projectTask, Guid.NewGuid());

            var project = new Project("Projeto", "Descrição", 1);
            project.GetType().GetProperty("ExternalId")?.SetValue(project, projectExternalId);
            projectTask.GetType().GetProperty("Project")?.SetValue(projectTask, project);

            _mockProjectTaskDomainService
                .Setup(s => s.CreateProjectTaskAsync(
                    projectExternalId,
                    request.Title,
                    request.Description,
                    request.Deadline,
                    (TaskManagementApp.Domain.Enums.ProjectTaskPriority)request.Priority
                ))
                .ReturnsAsync(projectTask);

            // Act
            var result = await _createProjectTaskService.ExecuteAsync(projectExternalId, request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(projectTask.ExternalId);
            result.Title.Should().Be(request.Title);
            result.Description.Should().Be(request.Description);
            result.Deadline.Should().Be(request.Deadline);
            result.Priority.Should().Be(request.Priority);
            result.Status.Should().Be(Models.Enums.ProjectTaskStatus.Pending);
            result.ProjectId.Should().Be(projectExternalId);

            _mockProjectTaskDomainService.Verify(s => s.CreateProjectTaskAsync(
                projectExternalId,
                request.Title,
                request.Description,
                request.Deadline,
                (TaskManagementApp.Domain.Enums.ProjectTaskPriority)request.Priority
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

            _mockProjectTaskDomainService
                .Setup(s => s.CreateProjectTaskAsync(
                    projectExternalId,
                    request.Title,
                    request.Description,
                    request.Deadline,
                    (TaskManagementApp.Domain.Enums.ProjectTaskPriority)request.Priority
                ))
                .ThrowsAsync(expectedException);

            // Act & Assert
            Func<Task> act = async () => await _createProjectTaskService.ExecuteAsync(projectExternalId, request);

            await act.Should().ThrowAsync<ArgumentException>()
                       .WithMessage(expectedException.Message);

            _mockProjectTaskDomainService.Verify(s => s.CreateProjectTaskAsync(
                projectExternalId,
                request.Title,
                request.Description,
                request.Deadline,
                (TaskManagementApp.Domain.Enums.ProjectTaskPriority)request.Priority
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

            _mockProjectTaskDomainService
                .Setup(s => s.CreateProjectTaskAsync(
                    projectExternalId,
                    request.Title,
                    request.Description,
                    request.Deadline,
                    (TaskManagementApp.Domain.Enums.ProjectTaskPriority)request.Priority
                ))
                .ThrowsAsync(expectedException);

            // Act & Assert
            Func<Task> act = async () => await _createProjectTaskService.ExecuteAsync(projectExternalId, request);

            await act.Should().ThrowAsync<InvalidOperationException>()
                       .WithMessage(expectedException.Message);

            _mockProjectTaskDomainService.Verify(s => s.CreateProjectTaskAsync(
                projectExternalId,
                request.Title,
                request.Description,
                request.Deadline,
                (TaskManagementApp.Domain.Enums.ProjectTaskPriority)request.Priority
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

            _mockProjectTaskDomainService
                .Setup(s => s.CreateProjectTaskAsync(
                    projectExternalId,
                    request.Title,
                    request.Description,
                    request.Deadline,
                    (TaskManagementApp.Domain.Enums.ProjectTaskPriority)request.Priority
                ))
                .ThrowsAsync(expectedException);

            // Act & Assert
            Func<Task> act = async () => await _createProjectTaskService.ExecuteAsync(projectExternalId, request);

            await act.Should().ThrowAsync<Exception>()
                       .WithMessage(expectedException.Message);

            _mockProjectTaskDomainService.Verify(s => s.CreateProjectTaskAsync(
                projectExternalId,
                request.Title,
                request.Description,
                request.Deadline,
                (TaskManagementApp.Domain.Enums.ProjectTaskPriority)request.Priority
            ), Times.Once());
        }
    }
}
