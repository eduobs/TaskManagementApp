using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementApp.Application.Projects;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Models.Projects;

namespace TaskManagementApp.Tests.Application.Projects
{
    public class GetAllProjectsServiceTests
    {
        private readonly Mock<IProjectService> _mockProjectService;
        private readonly Mock<ILogger<GetAllProjectsService>> _mockLogger;
        private readonly GetAllProjectsService _getAllProjectsService;

        public GetAllProjectsServiceTests()
        {
            _mockProjectService = new Mock<IProjectService>();
            _mockLogger = new Mock<ILogger<GetAllProjectsService>>();
            _getAllProjectsService = new GetAllProjectsService(_mockLogger.Object, _mockProjectService.Object);
        }

        [Fact(DisplayName = @"DADO uma solicitação para obter projetos
                            QUANDO existirem projetos
                            ENTÃO deve retornar uma lista de ProjectResponse corretamente")]
        public async Task ExecuteAsync_ProjetosExistem_DeveRetornarLista()
        {
            // Arrange
            var projects = new List<Project>
            {
                new("Projeto A", "Descrição A", 1),
                new("Projeto B", "Descrição B", 1)
            };

            projects[0].GetType().GetProperty("ExternalId")?.SetValue(projects[0], Guid.NewGuid());
            projects[1].GetType().GetProperty("ExternalId")?.SetValue(projects[1], Guid.NewGuid());

            _mockProjectService
                .Setup(s => s.GetAllProjectsAsync())
                .ReturnsAsync(projects);

            // Act
            var result = await _getAllProjectsService.ExecuteAsync();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(2);

            result.Should().ContainEquivalentOf(new ProjectResponse { Id = projects[0].ExternalId, Name = projects[0].Name, Description = projects[0].Description });
            result.Should().ContainEquivalentOf(new ProjectResponse { Id = projects[1].ExternalId, Name = projects[1].Name, Description = projects[1].Description });

            _mockProjectService.Verify(s => s.GetAllProjectsAsync(), Times.Once());
        }

        [Fact(DisplayName = @"DADO ama solicitação para obter projetos
                            QUANDO não existirem projetos
                            ENTÃO deve retornar uma lista vazia")]
        public async Task ExecuteAsync_NenhumProjetoExiste_DeveRetornarListaVazia()
        {
            // Arrange
            _mockProjectService
                .Setup(s => s.GetAllProjectsAsync())
                .ReturnsAsync([]);

            // Act
            var result = await _getAllProjectsService.ExecuteAsync();

            // Assert
            result.Should().NotBeNull().And.BeEmpty();
            result.Should().HaveCount(0);

            _mockProjectService.Verify(s => s.GetAllProjectsAsync(), Times.Once());
        }

        [Fact(DisplayName = @"DADO a solicitação para obter projetos
                            QUANDO o serviço lançar uma exceção
                            ENTÃO deve lançar a exceção")]
        public async Task ExecuteAsync_ServicoLancaExcecao_DevePropagarExcecao()
        {
            // Arrange
            var expectedException = new InvalidOperationException("Erro de conexão com o banco de dados.");
            _mockProjectService
                .Setup(s => s.GetAllProjectsAsync())
                .ThrowsAsync(expectedException);

            // Act
            Func<Task> act = async () => await _getAllProjectsService.ExecuteAsync();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                       .WithMessage(expectedException.Message);

            _mockProjectService.Verify(s => s.GetAllProjectsAsync(), Times.Once());
        }
    }
}
