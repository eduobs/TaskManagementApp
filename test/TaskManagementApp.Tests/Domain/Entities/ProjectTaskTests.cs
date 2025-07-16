using FluentAssertions;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Enums;

namespace TaskManagementApp.Tests.Domain.Entities
{
    public class ProjectTaskTests
    {
        [Fact(DisplayName = @"DADO dados válidos
                            QUANDO inicializar uma nova tarefa
                            ENTÃO deve preencher os dados corretamente
                            E definir status Pendente")]
        public void ProjectTask_ConstrutorComDadosValidos_DevePreencherDadosEStatusPendente()
        {
            // Arrange
            var title = "Título da Tarefa";
            var description = "Descrição da tarefa.";
            var deadline = DateTime.Today.AddDays(7);
            var priority = ProjectTaskPriority.Medium;
            var projectId = 1;

            // Act
            var task = new ProjectTask(title, description, deadline, priority, projectId);

            // Assert
            task.Should().NotBeNull();
            task.Title.Should().Be(title);
            task.Description.Should().Be(description);
            task.Deadline.Should().Be(deadline);
            task.Priority.Should().Be(priority);
            task.Status.Should().Be(ProjectTaskStatus.Pending);
            task.ProjectId.Should().Be(projectId);
            task.ExternalId.Should().NotBe(Guid.Empty);
        }

        [Theory(DisplayName = @"DADO um título inválido
                                QUANDO inicializar uma nova tarefa
                                ENTÃO deve lançar ArgumentException")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ProjectTask_ConstrutorComTituloInvalido_DeveLancarArgumentException(string invalidTitle)
        {
            // Arrange
            var description = "Descrição válida.";
            var deadline = DateTime.Today.AddDays(7);
            var priority = ProjectTaskPriority.Medium;
            var projectId = 1;

            // Act & Assert
            Action act = () => new ProjectTask(invalidTitle, description, deadline, priority, projectId);

            act.Should().Throw<ArgumentException>()
               .WithMessage("O título da tarefa não pode ser nulo ou vazio.*");
        }

        [Theory(DisplayName = @"DADO uma descrição de tarefa inválida
                                QUANDO inicializar nova tarefa
                                ENTÃO deve lançar um ArgumentException")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ProjectTask_ConstrutorComDescricaoInvalida_DeveLancarArgumentException(string invalidDescription)
        {
            // Arrange
            var title = "Título Válido";
            var deadline = DateTime.Today.AddDays(7);
            var priority = ProjectTaskPriority.Medium;
            var projectId = 1;

            // Act & Assert
            Action act = () => new ProjectTask(title, invalidDescription, deadline, priority, projectId);

            act.Should().Throw<ArgumentException>()
               .WithMessage("A descrição da tarefa não pode ser nula ou vazia.*");
        }

        [Fact(DisplayName = @"DADO uma data no passado
                            QUANDO inicializar nova tarefa
                            ENTÃO deve lançar ArgumentException")]
        public void ProjectTask_ConstrutorComDataNoPassado_DeveLancarArgumentException()
        {
            // Arrange
            var title = "Título Válido";
            var description = "Descrição Válida.";
            var deadline = DateTime.Today.AddDays(-1); // Data no passado
            var priority = ProjectTaskPriority.Low;
            var projectId = 1;

            // Act & Assert
            Action act = () => new ProjectTask(title, description, deadline, priority, projectId);

            act.Should().Throw<ArgumentException>()
               .WithMessage("A data de vencimento não pode ser no passado.*");
        }

        [Theory(DisplayName = @"DADO um id de projeto inválido
                                QUANDO inicializar uma tarefa
                                ENTÃO deve lançar uma ArgumentException")]
        [InlineData(0)]
        [InlineData(-1)]
        public void ProjectTask_ConstrutorComProjectIdInvalido_DeveLancarArgumentException(int invalidProjectId)
        {
            // Arrange
            var title = "Título Válido";
            var description = "Descrição Válida.";
            var deadline = DateTime.Today.AddDays(7);
            var priority = ProjectTaskPriority.Low;

            // Act & Assert
            Action act = () => new ProjectTask(title, description, deadline, priority, invalidProjectId);

            act.Should().Throw<ArgumentException>()
               .WithMessage("O ID do projeto deve ser válido.*");
        }

        [Fact(DisplayName = @"DADO que os novos detalhes da tarefa são válidos
                            QUANDO atualizar os detalhes
                            ENTÃO deve atualizar corretamente")]
        public void UpdateDetails_ComDadosValidos_DeveAtualizarPropriedadesCorretamente()
        {
            // Arrange
            var task = new ProjectTask("Título", "Descrição", DateTime.Today.AddDays(1), ProjectTaskPriority.Low, 1);
            var newTitle = "Novo Título";
            var newDescription = "Nova Descrição";
            var newDeadline = DateTime.Today.AddDays(14);

            // Act
            task.UpdateDetails(newTitle, newDescription, newDeadline);

            // Assert
            task.Title.Should().Be(newTitle);
            task.Description.Should().Be(newDescription);
            task.Deadline.Should().Be(newDeadline);

            task.Priority.Should().Be(ProjectTaskPriority.Low);
        }

        [Theory(DisplayName = @"DADO um novo título inválido
                                QUANDO tentar atualizar os detalhes
                                ENTÃO deve lançar uma ArgumentException")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void UpdateDetails_ComTituloInvalido_DeveLancarArgumentException(string invalidTitle)
        {
            // Arrange
            var task = new ProjectTask("Título", "Descrição", DateTime.Today.AddDays(1), ProjectTaskPriority.Low, 1);
            var oldDescription = task.Description;
            var oldDeadline = task.Deadline;

            // Act & Assert
            Action act = () => task.UpdateDetails(invalidTitle, "Nova descriçao", DateTime.Today);

            act.Should().Throw<ArgumentException>()
               .WithMessage("O título da tarefa não pode ser nulo ou vazio.*");

            task.Description.Should().Be(oldDescription);
            task.Deadline.Should().Be(oldDeadline);
        }

        [Theory(DisplayName = @"DADO uma nova descrição inválida
                                QUANDO tentar atualizar os detalhes
                                ENTÃO deve lançar uma ArgumentException")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void UpdateDetails_ComDescricaoInvalida_DeveLancarArgumentException(string invalidDescription)
        {
            // Arrange
            var task = new ProjectTask("Título", "Descrição", DateTime.Today.AddDays(1), ProjectTaskPriority.Low, 1);
            var oldTitle = task.Title;
            var oldDeadline = task.Deadline;

            // Act & Assert
            Action act = () => task.UpdateDetails("Novo Título", invalidDescription, DateTime.Today);

            act.Should().Throw<ArgumentException>()
               .WithMessage("A descrição da tarefa não pode ser nula ou vazia.*");

            task.Title.Should().Be(oldTitle);
            task.Deadline.Should().Be(oldDeadline);
        }

        [Fact(DisplayName = @"DADO uma nova data no passado
                            QUANDO tentar atualizar os detalhes
                            ENTÃO deve lançar uma ArgumentException")]
        public void UpdateDetails_ComDeadlineNoPassado_NaoConcluida_DeveLancarArgumentException()
        {
            // Arrange
            var task = new ProjectTask("Título", "Descrição", DateTime.Today.AddDays(1), ProjectTaskPriority.Low, 1);
            task.UpdateStatus(ProjectTaskStatus.InProgress);

            var newDeadline = DateTime.Today.AddDays(-1);

            // Act & Assert
            Action act = () => task.UpdateDetails("Novo título", "Nova descrição", newDeadline);

            act.Should().Throw<ArgumentException>()
               .WithMessage("A data de vencimento não pode ser no passado.*");
        }

        [Fact(DisplayName = @"DADO um novo status válido
                            QUANDO atualizar o status
                            ENTÃO deve atualizar corretamente")]
        public void UpdateStatus_NovoStatusValido_DeveAtualizarStatusCorretamente()
        {
            // Arrange
            var task = new ProjectTask("Título", "Descrição", DateTime.Today.AddDays(1), ProjectTaskPriority.Low, 1);
            var newStatus = ProjectTaskStatus.Completed;

            // Act
            task.UpdateStatus(newStatus);

            // Assert
            task.Status.Should().Be(newStatus);
        }
    }
}
