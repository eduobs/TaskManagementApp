using TaskManagementApp.Models.Reports;

namespace TaskManagementApp.Application.Reports
{
    public interface IGetPerformanceReportService
    {        
        /// <summary>
        /// Serviço que retorna um relatório de desempenho do usuário
        /// </summary>
        /// <param name="userId">Usuário a ser analisado</param>
        /// <returns>Relatório de desempenho do usuário</returns>
        Task<UserPerformanceReportResponse> ExecuteAsync(Guid userId);
    }
}
