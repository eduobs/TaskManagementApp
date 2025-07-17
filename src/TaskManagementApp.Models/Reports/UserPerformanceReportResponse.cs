namespace TaskManagementApp.Models.Reports
{
    public class UserPerformanceSummary
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int CompletedTasksCount { get; set; }
        public double AverageTasksPerDay { get; set; }
    }

    public class UserPerformanceReportResponse
    {
        public UserPerformanceReportResponse()
        {
            ReportGeneratedDate = DateTime.UtcNow;
        }

        public DateTime ReportGeneratedDate { get; set; }
        public int PeriodInDays { get; set; }
        public List<UserPerformanceSummary> PerformanceSummaries { get; set; } = [];
        public double OverallAverageTasksPerDay { get; set; }
    }
}
