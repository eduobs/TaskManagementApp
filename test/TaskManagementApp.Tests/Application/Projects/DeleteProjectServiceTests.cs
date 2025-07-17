using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementApp.Application.Projects;
using TaskManagementApp.Domain.Interfaces;

namespace TaskManagementApp.Tests.Application.Projects
{
    public class DeleteProjectServiceTests
    {
        private readonly Mock<IProjectService> _mockProjectDomainService;
        private readonly Mock<ILogger<DeleteProjectService>> _mockLogger;
        private readonly DeleteProjectService _deleteProjectService;

        public DeleteProjectServiceTests()
        {
            _mockProjectDomainService = new Mock<IProjectService>();
            _mockLogger = new Mock<ILogger<DeleteProjectService>>();

            _deleteProjectService = new DeleteProjectService(
                _mockProjectDomainService.Object,
                _mockLogger.Object
            );
        }

        [Fact(DisplayName = @"DADO um id válido
                            QUANDO excluir o projeto com sucesso
                            ENTÃO deve retornar true")]
        public async Task ExecuteAsync_ProjetoExiste_DeveRetornarTrue()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();

            _mockProjectDomainService
                .Setup(s => s.DeleteProjectAsync(projectExternalId))
                .ReturnsAsync(true);

            // Act
            var result = await _deleteProjectService.ExecuteAsync(projectExternalId);

            // Assert
            result.Should().BeTrue();

            _mockProjectDomainService.Verify(s => s.DeleteProjectAsync(projectExternalId), Times.Once());
        }

        [Fact(DisplayName = @"DADO um id de projeto inexistente
                            QUANDO tentar excluir
                            ENTÃO deve retornar false")]
        public async Task ExecuteAsync_ProjetoNaoEncontrado_DeveRetornarFalse()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();

            _mockProjectDomainService
                .Setup(s => s.DeleteProjectAsync(projectExternalId))
                .ReturnsAsync(false);

            // Act
            var result = await _deleteProjectService.ExecuteAsync(projectExternalId);

            // Assert
            result.Should().BeFalse();

            _mockProjectDomainService.Verify(s => s.DeleteProjectAsync(projectExternalId), Times.Once());
        }

        [Fact(DisplayName = @"DADO um id de projeto existente com tarefas pendentes
                            QUANDO o serviço lançar InvalidOperationException
                            ENTÃO deve retornar exception")]
        public async Task ExecuteAsync_ComTarefasPendentes_DeveRetornarException()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();
            var exception = new InvalidOperationException("Não é possível remover o projeto. Ainda há tarefas pendentes associadas a ele. Conclua ou remova as tarefas primeiro.");

            _mockProjectDomainService
                .Setup(s => s.DeleteProjectAsync(projectExternalId))
                .ThrowsAsync(exception);

            // Act
            Func<Task> act = async () => await _deleteProjectService.ExecuteAsync(projectExternalId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(exception.Message);

            _mockProjectDomainService.Verify(s => s.DeleteProjectAsync(projectExternalId), Times.Once());
        }

        [Fact(DisplayName = @"DADO uma requisição
                            QUANDO o serviço lançar uma exceção genérica
                            ENTÃO deve lançar exceção")]
        public async Task ExecuteAsync_ServicoLancaExcecaoGenerica_DeveRetornarExcecao()
        {
            // Arrange
            var projectExternalId = Guid.NewGuid();
            var expectedException = new Exception("Erro inesperado durante a exclusão do projeto.");

            _mockProjectDomainService
                .Setup(s => s.DeleteProjectAsync(projectExternalId))
                .ThrowsAsync(expectedException);

            // Act
            Func<Task> act = async () => await _deleteProjectService.ExecuteAsync(projectExternalId);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                       .WithMessage(expectedException.Message);

            _mockProjectDomainService.Verify(s => s.DeleteProjectAsync(projectExternalId), Times.Once());
        }
    }
}
