using Microsoft.AspNetCore.Mvc;
using TaskManagementApp.Application.Projects;
using TaskManagementApp.Models.Projects;

namespace TaskManagementApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ILogger<ProjectsController> _logger;
        private readonly ICreateProjectService _projectService;
        private readonly IGetProjectService _getProjectService;


        public ProjectsController(ILogger<ProjectsController> logger, ICreateProjectService projectService, IGetProjectService getProjectService)
        {
            _logger = logger;
            _projectService = projectService;
            _getProjectService = getProjectService;
        }

        /// <summary>
        /// Cria um novo projeto.
        /// </summary>
        /// <param name="request">Dados para a criação do projeto.</param>
        /// <returns>O projeto recém-criado.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
        {
            var response = await _projectService.ExecuteAsync(request);
            return CreatedAtAction(nameof(GetProjectById), new { id = response.Id }, response);
        }

        /// <summary>
        /// Obtém um projeto pelo seu ID.
        /// </summary>
        /// <param name="id">ID externo (GUID) do projeto.</param>
        /// <returns>O projeto solicitado.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            var response = await _getProjectService.GetProjectByExternalIdAsync(id);

            if (response == null)
                return NotFound();

            return Ok(response);
        }
    }
}
