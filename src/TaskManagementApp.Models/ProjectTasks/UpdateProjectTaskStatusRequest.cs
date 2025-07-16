using System.ComponentModel.DataAnnotations;
using TaskManagementApp.Models.Enums;

namespace TaskManagementApp.Models.ProjectTasks
{
    public class UpdateProjectTaskStatusRequest
    {
        [Required(ErrorMessage = "O status da tarefa é obrigatório.")]
        [EnumDataType(typeof(ProjectTaskStatus), ErrorMessage = "Status da tarefa inválido.")]
        public ProjectTaskStatus Status { get; set; }
    }
}
