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
        private readonly IDeleteProjectTaskService _deleteProjectTaskService;

        public ProjectTasksController(
            IUpdateProjectTaskService updateProjectTaskService,
            IUpdateProjectTaskStatusService updateProjectTaskStatusService,
            IDeleteProjectTaskService deleteProjectTaskService)
        {
            _updateProjectTaskService = updateProjectTaskService;
            _updateProjectTaskStatusService = updateProjectTaskStatusService;
            _deleteProjectTaskService = deleteProjectTaskService;
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

        /// <summary>
        /// Remove uma tarefa específica.
        /// </summary>
        /// <param name="taskId">Id externo (GUID) da tarefa a ser excluida.</param>
        /// <returns>No Content se a remoção for bem-sucedida, ou Not Found se a tarefa não existir.</returns>
        [HttpDelete("{taskId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProjectTask([FromRoute] Guid taskId)
        {
            var success = await _deleteProjectTaskService.ExecuteAsync(taskId);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
