using System.ComponentModel.DataAnnotations;

namespace TaskManagementApp.Models.ProjectTasks
{
    public class AddCommentToTaskRequest
    {
        [Required(ErrorMessage = "O conteúdo do comentário é obrigatório.")]
        [StringLength(1000, ErrorMessage = "O comentário não pode exceder 1000 caracteres.")]
        public string CommentContent { get; set; } = string.Empty;
    }
}
