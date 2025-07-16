using System.ComponentModel.DataAnnotations;
using TaskManagementApp.Models.Enums;

namespace TaskManagementApp.Models.ProjectTasks
{
    public class CreateProjectTaskRequest
    {
        [Required(ErrorMessage = "O título da tarefa é obrigatório.")]
        [StringLength(255, ErrorMessage = "O título da tarefa não pode exceder 255 caracteres.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição da tarefa é obrigatória.")]
        [StringLength(1000, ErrorMessage = "A descrição da tarefa não pode exceder 1000 caracteres.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data limite da tarefa é obrigatória.")]
        [DataType(DataType.Date, ErrorMessage = "Formato de data inválido.")]
        public DateTime Deadline { get; set; }

        [Required(ErrorMessage = "A prioridade da tarefa é obrigatória.")]
        [EnumDataType(typeof(ProjectTaskPriority), ErrorMessage = "Prioridade da tarefa inválida.")]
        public ProjectTaskPriority Priority { get; set; }
    }
}