using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementApp.Application.Reports;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Enums;
using TaskManagementApp.Domain.Interfaces;

namespace TaskManagementApp.Tests.Application.Reports
{
    public class GetPerformanceReportServiceTests
    {
        private readonly Mock<IProjectTaskService> _mockProjectTaskDomainService;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ILogger<GetPerformanceReportService>> _mockLogger;
        private readonly GetPerformanceReportService _getPerformanceReportService;

        private readonly User _managerUser;
        private readonly User _basicUser;
        private readonly Project _testProject;

        public GetPerformanceReportServiceTests()
        {
            _mockProjectTaskDomainService = new Mock<IProjectTaskService>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<GetPerformanceReportService>>();
            _getPerformanceReportService = new GetPerformanceReportService(                
                _mockLogger.Object,
                _mockProjectTaskDomainService.Object,
                _mockUserRepository.Object
            );

            _managerUser = new User("Gerente Teste", UserRole.Manager);
            _managerUser.GetType().GetProperty("Id")?.SetValue(_managerUser, 100);
            _managerUser.GetType().GetProperty("ExternalId")?.SetValue(_managerUser, Guid.Parse("00000000-0000-0000-0000-000000000100"));

            _basicUser = new User("Usuário Comum Teste", UserRole.Basic);
            _basicUser.GetType().GetProperty("Id")?.SetValue(_basicUser, 101);
            _basicUser.GetType().GetProperty("ExternalId")?.SetValue(_basicUser, Guid.Parse("00000000-0000-0000-0000-000000000101"));

            _testProject = new Project("Projeto Relatorio", "Desc", _managerUser.Id);
            _testProject.GetType().GetProperty("Id")?.SetValue(_testProject, 1);
            _testProject.GetType().GetProperty("ExternalId")?.SetValue(_testProject, Guid.NewGuid());
        }

