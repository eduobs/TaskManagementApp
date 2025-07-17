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
    public class UpdateProjectTaskStatusServiceTests
    {
        private readonly Mock<IProjectTaskService> _mockProjectTaskDomainService;
        private readonly Mock<ILogger<UpdateProjectTaskStatusService>> _mockLogger;
        private readonly UpdateProjectTaskStatusService _updateProjectTaskStatusService;
        private readonly User _testModifyingUser;

        public UpdateProjectTaskStatusServiceTests()
        {
            _mockProjectTaskDomainService = new Mock<IProjectTaskService>();
            _mockLogger = new Mock<ILogger<UpdateProjectTaskStatusService>>();

            _updateProjectTaskStatusService = new UpdateProjectTaskStatusService(
                _mockProjectTaskDomainService.Object,
                _mockLogger.Object
            );
            
            _testModifyingUser = new User("Usuário Teste", UserRole.Basic);
            _testModifyingUser.GetType().GetProperty("Id")?.SetValue(_testModifyingUser, 20);
            _testModifyingUser.GetType().GetProperty("ExternalId")?.SetValue(_testModifyingUser, Guid.NewGuid());
        }

        [Fact(DisplayName = @"DADO uma requisição dados válidos
                            QUANDO o serviço atualizar com sucesso
                            ENTÃO deve retornar tarefa atualizada")]
        public async Task ExecuteAsync_RequisicaoValida_DeveRetornarComSuceso()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var projectIdOriginal = Guid.NewGuid();
            var projectIdInterno = 1;
            var assignedToUserIdInterno = 10;
            var modifiedByUserId = _testModifyingUser.ExternalId;

            var request = new UpdateProjectTaskStatusRequest
            {
                Status = Models.Enums.ProjectTaskStatus.Completed
            };

            var task = new ProjectTask(
                "Título Original",
                "Descrição Original",
                DateTime.Today.AddDays(5),
                ProjectTaskPriority.Low,
                projectIdInterno,
                assignedToUserIdInterno
            );
            task.GetType().GetProperty("ExternalId")?.SetValue(task, taskExternalId);
            var parentProject = new Project("Projeto Original", "Descrição", 1);
            parentProject.GetType().GetProperty("ExternalId")?.SetValue(parentProject, projectIdOriginal);
            task.GetType().GetProperty("Project")?.SetValue(task, parentProject);
            task.UpdateStatus(ProjectTaskStatus.InProgress);

            var updatedTask = new ProjectTask(
                task.Title,
                task.Description,
                task.Deadline,
                task.Priority,
                projectIdInterno,
                assignedToUserIdInterno
            );
            updatedTask.GetType().GetProperty("ExternalId")?.SetValue(updatedTask, taskExternalId);
            updatedTask.GetType().GetProperty("Project")?.SetValue(updatedTask, parentProject);
            updatedTask.GetType().GetProperty("AssignedToUser")?.SetValue(updatedTask, _testModifyingUser);            
            updatedTask.UpdateStatus(ProjectTaskStatus.Completed);

            _mockProjectTaskDomainService
                .Setup(s => s.UpdateProjectTaskStatusAsync(
                    taskExternalId,
                    (ProjectTaskStatus)request.Status,
                    modifiedByUserId
                ))
                .ReturnsAsync(true);

            _mockProjectTaskDomainService
                .Setup(s => s.GetProjectTaskByExternalIdAsync(taskExternalId))
                .ReturnsAsync(updatedTask);

            // Act
            var result = await _updateProjectTaskStatusService.ExecuteAsync(taskExternalId, request, modifiedByUserId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(taskExternalId);
            result.Status.Should().Be(request.Status);
            result.Title.Should().Be(updatedTask.Title);
            result.Description.Should().Be(updatedTask.Description);
            result.Deadline.Should().Be(updatedTask.Deadline);
            result.Priority.Should().Be((Models.Enums.ProjectTaskPriority)updatedTask.Priority);
            result.ProjectId.Should().Be(projectIdOriginal);
            result.AssignedToUserId.Should().Be(_testModifyingUser.ExternalId);
            result.AssignedToUserName.Should().Be(_testModifyingUser.Name);

            _mockProjectTaskDomainService.Verify(s => s.UpdateProjectTaskStatusAsync(
                taskExternalId,
                (ProjectTaskStatus)request.Status,
                modifiedByUserId
            ), Times.Once());

            _mockProjectTaskDomainService.Verify(s => s.GetProjectTaskByExternalIdAsync(taskExternalId), Times.Once());
        }

        [Fact(DisplayName = @"DADO uma requisição de atualização de status
                            QUANDO a tarefa não for encontrada pelo id
                            ENTÃO deve retornar null")]
        public async Task ExecuteAsync_TarefaNaoEncontrada_DeveRetornarNull()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var modifiedByUserId = _testModifyingUser.ExternalId;
            var request = new UpdateProjectTaskStatusRequest
            {
                Status = Models.Enums.ProjectTaskStatus.Completed
            };

            _mockProjectTaskDomainService
                .Setup(s => s.UpdateProjectTaskStatusAsync(
                    taskExternalId,
                    It.IsAny<ProjectTaskStatus>(),
                    modifiedByUserId
                ))
                .ReturnsAsync(false);

            // Act
            var result = await _updateProjectTaskStatusService.ExecuteAsync(taskExternalId, request, modifiedByUserId);

            // Assert
            result.Should().BeNull();

            _mockProjectTaskDomainService.Verify(s => s.UpdateProjectTaskStatusAsync(
                taskExternalId,
                It.IsAny<ProjectTaskStatus>(),
                modifiedByUserId
            ), Times.Once());

            _mockProjectTaskDomainService.Verify(s => s.GetProjectTaskByExternalIdAsync(It.IsAny<Guid>()), Times.Never());
        }

        [Fact(DisplayName = @"DADO uma requisição com dados inválidos
                            QUANDO serviço lançar ArgumentException
                            ENTÃO deve retornar exceção")]
        public async Task ExecuteAsync_ServicoLancaArgumentException_DeveRetornarExcecao()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var modifiedByUserId = _testModifyingUser.ExternalId;
            var request = new UpdateProjectTaskStatusRequest
            {
                Status = (Models.Enums.ProjectTaskStatus)999
            };
            var exception = new ArgumentException("Status inválido.", "newStatus");

            _mockProjectTaskDomainService
                .Setup(s => s.UpdateProjectTaskStatusAsync(
                    taskExternalId,
                    (ProjectTaskStatus)request.Status,
                    modifiedByUserId
                ))
                .ThrowsAsync(exception);

            // Act & Assert
            Func<Task> act = async () => await _updateProjectTaskStatusService.ExecuteAsync(taskExternalId, request, modifiedByUserId);

            await act.Should().ThrowAsync<ArgumentException>().WithMessage(exception.Message);

            _mockProjectTaskDomainService.Verify(s => s.UpdateProjectTaskStatusAsync(
                taskExternalId,
                (ProjectTaskStatus)request.Status,
                modifiedByUserId
            ), Times.Once());

            _mockProjectTaskDomainService.Verify(s => s.GetProjectTaskByExternalIdAsync(It.IsAny<Guid>()), Times.Never());
        }

        [Fact(DisplayName = @"DADO uma requisição de atualização de status
                            QUANDO o serviço lançar uma exceção genérica
                            ENTÃO deve retornar exceção")]
        public async Task ExecuteAsync_ServicoLancaExcecaoGenerica_DeveLancarExcecao()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var modifiedByUserId = _testModifyingUser.ExternalId;
            var request = new UpdateProjectTaskStatusRequest
            {
                Status = Models.Enums.ProjectTaskStatus.Completed
            };

            var exception = new Exception("Erro inesperado durante a atualização de status.");

            _mockProjectTaskDomainService
                .Setup(s => s.UpdateProjectTaskStatusAsync(
                    taskExternalId,
                    It.IsAny<ProjectTaskStatus>(),
                    modifiedByUserId
                ))
                .ThrowsAsync(exception);

            // Act & Assert
            Func<Task> act = async () => await _updateProjectTaskStatusService.ExecuteAsync(taskExternalId, request, modifiedByUserId);

            await act.Should().ThrowAsync<Exception>().WithMessage(exception.Message);

            _mockProjectTaskDomainService.Verify(s => s.UpdateProjectTaskStatusAsync(
                taskExternalId,
                It.IsAny<ProjectTaskStatus>(),
                modifiedByUserId
            ), Times.Once());

            _mockProjectTaskDomainService.Verify(s => s.GetProjectTaskByExternalIdAsync(It.IsAny<Guid>()), Times.Never());
        }
    }
}
