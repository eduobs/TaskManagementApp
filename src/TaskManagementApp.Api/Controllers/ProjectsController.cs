using Microsoft.AspNetCore.Mvc;
using TaskManagementApp.Application.Projects;
using TaskManagementApp.Application.ProjectTasks;
using TaskManagementApp.Models.Errors;
using TaskManagementApp.Models.Projects;
using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ICreateProjectService _projectService;
        private readonly IGetProjectService _getProjectService;
        private readonly IGetAllProjectsService _getAllProjectsService;
        private readonly ICreateProjectTaskService _createProjectTaskService;
        private readonly IGetProjectTasksByProjectIdService _getProjectTasksByProjectIdService;
        private readonly IDeleteProjectService _deleteProjectService;


        public ProjectsController(ICreateProjectService projectService,
            IGetProjectService getProjectService,
            IGetAllProjectsService getAllProjectsService,
            ICreateProjectTaskService createProjectTaskService,
            IGetProjectTasksByProjectIdService getProjectTasksByProjectIdService,
            IDeleteProjectService deleteProjectService)
        {
            _projectService = projectService;
            _getProjectService = getProjectService;
            _getAllProjectsService = getAllProjectsService;
            _createProjectTaskService = createProjectTaskService;
            _getProjectTasksByProjectIdService = getProjectTasksByProjectIdService;
            _deleteProjectService = deleteProjectService;
        }

        /// <summary>
        /// Cria um novo projeto.
        /// </summary>
        /// <param name="request">Dados para a criação do projeto.</param>
        /// <returns>O projeto recém-criado.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request, [FromHeader(Name = "X-User-Id")] Guid xUserId)
        {
            if (xUserId.Equals(Guid.Empty))
                return BadRequest(new ErrorResponse("INVALID_ARGUMENT", "O cabeçalho 'X-User-Id' é obrigatório e deve ser um GUID válido."));

            var response = await _projectService.ExecuteAsync(request, xUserId);
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

        /// <summary>
        /// Lista todos os projetos.
        /// </summary>
        /// <returns>Lista de projetos.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProjectResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _getAllProjectsService.ExecuteAsync();

            if (projects == null || !projects.Any())
                return NoContent();

            return Ok(projects);
        }

        /// <summary>
        /// Adiciona uma nova tarefa a um projeto.
        /// </summary>
        /// <param name="projectId">Id externo (GUID) do projeto ao qual a tarefa será adicionada.</param>
        /// <param name="request">Dados para a criação da tarefa.</param>
        /// <returns>Tarefa recém-criada.</returns>
        [HttpPost("{projectId}/tasks")]
        [ProducesResponseType(typeof(ProjectTaskResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateProjectTask(
            [FromRoute] Guid projectId,
            [FromBody] CreateProjectTaskRequest request,
            [FromHeader(Name = "X-User-Id")] Guid xUserId)
        {
            if (xUserId.Equals(Guid.Empty))
                return BadRequest(new ErrorResponse("INVALID_ARGUMENT", "O cabeçalho 'X-User-Id' é obrigatório e deve ser um GUID válido."));

            var response = await _createProjectTaskService.ExecuteAsync(projectId, request, xUserId);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        /// <summary>
        /// Lista todas as tarefas de um projeto.
        /// </summary>
        /// <param name="projectId">ID externo (GUID) do projeto.</param>
        /// <returns>Lista de tarefas do projeto.</returns>
        [HttpGet("{projectId}/tasks")]
        [ProducesResponseType(typeof(IEnumerable<ProjectTaskResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProjectTasksByProjectId([FromRoute] Guid projectId)
        {
            var projectTasks = await _getProjectTasksByProjectIdService.ExecuteAsync(projectId);

            if (projectTasks == null || !projectTasks.Any())
                return NoContent();

            return Ok(projectTasks);
        }
        
        /// <summary>
        /// Remove um projeto específico.
        /// </summary>
        /// <param name="projectId">Id externo (GUID) do projeto a ser removido.</param>
        /// <returns>No Content se a remoção for bem-sucedida, ou Not Found se o projeto não existir, ou Conflict se tiver tarefas pendentes.</returns>
        [HttpDelete("{projectId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] 
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteProject([FromRoute] Guid projectId)
        {
            var success = await _deleteProjectService.ExecuteAsync(projectId);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
