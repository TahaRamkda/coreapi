using WhatsAppAPISolutionDL.Dto.Catalog;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ICatalogService
    {
        Task<bool> ImportCatalog(int clientId, int senderId, CatalogDto catalog);

        Task ExportCatalog(int clientId, int senderId);

        Task GenerateCatalogFlows(List<CatalogFlowGenerationDto> models);
    }
}
