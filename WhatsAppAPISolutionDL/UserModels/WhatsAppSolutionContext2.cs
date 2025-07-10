using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.AppSetting;
using WhatsAppAPISolutionDL.UserModels.Campaign;
using WhatsAppAPISolutionDL.UserModels.Catalog;
using WhatsAppAPISolutionDL.UserModels.CheckWallet;
using WhatsAppAPISolutionDL.UserModels.Client;
using WhatsAppAPISolutionDL.UserModels.Contact;
using WhatsAppAPISolutionDL.UserModels.Conversation;
using WhatsAppAPISolutionDL.UserModels.Dashboard;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Flow;
using WhatsAppAPISolutionDL.UserModels.Group;
using WhatsAppAPISolutionDL.UserModels.InteractiveTemplate;
using WhatsAppAPISolutionDL.UserModels.ITems;
using WhatsAppAPISolutionDL.UserModels.Location;
using WhatsAppAPISolutionDL.UserModels.MasterData;
using WhatsAppAPISolutionDL.UserModels.Media;
using WhatsAppAPISolutionDL.UserModels.Message;
using WhatsAppAPISolutionDL.UserModels.Permission;
using WhatsAppAPISolutionDL.UserModels.PlacedOrderInformation;
using WhatsAppAPISolutionDL.UserModels.SenderName;
using WhatsAppAPISolutionDL.UserModels.SurveyReport;
using WhatsAppAPISolutionDL.UserModels.SystemActions;
using WhatsAppAPISolutionDL.UserModels.Template;
using WhatsAppAPISolutionDL.UserModels.TemplateAnalytic;
using WhatsAppAPISolutionDL.UserModels.User;

namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class WhatsAppSolutionContext2 : DbContext
    {
        public WhatsAppSolutionContext2()
        {
        }

        public WhatsAppSolutionContext2(DbContextOptions<WhatsAppSolutionContext2> options)
            : base(options)
        {
        }

        public virtual DbSet<UUser> Users { get; set; } = null!;
        public virtual DbSet<UPermission> Permissions { get; set; } = null!;
        public virtual DbSet<UPermissionList> PermissionsList { get; set; } = null!;
        public virtual DbSet<UResponse> Response { get; set; } = null!;
        public virtual DbSet<UResponseWithID> ResponseWithID { get; set; } = null!;
        public virtual DbSet<UEntityDto> Entity { get; set; } = null!;
        public virtual DbSet<UEntity2Dto> Entity2 { get; set; } = null!;
        public virtual DbSet<UDashboardSummary> DashboardSummary { get; set; } = null!;
        public virtual DbSet<UDashboardReportSummary> DashboardReportSummary { get; set; } = null!;
        public virtual DbSet<UClient> Clients { get; set; } = null!;
        public virtual DbSet<UGroup> Groups { get; set; } = null!;
        public virtual DbSet<UContact> Contacts { get; set; } = null!;
        public virtual DbSet<USenderName> SenderNames { get; set; } = null!;
        public virtual DbSet<UTemplate> Templates { get; set; } = null!;
        public virtual DbSet<UTemplateParameter> TemplateParameters { get; set; } = null!;
        public virtual DbSet<UMediaUpload> UMediaUploads { get; set; } = null!;
        public virtual DbSet<UTemplateDetail> TemplateDetails { get; set; } = null!;
        public virtual DbSet<UCampaign> Campaigns { get; set; } = null!;
        public virtual DbSet<UAPIMessage> APIMessages { get; set; } = null!;
        public virtual DbSet<UMessageSentLog> MessageSentLogs { get; set; } = null!;
        public virtual DbSet<UMessageReceived> UMessageReceiveds { get; set; } = null!;
        public virtual DbSet<UAgent> Agents { get; set; } = null!;
        public virtual DbSet<UAgentTiming> AgentTimings { get; set; } = null!;
        public virtual DbSet<UConversation> Conversations { get; set; } = null!;
        public virtual DbSet<UAgentConversationList> AgentConversationLists { get; set; } = null!;
        public virtual DbSet<UConversationListByConversation> ConversationListByConversations { get; set; } = null!;
        public virtual DbSet<JsonData> JsonDatas { get; set; } = null!; 
        public virtual DbSet<UDefaultTemplateList> GetDefaultTemplates { get; set; } = null!;
        public virtual DbSet<URole> Roles { get; set; } = null!;
        public virtual DbSet<UCampaignContactStat> CampaignContactStats { get; set; } = null!;
        public virtual DbSet<UCampaignDetail> CampaignDetails { get; set; } = null!;
        public virtual DbSet<UCampaignDetailParam> CampaignDetailParams { get; set; } = null!;
        public virtual DbSet<UConversationReportList> ConversationReports { get; set; } = null!;
        public virtual DbSet<UAgentDetail> AgentDetails { get; set; } = null!;
        public virtual DbSet<UAgentSupervisorReport> AgentSupervisorReports { get; set; } = null!;
        public virtual DbSet<UClientDetail> ClientDetails { get; set; } = null!;
        public virtual DbSet<UContactDetail> ContactDetails { get; set; } = null!;
        public virtual DbSet<UGroupDetail> GroupDetails { get; set; } = null!;
        public virtual DbSet<URoleDetail> RoleDetails { get; set; } = null!;
        public virtual DbSet<USenderNameDetail> SenderNameDetails { get; set; } = null!;
        public virtual DbSet<UUserDetail> UserDetails { get; set; } = null!;
        public virtual DbSet<UUserList> UserLists{ get; set; } = null!;
        public virtual DbSet<ULatestConversationByConversation> LatestConversationByConversations { get; set; } = null!;
        public virtual DbSet<UAgentStat> AgentStats { get; set; } = null!;
        public virtual DbSet<UInteractiveTemplate> InteractiveTemplates { get; set; } = null!;
        public virtual DbSet<UInteractiveTemplateDetail> InteractiveTemplateDetails { get; set; } = null!;
        public virtual DbSet<USystemActions> SystemActions { get; set; } = null!;
        public virtual DbSet<USystemActionsDetail> SystemActionsDetail { get; set; } = null!;
        public virtual DbSet<UAppSetting> AppSetting { get; set; } = null!;
        public virtual DbSet<UConversationLogsList> ConversationLogsList { get; set; } = null!;
        public virtual DbSet<UConversationStatistics> ConversationStatistics { get; set; } = null!;
        public virtual DbSet<UMasterData> MasterData { get; set; } = null!;
        public virtual DbSet<UFlow> Flow { get; set; } = null!;
        public virtual DbSet<USurveyResponse> SurveyResponse { get; set; } = null!;
        public virtual DbSet<UCatalogExport> CatalogExports { get; set; } = null!;
        public virtual DbSet<ConversationAnalytic> ConversationAnalytics { get; set; } = null!;
        public virtual DbSet<DBResponse> DBResponses { get; set; } = null!;
        public virtual DbSet<UAppSettingsList> AppSettingsList { get; set; } = null!;
        public virtual DbSet<USurveyReport> SurveyReports { get; set; } = null!;
        public virtual DbSet<USaveGeoLocation> SaveGeoLocation { get; set; } = null!;
        public virtual DbSet<UItems> ITems { get; set; } = null!;
        public virtual DbSet<UItemModiferDetails> ITemModifier { get; set; } = null!;
        public virtual DbSet<UItemModifierItemDetails> ModifierItem { get; set; } = null!;
        public virtual DbSet<UOrderListingDetails> OrderListingDetails { get; set; } = null!;
        public virtual DbSet<UOrderSummaryDetails> OrderSummaryDetails { get; set; } = null!;
        public virtual DbSet<UWalletBalanceCheck> CheckWalleteBalance { get; set; } = null!;
        public virtual DbSet<UTemplateAnalyticsSummary> TemplateAnalyticsSummary { get; set; } = null!;
        public virtual DbSet<UTemplateAnalyticsDetailsList> TemplateAnalyticsDetailsList { get; set; } = null!;
        public virtual DbSet<UTemplateAnalyticsExportList> TemplateAnalyticsExportList { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UUser>().HasNoKey();
            modelBuilder.Entity<UPermission>().HasNoKey();
            modelBuilder.Entity<UPermissionList>().HasNoKey();
            modelBuilder.Entity<UResponse>().HasNoKey();
            modelBuilder.Entity<UEntityDto>().HasNoKey();
            modelBuilder.Entity<UDashboardSummary>().HasNoKey();
            modelBuilder.Entity<UDashboardReportSummary>().HasNoKey();
            modelBuilder.Entity<JsonData>().HasNoKey();
            modelBuilder.Entity<UClient>().HasNoKey();
            modelBuilder.Entity<UGroup>().HasNoKey();
            modelBuilder.Entity<UContact>().HasNoKey();
            modelBuilder.Entity<USenderName>().HasNoKey();
            modelBuilder.Entity<UTemplate>().HasNoKey();
            modelBuilder.Entity<UTemplateParameter>().HasNoKey();
            modelBuilder.Entity<UMediaUpload>().HasNoKey();
            modelBuilder.Entity<UTemplateDetail>().HasNoKey();
            modelBuilder.Entity<UCampaign>().HasNoKey();
            modelBuilder.Entity<UAPIMessage>().HasNoKey();
            modelBuilder.Entity<UMessageSentLog>().HasNoKey();
            modelBuilder.Entity<UMessageReceived>().HasNoKey();
            modelBuilder.Entity<UAgent>().HasNoKey();
            modelBuilder.Entity<UAgentTiming>().HasNoKey();
            modelBuilder.Entity<UConversation>().HasNoKey();
            modelBuilder.Entity<UAgentConversationList>().HasNoKey();
            modelBuilder.Entity<UConversationListByConversation>().HasNoKey();
            modelBuilder.Entity<UEntity2Dto>().HasNoKey(); 
            modelBuilder.Entity<UDefaultTemplateList>().HasNoKey();
            modelBuilder.Entity<URole>().HasNoKey();
            modelBuilder.Entity<UCampaignContactStat>().HasNoKey();
            modelBuilder.Entity<UCampaignDetail>().HasNoKey();
            modelBuilder.Entity<UCampaignDetailParam>().HasNoKey();
            modelBuilder.Entity<UConversationReportList>().HasNoKey();
            modelBuilder.Entity<UAgentDetail>().HasNoKey();
            modelBuilder.Entity<UAgentSupervisorReport>().HasNoKey();
            modelBuilder.Entity<UClientDetail>().HasNoKey();
            modelBuilder.Entity<UContactDetail>().HasNoKey();
            modelBuilder.Entity<UGroupDetail>().HasNoKey();
            modelBuilder.Entity<URoleDetail>().HasNoKey();
            modelBuilder.Entity<USenderNameDetail>().HasNoKey();
            modelBuilder.Entity<UUserDetail>().HasNoKey();
            modelBuilder.Entity<UUserList>().HasNoKey();
            modelBuilder.Entity<ULatestConversationByConversation>().HasNoKey();
            modelBuilder.Entity<UAgentStat>().HasNoKey();
            modelBuilder.Entity<UInteractiveTemplate>().HasNoKey();
            modelBuilder.Entity<UInteractiveTemplateDetail>().HasNoKey();
            modelBuilder.Entity<USystemActions>().HasNoKey();
            modelBuilder.Entity<USystemActionsDetail>().HasNoKey();
            modelBuilder.Entity<UAppSetting>().HasNoKey();
            modelBuilder.Entity<UConversationLogsList>().HasNoKey();
            modelBuilder.Entity<UConversationStatistics>().HasNoKey();
            modelBuilder.Entity<UMasterData>().HasNoKey();
            modelBuilder.Entity<UFlow>().HasNoKey();
            modelBuilder.Entity<USurveyResponse>().HasNoKey();
            modelBuilder.Entity<UCatalogExport>().HasNoKey();
            modelBuilder.Entity<ConversationAnalytic>().HasNoKey();
            modelBuilder.Entity<DBResponse>().HasNoKey();
            modelBuilder.Entity<UAppSettingsList>().HasNoKey();
            modelBuilder.Entity<USurveyReport>().HasNoKey();
            modelBuilder.Entity<UItems>().HasNoKey();
            modelBuilder.Entity<UItemModiferDetails>().HasNoKey();
            modelBuilder.Entity<UItemModifierItemDetails>().HasNoKey();
            modelBuilder.Entity<UOrderListingDetails>().HasNoKey();
            modelBuilder.Entity<UOrderSummaryDetails>().HasNoKey();
            modelBuilder.Entity<UWalletBalanceCheck>().HasNoKey();
            modelBuilder.Entity<UTemplateAnalyticsSummary>().HasNoKey();
            modelBuilder.Entity<UTemplateAnalyticsDetailsList>().HasNoKey();
            modelBuilder.Entity<UTemplateAnalyticsExportList>().HasNoKey();
        }
    }
}
