using System.ComponentModel.DataAnnotations;

namespace TaskManagementApp.Models.Projects
{
    public class CreateProjectRequest
    {
        [Required(ErrorMessage = "O nome do projeto é obrigatório.")]
        [StringLength(255, ErrorMessage = "O nome do projeto não pode exceder 255 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição do projeto é obrigatória.")]
        [StringLength(1000, ErrorMessage = "A descrição do projeto não pode exceder 1000 caracteres.")]
        public string Description { get; set; } = string.Empty;
    }
}
