using FluentAssertions;
using TaskManagementApp.Domain.Entities;

namespace TaskManagementApp.Tests.Domain.Entities
{
    public class ProjectTests
    {
        [Fact(DisplayName = @"DADO que os dados são válido
                            QUANDO inicializar um novo projeto
                            ENTÃO deve preencher os dados corretamente")]
        public void Project_ContrutorComDadosValidos_DevePreencherOsDadosCorretamente()
        {
            // Arrange
            var name = "Novo Projeto";
            var description = "Descrição do meu projeto de teste.";

            // Act
            var project = new Project(name, description);

            // Assert
            project.Should().NotBeNull();
            project.Name.Should().Be(name);
            project.Description.Should().Be(description);
            project.ExternalId.Should().NotBe(Guid.Empty);
        }

        [Theory(DisplayName = @"DADO um nome inválido
                                QUANDO inicializar um novo projeto
                                ENTÃO deve lançar uma exceção")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Project_ConstrutorComNomeInvalido_DeveLancarExcecao(string invalidName)
        {
            // Arrange
            var description = "Qualquer descrição.";

            // Act & Assert
            Action act = () => new Project(invalidName, description);

            act.Should().Throw<ArgumentException>()
               .WithMessage("O nome do projeto não pode ser nulo ou vazio.*");
        }

        [Theory(DisplayName = @"DADO uma descrição inválida
                                QUANDO inicializar um novo projeto
                                ENTÃO deve lançar uma exceção")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Project_ConstrutorComDescricaoInvalida_DeveLancarExcecao(string invalidDescription)
        {
            // Arrange
            var name = "Nome Válido";

            // Act & Assert
            Action act = () => new Project(name, invalidDescription);

            act.Should().Throw<ArgumentException>()
               .WithMessage("A descrição do projeto não pode ser nula ou vazia.*");
        }

        [Fact(DisplayName = @"DADO uma atualização de nome
                            QUANDO for um dado válido
                            ENTÃO deve atualizar os dados corretamente")]
        public void UpdateName_QuandoDadosValidos_DeveAtualizarOsDadosCorretamente()
        {
            // Arrange
            var project = new Project("Old Name", "Old Description");
            var newName = "Novo Nome do Projeto";

            // Act
            project.UpdateName(newName);

            // Assert
            project.Name.Should().Be(newName);
        }

        [Theory(DisplayName = @"DADO um nome inválido
                                QUANDO tentar atualizar o nome
                                ENTÃO deve retornar exception")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void UpdateName_QuandoNomeInvalido_DeveRetornarException(string invalidName)
        {
            // Arrange
            var project = new Project("Existing Name", "Existing Description");

            // Act & Assert
            Action act = () => project.UpdateName(invalidName);

            act.Should().Throw<ArgumentException>()
               .WithMessage("O nome do projeto não pode ser nulo ou vazio.*");
        }

        [Fact(DisplayName = @"DADO uma atualização de descrição
                            QUANDO for um dado válido
                            ENTÃO deve atualizar os dados corretamente")]
        public void UpdateDescription_QuandoDadosValidos_DeveAtualizarOsDadosCorretamente()
        {
            // Arrange
            var project = new Project("Project Name", "Old Description");
            var newDescription = "Nova Descrição do Projeto.";

            // Act
            project.UpdateDescription(newDescription);

            // Assert
            project.Description.Should().Be(newDescription);
        }

        [Theory(DisplayName = @"DADO uma descrição inválida
                                QUANDO tentar atualizar a descrição
                                ENTÃO deve retornar exception")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void UpdateDescription_QuandoDescricaoInvalida_DeveRetornarException(string invalidDescription)
        {
            // Arrange
            var project = new Project("Existing Name", "Existing Description");

            // Act & Assert
            Action act = () => project.UpdateDescription(invalidDescription);

            act.Should().Throw<ArgumentException>()
               .WithMessage("A descrição do projeto não pode ser nula ou vazia.*");
        }
    }
}
