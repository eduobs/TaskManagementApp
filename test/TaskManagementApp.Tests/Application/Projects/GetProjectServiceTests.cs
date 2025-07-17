using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementApp.Application.Projects;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Interfaces;

namespace TaskManagementApp.Tests.Application.Projects
{
    public class GetProjectServiceTests
    {
        private readonly Mock<IProjectService> _mockProjectService;
        private readonly Mock<ILogger<GetProjectService>> _mockLogger;
        private readonly GetProjectService _getProjectService;

        public GetProjectServiceTests()
        {
            _mockProjectService = new Mock<IProjectService>();
            _mockLogger = new Mock<ILogger<GetProjectService>>();
            _getProjectService = new GetProjectService(_mockLogger.Object, _mockProjectService.Object);
        }

        [Fact(DisplayName = @"DADO uma requisicao para obter um projeto
                            QUANDO registro existir na base
                            ENTÃO deve retornar projeto")]
        public async Task GetProjectByExternalIdAsync_QuandoProjectForValido_RetornaProject()
        {
            // Arrange
            var externalId = Guid.NewGuid();
            var project = new Project("Nome", "Descrição", 1);
            project.GetType().GetProperty("ExternalId")?.SetValue(project, externalId);

            _mockProjectService
                .Setup(s => s.GetProjectByExternalIdAsync(externalId))
                .ReturnsAsync(project);

            // Act
            var result = await _getProjectService.GetProjectByExternalIdAsync(externalId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(externalId);
            result.Name.Should().Be(project.Name);
            result.Description.Should().Be(project.Description);
            
            _mockProjectService.Verify(s => s.GetProjectByExternalIdAsync(externalId), Times.Once());
        }

        [Fact(DisplayName = @"DADO uma requisicao para obter um projeto
                            QUANDO não localizar na base o registro
                            ENTÃO deve retornar null")]
        public async Task GetProjectByExternalIdAsync_QuandoProjectNaoEncontrado_RetornaNull()
        {
            // Arrange
            var externalId = Guid.NewGuid();

            _mockProjectService
                .Setup(s => s.GetProjectByExternalIdAsync(externalId))
                .ReturnsAsync((Project?)null);

            // Act
            var result = await _getProjectService.GetProjectByExternalIdAsync(externalId);

            // Assert
            result.Should().BeNull();

            _mockProjectService.Verify(s => s.GetProjectByExternalIdAsync(externalId), Times.Once());
        }
    }
}
