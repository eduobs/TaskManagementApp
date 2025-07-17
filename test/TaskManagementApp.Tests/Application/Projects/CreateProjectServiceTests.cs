using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementApp.Application.Projects;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Models.Projects;

namespace TaskManagementApp.Tests.Application.Projects
{
    public class CreateProjectServiceTests
    {
        private readonly Mock<IProjectService> _mockProjectService;
        private readonly Mock<ILogger<CreateProjectService>> _mockLogger;
        private readonly CreateProjectService _createProjectService;

        public CreateProjectServiceTests()
        {
            _mockProjectService = new Mock<IProjectService>();
            _mockLogger = new Mock<ILogger<CreateProjectService>>();
            _createProjectService = new CreateProjectService(_mockLogger.Object, _mockProjectService.Object);
        }

        [Fact(DisplayName = @"DADO uma requisição de criação de projeto válida
                            QUANDO o serviço de domínio criar o projeto com sucesso
                            ENTÃO deve retornar um ProjectResponse mapeado corretamente")]
        public async Task ExecuteAsync_RequisicaoValida_DeveRetornarProjectResponse()
        {
            // Arrange
            var request = new CreateProjectRequest
            {
                Name = "Projeto de Teste",
                Description = "Descrição do projeto de teste na camada de aplicação."
            };
            var userId = Guid.NewGuid();

            var expectedExternalId = Guid.NewGuid();
            var project = new Project(request.Name, request.Description, 1);

            project.GetType().GetProperty("ExternalId")?.SetValue(project, expectedExternalId);

            _mockProjectService
                .Setup(s => s.CreateProjectAsync(request.Name, request.Description, userId))
                .ReturnsAsync(project);

            // Act
            var result = await _createProjectService.ExecuteAsync(request, userId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedExternalId);
            result.Name.Should().Be(request.Name);
            result.Description.Should().Be(request.Description);

            _mockProjectService.Verify(s => s.CreateProjectAsync(request.Name, request.Description, userId), Times.Once());
        }

        [Fact(DisplayName = @"DADO uma requisição de criação de projeto inválida
                            QUANDO o serviço lançar ArgumentException
                            ENTÃO deve lançar ArgumentException")]
        public async Task ExecuteAsync_RequisicaoInvalida_DevePropagarArgumentException()
        {
            // Arrange
            var request = new CreateProjectRequest
            {
                Name = "",
                Description = "Descrição válida."
            };
            var userId = Guid.NewGuid();

            _mockProjectService
                .Setup(s => s.CreateProjectAsync(request.Name, request.Description, userId))
                .ThrowsAsync(new ArgumentException("O nome do projeto não pode ser nulo ou vazio.", nameof(request.Name)));

            // Act & Assert
            Func<Task> act = async () => await _createProjectService.ExecuteAsync(request, userId);

            await act.Should().ThrowAsync<ArgumentException>()
                       .WithMessage("O nome do projeto não pode ser nulo ou vazio.*");

            _mockProjectService.Verify(s => s.CreateProjectAsync(request.Name, request.Description, userId), Times.Once());
        }

        [Fact(DisplayName = @"DADO uma requisição de criação de projeto
                            QUANDO o serviço de domínio lançar uma exceção genérica
                            ENTÃO deve lançar a exceção")]
        public async Task ExecuteAsync_QuandoServicoRetornarExcecao_DeveRetornarExcecao()
        {
            // Arrange
            var request = new CreateProjectRequest
            {
                Name = "Projeto X",
                Description = "Descrição Y"
            };
            var userId = Guid.NewGuid();

            _mockProjectService
                .Setup(s => s.CreateProjectAsync(request.Name, request.Description, userId))
                .ThrowsAsync(new InvalidOperationException("Erro inesperado."));

            // Act & Assert
            Func<Task> act = async () => await _createProjectService.ExecuteAsync(request, userId);

            await act.Should().ThrowAsync<InvalidOperationException>()
                       .WithMessage("Erro inesperado.");

            _mockProjectService.Verify(s => s.CreateProjectAsync(request.Name, request.Description, userId), Times.Once());
        }
    }
}
