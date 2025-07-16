using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Application.Mappings;
using FluentAssertions;

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
            var project = new Project("Projeto Pai", "Descrição do Projeto Pai");

            project.GetType().GetProperty("ExternalId")?.SetValue(project, projectIdOriginal);

            var task = new ProjectTask(
                "Título da Tarefa",
                "Descrição da Tarefa",
                DateTime.Today.AddDays(10),
                TaskManagementApp.Domain.Enums.ProjectTaskPriority.High,
                1
            );

            task.GetType().GetProperty("ExternalId")?.SetValue(task, taskExternalId);
            task.GetType().GetProperty("Project")?.SetValue(task, project);

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
    }
}
