using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementApp.Application.ProjectTasks;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Tests.Application.ProjectTasks
{
    public class UpdateProjectTaskServiceTests
    {
        private readonly Mock<IProjectTaskService> _mockProjectTaskDomainService;
        private readonly Mock<ILogger<UpdateProjectTaskService>> _mockLogger;
        private readonly UpdateProjectTaskService _updateProjectTaskService;

        public UpdateProjectTaskServiceTests()
        {
            _mockProjectTaskDomainService = new Mock<IProjectTaskService>();
            _mockLogger = new Mock<ILogger<UpdateProjectTaskService>>();
            _updateProjectTaskService = new UpdateProjectTaskService(
                _mockProjectTaskDomainService.Object,
                _mockLogger.Object
            );
        }

        [Fact(DisplayName = @"DADO uma requisição de atualização
                            QUANDO dados válidos
                            ENTÃO deve atualizar com sucesso")]
        public async Task ExecuteAsync_DadosValidos_DeveSalvarComSucesso()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var projectIdOriginal = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var projectIdInterno = 1;

            var request = new UpdateProjectTaskRequest
            {
                Title = "Novo Título da Tarefa",
                Description = "Nova Descrição da Tarefa.",
                Deadline = DateTime.Today.AddDays(15)
            };

            var existingTask = new ProjectTask(
                "Título", "Descrição",
                DateTime.Today.AddDays(5),
                TaskManagementApp.Domain.Enums.ProjectTaskPriority.Low,
                projectIdInterno,
                1
            );
            existingTask.GetType().GetProperty("ExternalId")?.SetValue(existingTask, taskExternalId);

            var project = new Project("Projeto Original", "Descrição", 1);
            project.GetType().GetProperty("ExternalId")?.SetValue(project, projectIdOriginal);
            existingTask.GetType().GetProperty("Project")?.SetValue(existingTask, project);

            var task = new ProjectTask(
                request.Title,
                request.Description,
                request.Deadline,
                TaskManagementApp.Domain.Enums.ProjectTaskPriority.Low,
                projectIdInterno,
                1
            );
            task.GetType().GetProperty("ExternalId")?.SetValue(task, taskExternalId);
            task.GetType().GetProperty("Project")?.SetValue(task, project);

            task.UpdateStatus(TaskManagementApp.Domain.Enums.ProjectTaskStatus.Pending);

            _mockProjectTaskDomainService
                .Setup(s => s.UpdateProjectTaskDetailsAsync(
                    taskExternalId,
                    request.Title,
                    request.Description,
                    request.Deadline,
                    userId
                ))
                .ReturnsAsync(true);

            _mockProjectTaskDomainService
                .Setup(s => s.GetProjectTaskByExternalIdAsync(taskExternalId))
                .ReturnsAsync(task);

            // Act
            var result = await _updateProjectTaskService.ExecuteAsync(taskExternalId, request, userId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(taskExternalId);
            result.Title.Should().Be(request.Title);
            result.Description.Should().Be(request.Description);
            result.Deadline.Should().Be(request.Deadline);
            result.Priority.Should().Be(Models.Enums.ProjectTaskPriority.Low);
            result.Status.Should().Be(Models.Enums.ProjectTaskStatus.Pending);
            result.ProjectId.Should().Be(projectIdOriginal);

            _mockProjectTaskDomainService.Verify(s => s.UpdateProjectTaskDetailsAsync(
                taskExternalId,
                request.Title,
                request.Description,
                request.Deadline,
                userId
            ), Times.Once());

            _mockProjectTaskDomainService.Verify(s => s.GetProjectTaskByExternalIdAsync(taskExternalId), Times.Once());
        }

        [Fact(DisplayName = @"DADO uma requisição de atualização
                            QUANDO a tarefa não for encontrada
                            ENTÃO deve retornar null")]
        public async Task ExecuteAsync_TarefaNaoEncontrada_DeveRetornarNull()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = new UpdateProjectTaskRequest
            {
                Title = "Título",
                Description = "Descrição",
                Deadline = DateTime.Today.AddDays(10)
            };

            _mockProjectTaskDomainService
                .Setup(s => s.UpdateProjectTaskDetailsAsync(
                    taskExternalId,
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>(),
                    userId
                ))
                .ReturnsAsync(false);

            // Act
            var result = await _updateProjectTaskService.ExecuteAsync(taskExternalId, request, userId);

            // Assert
            result.Should().BeNull();

            _mockProjectTaskDomainService.Verify(s => s.UpdateProjectTaskDetailsAsync(
                taskExternalId,
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                userId
            ), Times.Once());

            _mockProjectTaskDomainService.Verify(s => s.GetProjectTaskByExternalIdAsync(It.IsAny<Guid>()), Times.Never());
        }

        [Fact(DisplayName = @"DADO uma requisição inválida
                            QUANDO o serviço de lançar ArgumentException
                            ENTÃO deve retornar exceção")]
        public async Task ExecuteAsync_ServicoLancaException_DeveRetornarExcecao()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = new UpdateProjectTaskRequest
            {
                Title = "Novo Título",
                Description = "Nova Descrição",
                Deadline = DateTime.Today.AddDays(-1)
            };

            var exception = new ArgumentException("A data limite (deadline) não pode ser no passado.");

            _mockProjectTaskDomainService
                .Setup(s => s.UpdateProjectTaskDetailsAsync(
                    taskExternalId,
                    request.Title,
                    request.Description,
                    request.Deadline,
                    userId
                ))
                .ThrowsAsync(exception);

            // Act & Assert
            Func<Task> act = async () => await _updateProjectTaskService.ExecuteAsync(taskExternalId, request, userId);

            await act.Should().ThrowAsync<ArgumentException>().WithMessage(exception.Message);

            _mockProjectTaskDomainService.Verify(s => s.UpdateProjectTaskDetailsAsync(
                taskExternalId,
                request.Title,
                request.Description,
                request.Deadline,
                userId
            ), Times.Once());

            _mockProjectTaskDomainService.Verify(s => s.GetProjectTaskByExternalIdAsync(It.IsAny<Guid>()), Times.Never());
        }

        [Fact(DisplayName = @"DADO uma requisição de atualização de tarefa
                            QUANDO o serviço lançar exceção genérica
                            ENTÃO deve retornar umexceção")]
        public async Task ExecuteAsync_ServicoLancaExcecaoGenerica_DeveRetornarExcecao()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = new UpdateProjectTaskRequest
            {
                Title = "Titulo",
                Description = "Descrição",
                Deadline = DateTime.Today.AddDays(10)
            };

            var exception = new Exception("Erro inesperado no serviço de domínio.");

            _mockProjectTaskDomainService
                .Setup(s => s.UpdateProjectTaskDetailsAsync(
                    taskExternalId,
                    request.Title,
                    request.Description,
                    request.Deadline,
                    userId
                ))
                .ThrowsAsync(exception);

            // Act & Assert
            Func<Task> act = async () => await _updateProjectTaskService.ExecuteAsync(taskExternalId, request, userId);

            await act.Should().ThrowAsync<Exception>().WithMessage(exception.Message);

            _mockProjectTaskDomainService.Verify(s => s.UpdateProjectTaskDetailsAsync(
                taskExternalId,
                request.Title,
                request.Description,
                request.Deadline,
                userId
            ), Times.Once());
            
            _mockProjectTaskDomainService.Verify(s => s.GetProjectTaskByExternalIdAsync(It.IsAny<Guid>()), Times.Never());
        }
    }
}
