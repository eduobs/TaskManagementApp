using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementApp.Application.ProjectTasks;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Enums;
using TaskManagementApp.Domain.Interfaces;
using TaskManagementApp.Models.ProjectTasks;

namespace TaskManagementApp.Tests.Application.ProjectTasks
{
    public class AddCommentToTaskServiceTests
    {
        private readonly Mock<IProjectTaskService> _mockProjectTaskService;
        private readonly Mock<IProjectTaskHistoryRepository> _mockProjectTaskHistoryRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ILogger<AddCommentToTaskService>> _mockLogger;
        private readonly AddCommentToTaskService _addCommentToTaskService;

        private readonly User _testCommentUser;
        private readonly Project _testProject;
        private readonly ProjectTask _testProjectTask;

        public AddCommentToTaskServiceTests()
        {
            _mockProjectTaskService = new Mock<IProjectTaskService>();
            _mockProjectTaskHistoryRepository = new Mock<IProjectTaskHistoryRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<AddCommentToTaskService>>();

            _addCommentToTaskService = new AddCommentToTaskService(
                _mockLogger.Object,
                _mockProjectTaskService.Object,
                _mockProjectTaskHistoryRepository.Object,
                _mockUserRepository.Object
            );

            _testCommentUser = new User("Usuário", UserRole.Basic);
            _testCommentUser.GetType().GetProperty("Id")?.SetValue(_testCommentUser, 50);
            _testCommentUser.GetType().GetProperty("ExternalId")?.SetValue(_testCommentUser, Guid.NewGuid());

            _testProject = new Project("Projeto teste", "Desc", 1);
            _testProject.GetType().GetProperty("Id")?.SetValue(_testProject, 1);

            _testProjectTask = new ProjectTask(
                "Tarefa teste", "Descrição", DateTime.Today.AddDays(7),
                ProjectTaskPriority.Low, _testProject.Id, _testCommentUser.Id
            );
            _testProjectTask.GetType().GetProperty("ExternalId")?.SetValue(_testProjectTask, Guid.NewGuid());
            _testProjectTask.GetType().GetProperty("Id")?.SetValue(_testProjectTask, 1);
        }

