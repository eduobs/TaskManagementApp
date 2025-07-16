using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementApp.Application.ProjectTasks;
using TaskManagementApp.Domain.Interfaces;

namespace TaskManagementApp.Tests.Application.ProjectTasks
{
    public class DeleteProjectTaskServiceTests
    {
        private readonly Mock<IProjectTaskService> _mockProjectTaskDomainService;
        private readonly Mock<ILogger<DeleteProjectTaskService>> _mockLogger;
        private readonly DeleteProjectTaskService _deleteProjectTaskService;

        public DeleteProjectTaskServiceTests()
        {
            _mockProjectTaskDomainService = new Mock<IProjectTaskService>();
            _mockLogger = new Mock<ILogger<DeleteProjectTaskService>>();

            _deleteProjectTaskService = new DeleteProjectTaskService(
                _mockLogger.Object,
                _mockProjectTaskDomainService.Object
            );
        }

        [Fact(DisplayName = @"DADO um id de tarefa existente
                            QUANDO o serviço excluir a tarefa com sucesso
                            ENTÃO deve retornar true")]
        public async Task ExecuteAsync_TarefaExiste_DeveRetornarTrue()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();

            _mockProjectTaskDomainService
                .Setup(s => s.DeleteProjectTaskAsync(taskExternalId))
                .ReturnsAsync(true);

            // Act
            var result = await _deleteProjectTaskService.ExecuteAsync(taskExternalId);

            // Assert
            result.Should().BeTrue();

            _mockProjectTaskDomainService.Verify(s => s.DeleteProjectTaskAsync(taskExternalId), Times.Once());
        }

        [Fact(DisplayName = @"DADO um id de tarefa inexistente
                            QUANDO o serviço não encontrar a tarefa
                            ENTÃO deve retornar false")]
        public async Task ExecuteAsync_TarefaNaoEncontrada_DeveRetornarFalse()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();

            _mockProjectTaskDomainService
                .Setup(s => s.DeleteProjectTaskAsync(taskExternalId))
                .ReturnsAsync(false);

            // Act
            var result = await _deleteProjectTaskService.ExecuteAsync(taskExternalId);

            // Assert
            result.Should().BeFalse();

            _mockProjectTaskDomainService.Verify(s => s.DeleteProjectTaskAsync(taskExternalId), Times.Once());
        }

        [Fact(DisplayName = @"DADO uma requisição de exclusão de tarefa
                            QUANDO o serviço lançar uma exceção
                            ENTÃO deve retornar exceção")]
        public async Task ExecuteAsync_ServicoLancaExcecao_DeveRetornarExcecao()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var expectedException = new InvalidOperationException("Erro de banco de dados durante a exclusão.");

            _mockProjectTaskDomainService
                .Setup(s => s.DeleteProjectTaskAsync(taskExternalId))
                .ThrowsAsync(expectedException);

            // Act
            Func<Task> act = async () => await _deleteProjectTaskService.ExecuteAsync(taskExternalId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                       .WithMessage(expectedException.Message);

            _mockProjectTaskDomainService.Verify(s => s.DeleteProjectTaskAsync(taskExternalId), Times.Once());
        }
    }
}
