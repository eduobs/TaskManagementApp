using Microsoft.Extensions.Logging;
using TaskManagementApp.Domain.Interfaces;

namespace TaskManagementApp.Application.Projects
{
    public class DeleteProjectService : IDeleteProjectService
    {
        private readonly IProjectService _projectDomainService;
        private readonly ILogger<DeleteProjectService> _logger;

        public DeleteProjectService(IProjectService projectDomainService, ILogger<DeleteProjectService> logger)
        {
            _projectDomainService = projectDomainService;
            _logger = logger;
        }

        public async Task<bool> ExecuteAsync(Guid projectExternalId)
        {
            _logger.LogInformation("Iniciando a exclusão do projeto {ProjectExternalId}.", projectExternalId);

            var success = await _projectDomainService.DeleteProjectAsync(projectExternalId);

            if (!success)
            {
                _logger.LogWarning("Projeto com ExternalId {ProjectExternalId} não encontrado para exclusão.", projectExternalId);
                return false;
            }

            _logger.LogInformation("Projeto com id: {ProjectExternalId} excluído com sucesso.", projectExternalId);

            return true;
        }
    }
}
