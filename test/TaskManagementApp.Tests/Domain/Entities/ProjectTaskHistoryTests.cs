using FluentAssertions;
using TaskManagementApp.Domain.Entities;

namespace TaskManagementApp.Tests.Domain.Entities
{
    public class ProjectTaskHistoryTests
    {
        [Fact(DisplayName = @"DADO que os dados são válidos
                            QUANDO criar um registro
                            ENTÃO deve preencher os dados corretamente")]
        public void Create_ComDadosValidos_DeveCriarRegistroCorretamente()
        {
            // Arrange
            var projectTaskId = 1;
            var propertyName = "Status";
            var oldValue = "Pending";
            var newValue = "Completed";
            var modifiedByUserId = Guid.NewGuid();
            var changeType = "Status Change";

            // Act
            var historyEntry = ProjectTaskHistory.Create(projectTaskId, propertyName, oldValue, newValue, modifiedByUserId, changeType);

            // Assert
            historyEntry.Should().NotBeNull();
            historyEntry.ProjectTaskId.Should().Be(projectTaskId);
            historyEntry.PropertyName.Should().Be(propertyName);
            historyEntry.OldValue.Should().Be(oldValue);
            historyEntry.NewValue.Should().Be(newValue);
            historyEntry.ModifiedByUserId.Should().Be(modifiedByUserId);
            historyEntry.ChangeType.Should().Be(changeType);
            historyEntry.ModificationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            historyEntry.Id.Should().Be(0);
        }

        [Fact(DisplayName = @"DADO que os dados são válidos sem tipo de mudança
                            QUANDO criar um registro
                            ENTÃO deve preencher o tipo de mudança como 'Update' por padrão")]
        public void Create_ComDadosValidosSemChangeType_DeveDefinirChangeTypeComoUpdate()
        {
            // Arrange
            var projectTaskId = 1;
            var propertyName = "Título";
            var oldValue = "Título antigo";
            var newValue = "Novo título";
            var modifiedByUserId = Guid.NewGuid();

            // Act
            var historyEntry = ProjectTaskHistory.Create(projectTaskId, propertyName, oldValue, newValue, modifiedByUserId);

            // Assert
            historyEntry.Should().NotBeNull();
            historyEntry.ChangeType.Should().Be("Update");
        }

        [Theory(DisplayName = @"DADO um id de tarefa interna inválido
                                QUANDO criar um registro
                                ENTÃO deve lançar uma ArgumentException")]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_ComTaskIdInvalido_DeveLancarArgumentException(int invalidProjectTaskId)
        {
            // Arrange
            var propertyName = "Status";
            var oldValue = "Pending";
            var newValue = "Completed";
            var modifiedByUserId = Guid.NewGuid();

            // Act & Assert
            Action act = () => ProjectTaskHistory.Create(invalidProjectTaskId, propertyName, oldValue, newValue, modifiedByUserId);

            act.Should().Throw<ArgumentException>()
               .WithMessage("O id interno da tarefa é inválido.*");
        }

        [Theory(DisplayName = @"DADO um nome de propriedade inválida
                                QUANDO criar um registro
                                ENTÃO deve lançar uma ArgumentException")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Create_ComNomeInvalido_DeveLancarArgumentException(string invalidPropertyName)
        {
            // Arrange
            var projectTaskId = 1;
            var oldValue = "Pending";
            var newValue = "Completed";
            var modifiedByUserId = Guid.NewGuid();

            // Act & Assert
            Action act = () => ProjectTaskHistory.Create(projectTaskId, invalidPropertyName, oldValue, newValue, modifiedByUserId);

            act.Should().Throw<ArgumentException>()
               .WithMessage("O nome da propriedade modificada é obrigatório.*");
        }

        [Fact(DisplayName = @"DADO um id de usuário vazio
                            QUANDO criar um registro
                            ENTÃO deve lançar uma ArgumentException")]
        public void Create_ComUserIdVazio_DeveLancarArgumentException()
        {
            // Arrange
            var projectTaskId = 1;
            var propertyName = "Status";
            var oldValue = "Pending";
            var newValue = "Completed";
            var modifiedByUserId = Guid.Empty;

            // Act & Assert
            Action act = () => ProjectTaskHistory.Create(projectTaskId, propertyName, oldValue, newValue, modifiedByUserId);

            act.Should().Throw<ArgumentException>()
               .WithMessage("O id do usuário modificador é obrigatório.*");
        }
    }
}