        [Fact(DisplayName = @"DADO um usuário com função 'Gerente'
                            QUANDO gerar o relatório de desempenho com tarefas concluídas no período
                            ENTÃO deve retornar o relatório com as médias")]
        public async Task ExecuteAsync_UsuarioGerenteComTarefasConcluidasNoPeriodo_DeveRetornarRelatorio()
        {
            // Arrange
            var requestingUserExternalId = _managerUser.ExternalId;
            var todayUtc = DateTime.UtcNow.Date;
            var thirtyDaysAgo = todayUtc.AddDays(-30);

            _mockUserRepository
                .Setup(r => r.GetByExternalIdAsync(requestingUserExternalId))
                .ReturnsAsync(_managerUser);
            _mockUserRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync([_managerUser, _basicUser]);

            var tasks = new List<ProjectTask>
            {
                CreateTestTask(todayUtc.AddDays(-20), ProjectTaskStatus.Completed, _managerUser.Id, _testProject.Id, _testProject, _managerUser),
                CreateTestTask(todayUtc.AddDays(-5), ProjectTaskStatus.Completed, _managerUser.Id, _testProject.Id, _testProject, _managerUser),
                CreateTestTask(todayUtc.AddDays(-15), ProjectTaskStatus.Completed, _basicUser.Id, _testProject.Id, _testProject, _basicUser),
                CreateTestTask(todayUtc.AddDays(-31), ProjectTaskStatus.Completed, _managerUser.Id, _testProject.Id, _testProject, _managerUser),
                CreateTestTask(todayUtc.AddDays(-10), ProjectTaskStatus.Pending, _managerUser.Id, _testProject.Id, _testProject, _managerUser)
            };

            _mockProjectTaskDomainService
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(tasks);

            // Act
            var report = await _getPerformanceReportService.ExecuteAsync(requestingUserExternalId);

            // Assert
            report.Should().NotBeNull();
            report.PeriodInDays.Should().Be(30);
            report.ReportGeneratedDate.Date.Should().Be(todayUtc);

            report.PerformanceSummaries.Should().NotBeNullOrEmpty();
            report.PerformanceSummaries.Should().HaveCount(2);

            var managerSummary = report.PerformanceSummaries.FirstOrDefault(s => s.UserId == _managerUser.ExternalId);
            managerSummary.Should().NotBeNull();
            managerSummary!.UserName.Should().Be(_managerUser.Name);
            managerSummary.CompletedTasksCount.Should().Be(2);
            managerSummary.AverageTasksPerDay.Should().BeApproximately(2.0 / 30.0, 0.001);

            var basicUserSummary = report.PerformanceSummaries.FirstOrDefault(s => s.UserId == _basicUser.ExternalId);
            basicUserSummary.Should().NotBeNull();
            basicUserSummary!.UserName.Should().Be(_basicUser.Name);
            basicUserSummary.CompletedTasksCount.Should().Be(1);
            basicUserSummary.AverageTasksPerDay.Should().BeApproximately(1.0 / 30.0, 0.001);

            report.OverallAverageTasksPerDay.Should().BeApproximately(3.0 / 30.0, 0.1);

            _mockUserRepository.Verify(r => r.GetByExternalIdAsync(requestingUserExternalId), Times.Once());
            _mockUserRepository.Verify(r => r.GetAllAsync(), Times.Once());
            _mockProjectTaskDomainService.Verify(s => s.GetAllAsync(), Times.Once());
        }

        [Fact(DisplayName = @"DADO um usuário com função 'Gerente'
                            QUANDO não houver tarefas concluídas
                            ENTÃO deve retornar listas vazias 
                            E médias zero")]
        public async Task ExecuteAsync_SemTarefasConcluidas_DeveRetornarRelatorioVazio()
        {
            // Arrange
            var requestingUserExternalId = _managerUser.ExternalId;

            _mockUserRepository
                .Setup(r => r.GetByExternalIdAsync(requestingUserExternalId))
                .ReturnsAsync(_managerUser);
            _mockUserRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync([_managerUser]);

            var tasks = new List<ProjectTask>
            {
                CreateTestTask(DateTime.Today.AddDays(-50), ProjectTaskStatus.Completed, _managerUser.Id, _testProject.Id, _testProject, _managerUser),
                CreateTestTask(DateTime.Today.AddDays(-5), ProjectTaskStatus.Pending, _managerUser.Id, _testProject.Id, _testProject, _managerUser)
            };

            _mockProjectTaskDomainService
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(tasks);

            // Act
            var report = await _getPerformanceReportService.ExecuteAsync(requestingUserExternalId);

            // Assert
            report.Should().NotBeNull();
            report.PerformanceSummaries.Should().BeEmpty();
            report.OverallAverageTasksPerDay.Should().Be(0);

            _mockProjectTaskDomainService.Verify(s => s.GetAllAsync(), Times.Once());
        }

        [Fact(DisplayName = @"DADO um usuário com função 'Básica'
                            QUANDO tentar gerar o relatório
                            ENTÃO deve lançar uma UnauthorizedAccessException")]
        public async Task ExecuteAsync_UsuarioComum_DeveLancarUnauthorizedAccessException()
        {
            // Arrange
            var requestingUserExternalId = _basicUser.ExternalId;
            _mockUserRepository
                .Setup(r => r.GetByExternalIdAsync(requestingUserExternalId))
                .ReturnsAsync(_basicUser);

            // Act
            Func<Task> act = async () => await _getPerformanceReportService.ExecuteAsync(requestingUserExternalId);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                       .WithMessage("Você não tem permissão para acessar este relatório. Apenas usuários com função de 'gerente' podem fazê-lo.");

            _mockUserRepository.Verify(r => r.GetByExternalIdAsync(requestingUserExternalId), Times.Once());
            _mockProjectTaskDomainService.Verify(s => s.GetAllAsync(), Times.Never());
        }

        [Fact(DisplayName = @"DADO um id de usuário inexistente
                            QUANDO tentar gerar o relatório
                            ENTÃO deve lançar uma ArgumentException")]
        public async Task ExecuteAsync_UsuarioInexistente_DeveLancarArgumentException()
        {
            // Arrange
            var requestingUserExternalId = Guid.NewGuid();
            _mockUserRepository
                .Setup(r => r.GetByExternalIdAsync(requestingUserExternalId))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _getPerformanceReportService.ExecuteAsync(requestingUserExternalId);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                       .WithMessage("Usuário solicitante do relatório não encontrado.");

            _mockUserRepository.Verify(r => r.GetByExternalIdAsync(requestingUserExternalId), Times.Once());
            _mockProjectTaskDomainService.Verify(s => s.GetAllAsync(), Times.Never());
        }

        private static ProjectTask CreateTestTask(DateTime updatedAt, ProjectTaskStatus status, int assignedToUserId, int projectId, Project project, User assignedUserEntity)
        {
            var safeDeadline = DateTime.Today.AddDays(1);

            var task = new ProjectTask($"Tarefa {Guid.NewGuid()}", "Descrição", safeDeadline, ProjectTaskPriority.Low, projectId, assignedToUserId);
            task.GetType().GetProperty("ExternalId")?.SetValue(task, Guid.NewGuid());
            task.GetType().GetProperty("CreatedAt")?.SetValue(task, DateTime.UtcNow.AddDays(-60));
            task.GetType().GetProperty("UpdatedAt")?.SetValue(task, updatedAt);
            task.GetType().GetProperty("Status")?.SetValue(task, status);

            task.GetType().GetProperty("Project")?.SetValue(task, project);
            task.GetType().GetProperty("AssignedToUser")?.SetValue(task, assignedUserEntity);

            return task;
        }
    }
}
