using Microsoft.Extensions.Logging;
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

        public Task<UserPerformanceReportResponse> ExecuteAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}