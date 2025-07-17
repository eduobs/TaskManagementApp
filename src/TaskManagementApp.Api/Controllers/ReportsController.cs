using Microsoft.AspNetCore.Mvc;
using TaskManagementApp.Application.Reports;
using TaskManagementApp.Models.Reports;

namespace TaskManagementApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IGetPerformanceReportService _getPerformanceReportService;

        public ReportsController(IGetPerformanceReportService getPerformanceReportService)
        {
            _getPerformanceReportService = getPerformanceReportService;
        }

        /// <summary>
        /// Gera um relatório de desempenho, mostrando o número médio de tarefas concluídas por usuário nos últimos 30 dias.
        /// Apenas usuários com a função 'gerente' podem acessar este relatório.
        /// </summary>
        /// <param name="xUserId">Id externo (GUID) do usuário solicitante.</param>
        /// <returns>Relatório de desempenho.</returns>
        [HttpGet("performance")]
        [ProducesResponseType(typeof(UserPerformanceReportResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPerformanceReport([FromHeader(Name = "X-User-Id")] Guid xUserId)
        {
            if (xUserId == Guid.Empty)
                return BadRequest(new { message = "O cabeçalho 'X-User-Id' é obrigatório e deve ser um GUID válido." });

            var report = await _getPerformanceReportService.ExecuteAsync(xUserId);

            return Ok(report);
        }
    }
}