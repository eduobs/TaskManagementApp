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
        private readonly IUpdateProjectTaskStatusService _updateProjectTaskStatusService;


        public ProjectTasksController(
            IUpdateProjectTaskService updateProjectTaskService,
            IUpdateProjectTaskStatusService updateProjectTaskStatusService)
        {
            _updateProjectTaskService = updateProjectTaskService;
            _updateProjectTaskStatusService = updateProjectTaskStatusService;
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

        /// <summary>
        /// Atualiza o status de uma tarefa específica.
        /// </summary>
        /// <param name="taskId">ID externo (GUID) da tarefa cujo status será atualizado.</param>
        /// <param name="request">Dados para a atualização do status.</param>
        /// <returns>Tarefa com o status atualizado.</returns>
        [HttpPatch("{taskId}/status")]
        [ProducesResponseType(typeof(ProjectTaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProjectTaskStatus([FromRoute] Guid taskId, [FromBody] UpdateProjectTaskStatusRequest request)
        {
            var response = await _updateProjectTaskStatusService.ExecuteAsync(taskId, request);

            if (response == null)
                return NotFound();

            return Ok(response);
        }
    }
}
