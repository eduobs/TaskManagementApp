using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Application.Mappings;
using FluentAssertions;
using TaskManagementApp.Domain.Enums;

namespace TaskManagementApp.Tests.Application.Mappings
{
    public class ProjectTaskMappingTests
    {
        [Fact(DisplayName = @"DADO um projeto válido
                            QUANDO mapear para ProjectTaskResponse
                            ENTÃO deve retornar o DTO corretamente")]
        public void ToDto_ProjetoValido_DeveMapearCorretamente()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var projectIdOriginal = Guid.NewGuid();
            var project = new Project("Projeto Pai", "Descrição do Projeto Pai", 1);
            var assignedUserExternalId = Guid.NewGuid();
            var assignedUserName = "Responsavel Teste";

            var assignedUser = new User(assignedUserName, UserRole.Basic);
            assignedUser.GetType().GetProperty("ExternalId")?.SetValue(assignedUser, assignedUserExternalId);
            assignedUser.GetType().GetProperty("Id")?.SetValue(assignedUser, 1);

            project.GetType().GetProperty("ExternalId")?.SetValue(project, projectIdOriginal);
            project.GetType().GetProperty("Id")?.SetValue(project, 1);

            var task = new ProjectTask(
                "Título da Tarefa",
                "Descrição da Tarefa",
                DateTime.Today.AddDays(10),
                ProjectTaskPriority.High,
                project.Id,
                assignedUser.Id
            );

            task.GetType().GetProperty("ExternalId")?.SetValue(task, taskExternalId);
            task.GetType().GetProperty("Project")?.SetValue(task, project);
            task.GetType().GetProperty("AssignedToUser")?.SetValue(task, assignedUser);

            // Act
            var taskResponse = task.ToDto();

            // Assert
            taskResponse.Should().NotBeNull();
            taskResponse.Id.Should().Be(taskExternalId);
            taskResponse.Title.Should().Be(task.Title);
            taskResponse.Description.Should().Be(task.Description);
            taskResponse.Deadline.Should().Be(task.Deadline);

            taskResponse.Priority.Should().Be((Models.Enums.ProjectTaskPriority)task.Priority);
            taskResponse.Status.Should().Be((Models.Enums.ProjectTaskStatus)task.Status);
            taskResponse.ProjectId.Should().Be(projectIdOriginal);
            taskResponse.AssignedToUserId.Should().Be(assignedUserExternalId);
            taskResponse.AssignedToUserName.Should().Be(assignedUserName);
        }

        [Fact(DisplayName = @"DADO uma tarefa nula
                            QUANDO tentar mapear
                            ENTÃO deve lançar ArgumentNullException")]
        public void ToDto_TarefaNull_DeveLancarArgumentNullException()
        {
            // Arrange
            ProjectTask? projectTask = null;

            // Act & Assert
            Action act = () => projectTask!.ToDto();

            act.Should().Throw<ArgumentNullException>()
               .WithMessage("Value cannot be null. (Parameter 'projectTask')");
        }

        [Fact(DisplayName = @"DADO uma tarefa válido SEM Project e AssignedToUser
                            QUANDO tentar mapear para
                            ENTÃO deve preencher o DTO
                            E IDs/nomes devem ser Guid.Empty/padrão")]
        public void ToDto_ComTaskValidoSemAssociacoes_DeveMapearCorretamenteComValoresPadrao()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var task = new ProjectTask(
                "Tarefa Sem Associações",
                "Tarefa com associações nulas para teste de mapeamento",
                DateTime.Today.AddDays(5),
                ProjectTaskPriority.Low,
                999,
                998
            );

            task.GetType().GetProperty("ExternalId")?.SetValue(task, taskExternalId);

            // Act
            var taskResponse = task.ToDto();

            // Assert
            taskResponse.Should().NotBeNull();
            taskResponse.Id.Should().Be(taskExternalId);
            taskResponse.ProjectId.Should().Be(Guid.Empty);
            taskResponse.AssignedToUserId.Should().Be(Guid.Empty);
            taskResponse.AssignedToUserName.Should().Be("Usuário Desconhecido");
        }
    }
}
