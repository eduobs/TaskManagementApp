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

            var expectedExternalId = Guid.NewGuid();
            var domainProject = new Project(request.Name, request.Description);

            domainProject.GetType().GetProperty("ExternalId")?.SetValue(domainProject, expectedExternalId);

            _mockProjectService
                .Setup(s => s.CreateProjectAsync(request.Name, request.Description))
                .ReturnsAsync(domainProject);

            // Act
            var result = await _createProjectService.ExecuteAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedExternalId);
            result.Name.Should().Be(request.Name);
            result.Description.Should().Be(request.Description);

            _mockProjectService.Verify(s => s.CreateProjectAsync(request.Name, request.Description), Times.Once());
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

            _mockProjectService
                .Setup(s => s.CreateProjectAsync(request.Name, request.Description))
                .ThrowsAsync(new ArgumentException("O nome do projeto não pode ser nulo ou vazio.", nameof(request.Name)));

            // Act & Assert
            Func<Task> act = async () => await _createProjectService.ExecuteAsync(request);

            await act.Should().ThrowAsync<ArgumentException>()
                       .WithMessage("O nome do projeto não pode ser nulo ou vazio.*");

            _mockProjectService.Verify(s => s.CreateProjectAsync(request.Name, request.Description), Times.Once());
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

            _mockProjectService
                .Setup(s => s.CreateProjectAsync(request.Name, request.Description))
                .ThrowsAsync(new InvalidOperationException("Erro inesperado."));

            // Act & Assert
            Func<Task> act = async () => await _createProjectService.ExecuteAsync(request);

            await act.Should().ThrowAsync<InvalidOperationException>()
                       .WithMessage("Erro inesperado.");

            _mockProjectService.Verify(s => s.CreateProjectAsync(request.Name, request.Description), Times.Once());
        }
    }
}
