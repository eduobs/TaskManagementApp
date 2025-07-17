using Microsoft.Extensions.Logging;
using TaskManagementApp.Domain.Enums;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Models.Reports;

namespace TaskManagementApp.Application.Reports
{
    public class GetPerformanceReportService : IGetPerformanceReportService
    {
        private readonly IProjectTaskService _projectTaskDomainService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetPerformanceReportService> _logger;
        private const int ReportPeriodInDays = 30;

        public GetPerformanceReportService(
            ILogger<GetPerformanceReportService> logger,
            IProjectTaskService projectTaskDomainService,
            IUserRepository userRepository)
        {
            _projectTaskDomainService = projectTaskDomainService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserPerformanceReportResponse> ExecuteAsync(Guid userId)
        {
            _logger.LogInformation("Iniciando geração de relatório de desempenho para o usuário {RequestingUserExternalId}.", userId);

            var requestingUser = await _userRepository.GetByExternalIdAsync(userId);
            CheckUserExistent(userId, requestingUser);
            CheckRoleUser(requestingUser);

            var endDate = DateTime.UtcNow.Date;
            var startDate = endDate.AddDays(-ReportPeriodInDays);

            _logger.LogInformation("Período do relatório: de {StartDate:d} a {EndDate:d} ({PeriodInDays} dias).", startDate, endDate, ReportPeriodInDays);

            var allTasks = await _projectTaskDomainService.GetAllAsync();

            var completedTasksInPeriod = allTasks
                .Where(t => t.Status == ProjectTaskStatus.Completed &&
                            t.UpdatedAt.Date >= startDate && t.UpdatedAt.Date <= endDate)
                .ToList();

            _logger.LogInformation("Encontradas {CompletedTasksCount} tarefas concluídas no período de {PeriodInDays} dias.", completedTasksInPeriod.Count, ReportPeriodInDays);

            var usersPerformanceData = completedTasksInPeriod
                .GroupBy(t => t.AssignedToUserId)
                .Select(g => new
                {
                    AssignedToUserIdInternal = g.Key,
                    CompletedTasksCount = g.Count()
                })
                .ToList();

            var allUsers = await _userRepository.GetAllAsync();
            var usersByIdInternal = allUsers.ToDictionary(u => u.Id, u => u);

            var performanceSummaries = new List<UserPerformanceSummary>();
            foreach (var userData in usersPerformanceData)
            {
                if (usersByIdInternal.TryGetValue(userData.AssignedToUserIdInternal, out var user))
                {
                    performanceSummaries.Add(new UserPerformanceSummary
                    {
                        UserId = user.ExternalId,
                        UserName = user.Name,
                        CompletedTasksCount = userData.CompletedTasksCount,
                        AverageTasksPerDay = (double)userData.CompletedTasksCount / ReportPeriodInDays
                    });
                }
                else
                {
                    _logger.LogWarning("Usuário com ID interno {UserIdInternal} não encontrado no repositório de usuários para relatório. Pode ter sido excluído.", userData.AssignedToUserIdInternal);
                }
            }

            double overallAverage = performanceSummaries.Count != 0 ? performanceSummaries.Average(s => s.AverageTasksPerDay) : 0;

            _logger.LogInformation("Relatório de desempenho gerado com {UserCount} usuários e média geral de {OverallAverage} tarefas/dia.", performanceSummaries.Count, overallAverage);

            return new UserPerformanceReportResponse
            {
                PeriodInDays = ReportPeriodInDays,
                PerformanceSummaries = [.. performanceSummaries.OrderByDescending(s => s.AverageTasksPerDay)],
                OverallAverageTasksPerDay = overallAverage
            };
        }

        private void CheckUserExistent(Guid userId, Domain.Entities.User? requestingUser)
        {
            if (requestingUser == null)
            {
                _logger.LogWarning("Tentativa de acesso ao relatório por usuário inexistente: {RequestingUserExternalId}.", userId);
                throw new ArgumentException("Usuário solicitante do relatório não encontrado.");
            }
        }

        private void CheckRoleUser(Domain.Entities.User? requestingUser)
        {
            if (requestingUser != null && requestingUser.Role != UserRole.Manager)
            {
                _logger.LogWarning("Acesso não autorizado ao relatório de desempenho para o usuário {UserName} (ID: {UserId}, Função: {Role}). Apenas gerentes podem acessar.", requestingUser.Name, requestingUser.ExternalId, requestingUser.Role);
                throw new UnauthorizedAccessException("Você não tem permissão para acessar este relatório. Apenas usuários com função de 'gerente' podem fazê-lo.");
            }
        }
    }
}
