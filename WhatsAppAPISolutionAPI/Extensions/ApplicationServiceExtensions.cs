using WhatsAppAPISolutionAPI.Security;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;

namespace WhatsAppAPISolutionAPI.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<TokenService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IExportManager, ExportManager>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<ISenderNameService, SenderNameService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<IMediaService, MediaService>();
            services.AddScoped<ICampaignService, CampaignService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<ICustomIntegrationService, CustomIntegrationService>();
            services.AddScoped<IAPIMessageService, APIMessageService>();
            services.AddScoped<IMessageSentLogsService, MessageSentLogsService>();
            services.AddScoped<ICommunicationService, CommunicationService>();
            services.AddScoped<IAgentsService, AgentsService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IImportManager, ImportManager>();
            services.AddScoped<IInteractiveTemplateService, InteractiveTemplateService>();
            services.AddScoped<ISystemActionsService, SystemActionsService>();
            services.AddScoped<IMasterDataService, MasterDataService>();
            services.AddScoped<IFlowsService, FlowsService>();
            services.AddScoped<ISupervisorService, SupervisorService>();
            services.AddScoped<FlowOpsService>();
            services.AddScoped<ICatalogService, CatalogService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IAppSettingsService, AppSettingsService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IConversationAnalyticsService, ConversationAnalyticsService>();
            services.AddScoped<ISurveyReportService, SurveyReportService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IMediatorService, MediatorService>();
            services.AddScoped<IKFGPaymentService, KFGPaymentService>();
            services.AddScoped<ISignalRService, SignalRService>();

            #region Third party services

            services.AddScoped<IOneSignalService, OneSignalService>();
            services.AddScoped<IWitAIService, WitAIService>();
             
            #endregion

            return services;

        }
    }
}
