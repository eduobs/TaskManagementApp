using System.ComponentModel.DataAnnotations;

namespace TaskManagementApp.Models.ProjectTasks
{
    public class UpdateProjectTaskRequest
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
    }
}
