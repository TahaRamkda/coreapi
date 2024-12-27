using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

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
        public virtual DbSet<UTemplateDetails> TemplateDetails { get; set; } = null!;
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
        public virtual DbSet<UGetAgentById> GetAgentByIds { get; set; } = null!;

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
            modelBuilder.Entity<UTemplateDetails>().HasNoKey();
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
            modelBuilder.Entity<UGetAgentById>().HasNoKey();
        }
    }
}
