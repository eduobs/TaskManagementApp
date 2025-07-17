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
    public class GetProjectTasksByProjectIdServiceTests
    {
        private readonly Mock<IProjectTaskService> _mockProjectTaskDomainService;
        private readonly Mock<ILogger<GetProjectTasksByProjectIdService>> _mockLogger;
        private readonly GetProjectTasksByProjectIdService _getProjectTasksByProjectIdService;
        private readonly User _testAssignedUser;
        private readonly Project _testParentProject;

        public GetProjectTasksByProjectIdServiceTests()
        {
            _mockProjectTaskDomainService = new Mock<IProjectTaskService>();
            _mockLogger = new Mock<ILogger<GetProjectTasksByProjectIdService>>();

            _getProjectTasksByProjectIdService = new GetProjectTasksByProjectIdService(
                _mockProjectTaskDomainService.Object,
                _mockLogger.Object
            );
            
            _testAssignedUser = new User("Usuário Teste", UserRole.Basic);
            _testAssignedUser.GetType().GetProperty("Id")?.SetValue(_testAssignedUser, 10);
            _testAssignedUser.GetType().GetProperty("ExternalId")?.SetValue(_testAssignedUser, Guid.NewGuid());

            _testParentProject = new Project("Projeto teste", "Descrição", 1);
            _testParentProject.GetType().GetProperty("Id")?.SetValue(_testParentProject, 1);
            _testParentProject.GetType().GetProperty("ExternalId")?.SetValue(_testParentProject, Guid.NewGuid());        
        }

        [Fact(DisplayName = @"DADO um Id válido
                            QUANDO existirem tarefas
                            ENTÃO deve retornar uma lista")]
        public async Task ExecuteAsync_ProjetoComTarefas_DeveRetornarListaDeTarefas()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();

            var tasks = new List<ProjectTask>
            {
                new ("Tarefa 1", "Descrição 1", DateTime.Today.AddDays(10),
                    ProjectTaskPriority.Low, _testParentProject.Id,
                    _testAssignedUser.Id),
                new ("Tarefa 2", "Descrição 2", DateTime.Today.AddDays(5),
                    ProjectTaskPriority.High, _testParentProject.Id,
                    _testAssignedUser.Id)
            };

            tasks[0].GetType().GetProperty("ExternalId")?.SetValue(tasks[0], Guid.NewGuid());
            tasks[0].GetType().GetProperty("Project")?.SetValue(tasks[0], _testParentProject);
            tasks[0].GetType().GetProperty("AssignedToUser")?.SetValue(tasks[0], _testAssignedUser);

            tasks[1].GetType().GetProperty("ExternalId")?.SetValue(tasks[1], Guid.NewGuid());
            tasks[1].GetType().GetProperty("Project")?.SetValue(tasks[1], _testParentProject);
            tasks[1].GetType().GetProperty("AssignedToUser")?.SetValue(tasks[1], _testAssignedUser);

            _mockProjectTaskDomainService
                .Setup(s => s.GetAllProjectTasksByProjectIdAsync(projectExternalId))
                .ReturnsAsync(tasks);

            // Act
            var result = await _getProjectTasksByProjectIdService.ExecuteAsync(projectExternalId);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(2);

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
