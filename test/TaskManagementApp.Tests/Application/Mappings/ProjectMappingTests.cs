using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Application.Mappings;
using FluentAssertions;

namespace TaskManagementApp.Tests.Application.Mappings
{
    public class ProjectMappingTests
    {
        [Fact(DisplayName = @"DADO um objeto Project válido
                            QUANDO mapear para ProjectResponse
                            ENTÃO deve preencher o DTO corretamente")]
        public void ToDto_ProjectValido_DeveMapearComSucesso()
        {
            // Arrange
            var projectName = "Projeto Original";
            var projectDescription = "Descrição original do projeto.";
            var projectExternalId = Guid.NewGuid();

            var project = new Project(projectName, projectDescription);
            project.GetType().GetProperty("ExternalId")?.SetValue(project, projectExternalId);

            // Act
            var projectResponse = project.ToDto();

            // Assert
            projectResponse.Should().NotBeNull();
            projectResponse.Id.Should().Be(projectExternalId);
            projectResponse.Name.Should().Be(projectName);
            projectResponse.Description.Should().Be(projectDescription);
        }

        [Fact(DisplayName = @"DADO um objeto Project nulo
                            QUANDO mapear para ProjectResponse
                            ENTÃO deve lançar ArgumentNullException")]
        public void ToDto_ProjectNulo_DeveLancarArgumentNullException()
        {
            // Arrange
            Project? project = null;

            // Act & Assert
            Action act = () => project!.ToDto();

            act.Should().Throw<ArgumentNullException>()
               .WithMessage("Value cannot be null. (Parameter 'project')");
        }
    }
}