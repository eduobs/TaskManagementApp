using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementApp.Application.ProjectTasks;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Tests.Application.ProjectTasks
{
    public class GetProjectTasksByProjectIdServiceTests
    {
        private readonly Mock<IProjectTaskService> _mockProjectTaskDomainService;
        private readonly Mock<ILogger<GetProjectTasksByProjectIdService>> _mockLogger;
        private readonly GetProjectTasksByProjectIdService _getProjectTasksByProjectIdService;

        public GetProjectTasksByProjectIdServiceTests()
        {
            _mockProjectTaskDomainService = new Mock<IProjectTaskService>();
            _mockLogger = new Mock<ILogger<GetProjectTasksByProjectIdService>>();

            _getProjectTasksByProjectIdService = new GetProjectTasksByProjectIdService(
                _mockProjectTaskDomainService.Object,
                _mockLogger.Object
            );
        }

        [Fact(DisplayName = @"DADO um Id válido
                            QUANDO existirem tarefas
                            ENTÃO deve retornar uma lista")]
        public async Task ExecuteAsync_ProjetoComTarefas_DeveRetornarListaDeTarefas()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();
            var projectIdInterno = 1;

            var project = new Project("Projeto Teste", "Descrição");
            project.GetType().GetProperty("ExternalId")?.SetValue(project, projectExternalId);
            project.GetType().GetProperty("Id")?.SetValue(project, projectIdInterno);


            var tasks = new List<ProjectTask>
            {
                new ("Tarefa 1", "Descrição 1", DateTime.Today.AddDays(10), TaskManagementApp.Domain.Enums.ProjectTaskPriority.Low, projectIdInterno),
                new ("Tarefa 2", "Descrição 2", DateTime.Today.AddDays(5), TaskManagementApp.Domain.Enums.ProjectTaskPriority.High, projectIdInterno)
            };

            tasks[0].GetType().GetProperty("ExternalId")?.SetValue(tasks[0], Guid.NewGuid());
            tasks[0].GetType().GetProperty("Project")?.SetValue(tasks[0], project);

            tasks[1].GetType().GetProperty("ExternalId")?.SetValue(tasks[1], Guid.NewGuid());
            tasks[1].GetType().GetProperty("Project")?.SetValue(tasks[1], project);


            _mockProjectTaskDomainService
                .Setup(s => s.GetAllProjectTasksByProjectIdAsync(projectExternalId))
                .ReturnsAsync(tasks);

            // Act
            var result = await _getProjectTasksByProjectIdService.ExecuteAsync(projectExternalId);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(2);

            result.Should().ContainEquivalentOf(new ProjectTaskResponse
            {
                Id = tasks[0].ExternalId,
                Title = tasks[0].Title,
                Description = tasks[0].Description,
                Deadline = tasks[0].Deadline,
                Status = (Models.Enums.ProjectTaskStatus)tasks[0].Status,
                Priority = (Models.Enums.ProjectTaskPriority)tasks[0].Priority,
                ProjectId = projectExternalId
            });

            _mockProjectTaskDomainService.Verify(s => s.GetAllProjectTasksByProjectIdAsync(projectExternalId), Times.Once());
        }

        [Fact(DisplayName = @"DADO um Id válido
                            QUANDO não existirem tarefas
                            ENTÃO deve retornar uma lista vazia")]
        public async Task ExecuteAsync_ProjetoSemTarefas_DeveRetornarListaVazia()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();

            _mockProjectTaskDomainService
                .Setup(s => s.GetAllProjectTasksByProjectIdAsync(projectExternalId))
                .ReturnsAsync([]);

            // Act
            var result = await _getProjectTasksByProjectIdService.ExecuteAsync(projectExternalId);

            // Assert
            result.Should().NotBeNull().And.BeEmpty();
            result.Should().HaveCount(0);

            _mockProjectTaskDomainService.Verify(s => s.GetAllProjectTasksByProjectIdAsync(projectExternalId), Times.Once());
        }

        [Fact(DisplayName = @"DADO um Id de projeto
                            QUANDO o serviço lançar uma exceção
                            ENTÃO deve retornar exceção")]
        public async Task ExecuteAsync_ServicoLancaExcecao_DeveRetornarExcecao()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();
            var exception = new Exception("Erro de comunicação com o serviço de domínio.");

            _mockProjectTaskDomainService
                .Setup(s => s.GetAllProjectTasksByProjectIdAsync(projectExternalId))
                .ThrowsAsync(exception);

            // Act
            Func<Task> act = async () => await _getProjectTasksByProjectIdService.ExecuteAsync(projectExternalId);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                       .WithMessage(exception.Message);

            _mockProjectTaskDomainService.Verify(s => s.GetAllProjectTasksByProjectIdAsync(projectExternalId), Times.Once());
        }
    }
}