        [Fact(DisplayName = @"DADO uma requisição válida
                            QUANDO a tarefa e o usuário existirem
                            ENTÃO deve adicionar o comentário")]
        public async Task ExecuteAsync_DadosValidos_DeveAdicionarERetornarTrue()
        {
            // Arrange
            var taskExternalId = _testProjectTask.ExternalId;
            var commentedByUserId = _testCommentUser.ExternalId;
            var request = new AddCommentToTaskRequest { CommentContent = "Este é um novo comentário de teste." };

            _mockProjectTaskService
                .Setup(s => s.GetProjectTaskByExternalIdAsync(taskExternalId))
                .ReturnsAsync(_testProjectTask);
            _mockUserRepository
                .Setup(r => r.GetByExternalIdAsync(commentedByUserId))
                .ReturnsAsync(_testCommentUser);
            _mockProjectTaskHistoryRepository
                .Setup(r => r.AddAsync(It.IsAny<ProjectTaskHistory>()))
                .Returns(Task.CompletedTask);
            _mockProjectTaskHistoryRepository
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _addCommentToTaskService.ExecuteAsync(taskExternalId, request, commentedByUserId);

            // Assert
            result.Should().BeTrue();

            _mockProjectTaskService.Verify(s => s.GetProjectTaskByExternalIdAsync(taskExternalId), Times.Once());
            _mockUserRepository.Verify(r => r.GetByExternalIdAsync(commentedByUserId), Times.Once());
            _mockProjectTaskHistoryRepository.Verify(
                r => r.AddAsync(It.Is<ProjectTaskHistory>(h =>
                    h.ProjectTaskId == _testProjectTask.Id &&
                    h.PropertyName == "Comment" &&
                    h.OldValue == "" &&
                    h.NewValue == request.CommentContent &&
                    h.ModifiedByUserId == commentedByUserId &&
                    h.ChangeType == HistoryChangeType.CommentAdded)),
                Times.Once()
            );
            _mockProjectTaskHistoryRepository.Verify(r => r.SaveChangesAsync(), Times.Once());
        }

        [Fact(DisplayName = @"DADO uma requisição
                            QUANDO a tarefa não for encontrada
                            ENTÃO deve retornar false")]
        public async Task ExecuteAsync_TarefaNaoEncontrada_DeveRetornarFalse()
        {
            // Arrange
            var taskExternalId = Guid.NewGuid();
            var commentedByUserId = _testCommentUser.ExternalId;
            var request = new AddCommentToTaskRequest { CommentContent = "Comentário." };

            _mockProjectTaskService
                .Setup(s => s.GetProjectTaskByExternalIdAsync(taskExternalId))
                .ReturnsAsync((ProjectTask?)null);
            _mockUserRepository
                .Setup(r => r.GetByExternalIdAsync(commentedByUserId))
                .ReturnsAsync(_testCommentUser);

            // Act
            var result = await _addCommentToTaskService.ExecuteAsync(taskExternalId, request, commentedByUserId);

            // Assert
            result.Should().BeFalse();

            _mockProjectTaskService.Verify(s => s.GetProjectTaskByExternalIdAsync(taskExternalId), Times.Once());
            _mockUserRepository.Verify(r => r.GetByExternalIdAsync(commentedByUserId), Times.Never());
            _mockProjectTaskHistoryRepository.Verify(r => r.AddAsync(It.IsAny<ProjectTaskHistory>()), Times.Never());
            _mockProjectTaskHistoryRepository.Verify(r => r.SaveChangesAsync(), Times.Never());
        }

        [Fact(DisplayName = @"DADO uma requisição com usuário vazio
                            QUANDO tentar adicionar o comentário
                            ENTÃO deve lançar uma ArgumentException")]
        public async Task ExecuteAsync_UserIdVazio_DeveLancarArgumentException()
        {
            // Arrange
            var taskExternalId = _testProjectTask.ExternalId;
            var commentedByUserId = Guid.Empty;
            var request = new AddCommentToTaskRequest { CommentContent = "Comentário." };

            _mockProjectTaskService
                .Setup(s => s.GetProjectTaskByExternalIdAsync(taskExternalId))
                .ReturnsAsync(_testProjectTask);

            // Act
            Func<Task> act = async () => await _addCommentToTaskService.ExecuteAsync(taskExternalId, request, commentedByUserId);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                       .WithMessage("O id do usuário que comentou é obrigatório.*");

            _mockProjectTaskService.Verify(s => s.GetProjectTaskByExternalIdAsync(taskExternalId), Times.Once());
            _mockUserRepository.Verify(r => r.GetByExternalIdAsync(It.IsAny<Guid>()), Times.Never());
            _mockProjectTaskHistoryRepository.Verify(r => r.AddAsync(It.IsAny<ProjectTaskHistory>()), Times.Never());
        }

        [Fact(DisplayName = @"DADO uma requisição de comentário com usuário inexistente
                            QUANDO tentar adicionar o comentário
                            ENTÃO deve lançar uma ArgumentException")]
        public async Task ExecuteAsync_UserNaoEncontrado_DeveLancarArgumentException()
        {
            // Arrange
            var taskExternalId = _testProjectTask.ExternalId;
            var nonExistentCommentedByUserId = Guid.NewGuid();
            var request = new AddCommentToTaskRequest { CommentContent = "Comentário." };

            _mockProjectTaskService
                .Setup(s => s.GetProjectTaskByExternalIdAsync(taskExternalId))
                .ReturnsAsync(_testProjectTask);
            _mockUserRepository
                .Setup(r => r.GetByExternalIdAsync(nonExistentCommentedByUserId))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _addCommentToTaskService.ExecuteAsync(taskExternalId, request, nonExistentCommentedByUserId);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                       .WithMessage($"Usuário com id {nonExistentCommentedByUserId} não encontrado para registrar comentário.");

            _mockProjectTaskService.Verify(s => s.GetProjectTaskByExternalIdAsync(taskExternalId), Times.Once());
            _mockUserRepository.Verify(r => r.GetByExternalIdAsync(nonExistentCommentedByUserId), Times.Once());
            _mockProjectTaskHistoryRepository.Verify(r => r.AddAsync(It.IsAny<ProjectTaskHistory>()), Times.Never());
        }

        [Fact(DisplayName = @"DADO uma requisição de comentário
                            QUANDO o repositório lançar uma exceção
                            ENTÃO deve retornar exceção")]
        public async Task ExecuteAsync_RepositorioLancaExcecao_DeveRetornarExcecao()
        {
            // Arrange
            var taskExternalId = _testProjectTask.ExternalId;
            var commentedByUserId = _testCommentUser.ExternalId;
            var request = new AddCommentToTaskRequest { CommentContent = "Comentário." };
            var expectedException = new InvalidOperationException("Erro de banco de dados ao salvar histórico.");

            _mockProjectTaskService
                .Setup(s => s.GetProjectTaskByExternalIdAsync(taskExternalId))
                .ReturnsAsync(_testProjectTask);
            _mockUserRepository
                .Setup(r => r.GetByExternalIdAsync(commentedByUserId))
                .ReturnsAsync(_testCommentUser);
            _mockProjectTaskHistoryRepository
                .Setup(r => r.AddAsync(It.IsAny<ProjectTaskHistory>()))
                .ThrowsAsync(expectedException);

            // Act
            Func<Task> act = async () => await _addCommentToTaskService.ExecuteAsync(taskExternalId, request, commentedByUserId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                       .WithMessage(expectedException.Message);

            _mockProjectTaskHistoryRepository.Verify(r => r.AddAsync(It.IsAny<ProjectTaskHistory>()), Times.Once());
            _mockProjectTaskHistoryRepository.Verify(r => r.SaveChangesAsync(), Times.Never());
        }
    }
}
