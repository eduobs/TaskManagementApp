using Microsoft.AspNetCore.Mvc;
using TaskManagementApp.Application.ProjectTasks;
using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Api.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class ProjectTasksController : ControllerBase
    {
        private readonly IUpdateProjectTaskService _updateProjectTaskService;

        public ProjectTasksController(IUpdateProjectTaskService updateProjectTaskService)
        {
            _updateProjectTaskService = updateProjectTaskService;
        }

        /// <summary>
        /// Atualiza os detalhes de uma tarefa.
        /// </summary>
        /// <param name="taskId">ID externo (GUID) da tarefa a ser atualizada.</param>
        /// <param name="request">Dados para a atualização da tarefa.</param>
        /// <returns>Tarefa atualizada.</returns>
        [HttpPut("{taskId}")]
        [ProducesResponseType(typeof(ProjectTaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProjectTask([FromRoute] Guid taskId, [FromBody] UpdateProjectTaskRequest request)
        {
            var response = await _updateProjectTaskService.ExecuteAsync(taskId, request);

            if (response == null)
                return NotFound();

            return Ok(response);
        }
    }
}
