using FluentAssertions;
using Moq;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Domain.Services;

namespace TaskManagementApp.Tests.Domain.Services
{
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly ProjectService _projectService;

        public ProjectServiceTests()
        {
            _mockProjectRepository = new Mock<IProjectRepository>();
            _projectService = new ProjectService(_mockProjectRepository.Object);
        }

        [Fact(DisplayName = @"DADO um projeto válido
                            QUANDO criar um projeto com sucesso
                            ENTÃO deve adicionar o projeto e salvar as alterações")]
        public async Task CreateProjectAsync_ProjetoValido_DeveAdicionarProjetoESalvarAlteracoes()
        {
            // Arrange
            var projectName = "Novo Projeto de Teste";
            var projectDescription = "Descrição para o novo projeto de teste.";

            _mockProjectRepository
                .Setup(r => r.AddAsync(It.IsAny<Project>()))
                .Returns(Task.CompletedTask);

            _mockProjectRepository
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var createdProject = await _projectService.CreateProjectAsync(projectName, projectDescription);

            // Assert
            createdProject.Should().NotBeNull();
            createdProject.Name.Should().Be(projectName);
            createdProject.Description.Should().Be(projectDescription);
            createdProject.ExternalId.Should().NotBe(Guid.Empty);

            _mockProjectRepository.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Once());
            _mockProjectRepository.Verify(r => r.SaveChangesAsync(), Times.Once());
        }

        [Fact(DisplayName = @"DADO um Id válido
                            QUANDO localizar um projeto
                            ENTÃO deve retornar projeto")]
        public async Task GetProjectByExternalIdAsync_QuandoLocalizarProjeto_RetornaProjeto()
        {
            // Arrange
            var externalId = Guid.NewGuid();
            var project = new Project("Teste", "Descrição Teste");
            project.GetType().GetProperty("ExternalId")?.SetValue(project, externalId);

            _mockProjectRepository
                .Setup(r => r.GetByIdAsync(externalId))
                .ReturnsAsync(project);

            // Act
            var result = await _projectService.GetProjectByExternalIdAsync(externalId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(project);
        }

        [Fact(DisplayName = @"DADO um Id válido
                            QUANDO não localizar um projeto
                            ENTÃO deve retornar null")]
        public async Task GetProjectByExternalIdAsync_QuandoNaoLocalizarProjeto_RetornaNull()
        {
            // Arrange
            var externalId = Guid.NewGuid();
            _mockProjectRepository
                .Setup(r => r.GetByIdAsync(externalId))
                .ReturnsAsync((Project?)null);

            // Act
            var result = await _projectService.GetProjectByExternalIdAsync(externalId);

            // Assert
            result.Should().BeNull();
        }

        [Fact(DisplayName = @"DADO uma lista de projetos
                            QUANDO obter todos os projetos
                            ENTÃO deve retornar a lista de projetos")]
        public async Task GetAllProjectsAsync_QuandoExistiremProjetos_DeveRetornarListaDeProjetos()
        {
            // Arrange
            var projects = new List<Project>
            {
                new("Projeto 1", "Descrição 1"),
                new("Projeto 2", "Descrição 2")
            };

            _mockProjectRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(projects);

            // Act
            var result = await _projectService.GetAllProjectsAsync();

            // Assert
            result.Should().NotBeNull().And.HaveCount(2);
            result.Should().BeEquivalentTo(projects);
        }

        [Fact(DisplayName = @"DADO um Id válido
                            QUANDO atualizar projeto
                            ENTÃO deve retornar true")]
        public async Task UpdateProjectAsync_QuandoAtualizarComSucesso_RetornaTrue()
        {
            // Arrange
            var externalId = Guid.NewGuid();
            var existingProject = new Project("Nome", "Descrição");
            existingProject.GetType().GetProperty("ExternalId")?.SetValue(existingProject, externalId);

            var newName = "Novo nome";
            var newDescription = "Nova descrição";

            _mockProjectRepository
                .Setup(r => r.GetByIdAsync(externalId))
                .ReturnsAsync(existingProject);
            _mockProjectRepository.Setup(r => r.Update(It.IsAny<Project>()));
            _mockProjectRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _projectService.UpdateProjectAsync(externalId, newName, newDescription);

            // Assert
            result.Should().BeTrue();
            existingProject.Name.Should().Be(newName);
            existingProject.Description.Should().Be(newDescription);
            _mockProjectRepository.Verify(r => r.GetByIdAsync(externalId), Times.Once());
            _mockProjectRepository.Verify(r => r.Update(existingProject), Times.Once());
            _mockProjectRepository.Verify(r => r.SaveChangesAsync(), Times.Once());
        }

        [Fact(DisplayName = @"DADO um Id válido
                            QUANDO não localizar um projeto
                            ENTÃO retornar false")]
        public async Task UpdateProjectAsync_QuandoNaoLocalizarProjeto_RetornaFalse()
        {
            // Arrange
            var externalId = Guid.NewGuid();

            _mockProjectRepository
                .Setup(r => r.GetByIdAsync(externalId))
                .ReturnsAsync((Project?)null);

            // Act
            var result = await _projectService.UpdateProjectAsync(externalId, "Novo nome", "Nova descrição");

            // Assert
            result.Should().BeFalse();
            _mockProjectRepository.Verify(r => r.Update(It.IsAny<Project>()), Times.Never());
            _mockProjectRepository.Verify(r => r.SaveChangesAsync(), Times.Never());
        }

        [Fact(DisplayName = @"DADO um Id válido
                            QUANDO deletar projeto
                            ENTÃO deve retornar true")]
        public async Task DeleteProjectAsync_QuandoDeletarComSucesso_RetornaTrue()
        {
            // Arrange
            var externalId = Guid.NewGuid();
            var existingProject = new Project("Projeto para deletar", "Descrição do projeto");
            existingProject.GetType().GetProperty("ExternalId")?.SetValue(existingProject, externalId);

            _mockProjectRepository
                .Setup(r => r.GetByIdAsync(externalId))
                .ReturnsAsync(existingProject);
            _mockProjectRepository.Setup(r => r.Delete(It.IsAny<Project>()));
            _mockProjectRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _projectService.DeleteProjectAsync(externalId);

            // Assert
            result.Should().BeTrue();
            _mockProjectRepository.Verify(r => r.GetByIdAsync(externalId), Times.Once());
            _mockProjectRepository.Verify(r => r.Delete(existingProject), Times.Once());
            _mockProjectRepository.Verify(r => r.SaveChangesAsync(), Times.Once());
        }

        [Fact(DisplayName = @"DADO um Id válido
                            QUANDO não encontrar projeto
                            ENTÃO deve retornar false")]
        public async Task DeleteProjectAsync_QuandoNaoEncontrarProjeto_RetornaFalse()
        {
            // Arrange
            var externalId = Guid.NewGuid();

            _mockProjectRepository
                .Setup(r => r.GetByIdAsync(externalId))
                .ReturnsAsync((Project?)null);

            // Act
            var result = await _projectService.DeleteProjectAsync(externalId);

            // Assert
            result.Should().BeFalse();
            _mockProjectRepository.Verify(r => r.Delete(It.IsAny<Project>()), Times.Never());
            _mockProjectRepository.Verify(r => r.SaveChangesAsync(), Times.Never());
        }
    }
}
