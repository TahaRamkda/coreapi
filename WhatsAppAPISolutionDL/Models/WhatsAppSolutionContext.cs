using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class WhatsAppSolutionContext : DbContext
    {
        public WhatsAppSolutionContext()
        {
        }

        public WhatsAppSolutionContext(DbContextOptions<WhatsAppSolutionContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Agent> Agents { get; set; }
        public virtual DbSet<AgentChatReasonMap> AgentChatReasonMaps { get; set; }
        public virtual DbSet<AgentLog> AgentLogs { get; set; }
        public virtual DbSet<AgentLogsHist> AgentLogsHists { get; set; }
        public virtual DbSet<AgentSenderMap> AgentSenderMaps { get; set; }
        public virtual DbSet<AgentTiming> AgentTimings { get; set; }
        public virtual DbSet<Apimessage> Apimessages { get; set; }
        public virtual DbSet<ApimessagesHist> ApimessagesHists { get; set; }
        public virtual DbSet<Appsetting> Appsettings { get; set; }
        public virtual DbSet<Campaign> Campaigns { get; set; }
        public virtual DbSet<CampaignContact> CampaignContacts { get; set; }
        public virtual DbSet<CampaignParam> CampaignParams { get; set; }
        public virtual DbSet<CampaignResponse> CampaignResponses { get; set; }
        public virtual DbSet<CatalogImportHistory> CatalogImportHistories { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategoryItemMap> CategoryItemMaps { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<ClientWallet> ClientWallets { get; set; }
        public virtual DbSet<ClientWalletTransaction> ClientWalletTransactions { get; set; }
        public virtual DbSet<ClientWalletTransactionSummary> ClientWalletTransactionSummaries { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Conversation> Conversations { get; set; }
        public virtual DbSet<ConversationAnalytic> ConversationAnalytics { get; set; }
        public virtual DbSet<ConversationLog> ConversationLogs { get; set; }
        public virtual DbSet<ConversationLogsHist> ConversationLogsHists { get; set; }
        public virtual DbSet<ConversationMessage> ConversationMessages { get; set; }
        public virtual DbSet<ConversationMessagesHist> ConversationMessagesHists { get; set; }
        public virtual DbSet<ConversationsAgentSummary> ConversationsAgentSummaries { get; set; }
        public virtual DbSet<ConversationsHist> ConversationsHists { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Flow> Flows { get; set; }
        public virtual DbSet<FlowChildren> FlowChildrens { get; set; }
        public virtual DbSet<FlowOption> FlowOptions { get; set; }
        public virtual DbSet<FlowScreen> FlowScreens { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<InteractiveTemplate> InteractiveTemplates { get; set; }
        public virtual DbSet<InteractiveTemplateButton> InteractiveTemplateButtons { get; set; }
        public virtual DbSet<InteractiveTemplateParameter> InteractiveTemplateParameters { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemModifierMap> ItemModifierMaps { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<MasterDatum> MasterData { get; set; }
        public virtual DbSet<Media> Medias { get; set; }
        public virtual DbSet<MessageReceivedLog> MessageReceivedLogs { get; set; }
        public virtual DbSet<MessageReceivedLogsHist> MessageReceivedLogsHists { get; set; }
        public virtual DbSet<MessageSentLog> MessageSentLogs { get; set; }
        public virtual DbSet<MessageSentLogsHist> MessageSentLogsHists { get; set; }
        public virtual DbSet<MessagesentlogsStatusUpdate> MessagesentlogsStatusUpdates { get; set; }
        public virtual DbSet<MessagesentlogsStatusUpdateHist> MessagesentlogsStatusUpdateHists { get; set; }
        public virtual DbSet<ModifierGroup> ModifierGroups { get; set; }
        public virtual DbSet<ModifierItemMap> ModifierItemMaps { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderAddress> OrderAddresses { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<OrderStep> OrderSteps { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PermissionTask> PermissionTasks { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SenderName> SenderNames { get; set; }
        public virtual DbSet<SignalRqueue> SignalRqueues { get; set; }
        public virtual DbSet<Survey> Surveys { get; set; }
        public virtual DbSet<SurveyResponse> SurveyResponses { get; set; }
        public virtual DbSet<SurveyResponseDetail> SurveyResponseDetails { get; set; }
        public virtual DbSet<SystemAction> SystemActions { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<TemplateAnalytic> TemplateAnalytics { get; set; }
        public virtual DbSet<TemplateButton> TemplateButtons { get; set; }
        public virtual DbSet<TemplateParameter> TemplateParameters { get; set; }
        public virtual DbSet<TemplateResponse> TemplateResponses { get; set; }
        public virtual DbSet<TemplateResponsesHist> TemplateResponsesHists { get; set; }
        public virtual DbSet<TemplateScreen> TemplateScreens { get; set; }
        public virtual DbSet<TemplatesAnalyticsDetail> TemplatesAnalyticsDetails { get; set; }
        public virtual DbSet<UnsubscribedNumber> UnsubscribedNumbers { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersRole> UsersRoles { get; set; }
        public virtual DbSet<WhatsAppPricing> WhatsAppPricings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:WhatsAppAPISolutionDataBase");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AgentFname)
                    .HasMaxLength(50)
                    .HasColumnName("AgentFName");

                entity.Property(e => e.AgentFnameAr)
                    .HasMaxLength(100)
                    .HasColumnName("AgentFNameAR");

                entity.Property(e => e.AgentLname)
                    .HasMaxLength(50)
                    .HasColumnName("AgentLName");

                entity.Property(e => e.AgentLnameAr)
                    .HasMaxLength(100)
                    .HasColumnName("AgentLNameAR");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LastOnline).HasColumnType("datetime");

                entity.Property(e => e.PreferredLanguage).HasMaxLength(20);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<AgentChatReasonMap>(entity =>
            {
                entity.ToTable("AgentChatReasonMap");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<AgentLog>(entity =>
            {
                entity.Property(e => e.Comment).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<AgentLogsHist>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AgentLogs_Hist");

                entity.Property(e => e.Comment).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<AgentSenderMap>(entity =>
            {
                entity.ToTable("AgentSenderMap");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<AgentTiming>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.WeekDayName).HasMaxLength(50);
            });

            modelBuilder.Entity<Apimessage>(entity =>
            {
                entity.ToTable("APIMessages");

                entity.Property(e => e.ApimessageId).HasColumnName("APIMessageId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveredDate).HasColumnType("datetime");

                entity.Property(e => e.FailedTime).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.ReadDate).HasColumnType("datetime");

                entity.Property(e => e.ScheduleTime).HasColumnType("datetime");

                entity.Property(e => e.TrxType).HasMaxLength(250);

                entity.Property(e => e.Udf1)
                    .HasMaxLength(50)
                    .HasColumnName("UDF1");

                entity.Property(e => e.Udf2)
                    .HasMaxLength(50)
                    .HasColumnName("UDF2");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Url).HasColumnName("URL");

                entity.Property(e => e.WaId)
                    .HasMaxLength(250)
                    .HasColumnName("WaID");
            });

            modelBuilder.Entity<ApimessagesHist>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("APIMessages_Hist");

                entity.Property(e => e.ApimessageId).HasColumnName("APIMessageId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveredDate).HasColumnType("datetime");

                entity.Property(e => e.FailedTime).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.ReadDate).HasColumnType("datetime");

                entity.Property(e => e.ScheduleTime).HasColumnType("datetime");

                entity.Property(e => e.TrxType).HasMaxLength(250);

                entity.Property(e => e.Udf1)
                    .HasMaxLength(50)
                    .HasColumnName("UDF1");

                entity.Property(e => e.Udf2)
                    .HasMaxLength(50)
                    .HasColumnName("UDF2");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Url).HasColumnName("URL");

                entity.Property(e => e.WaId)
                    .HasMaxLength(250)
                    .HasColumnName("WaID");
            });

            modelBuilder.Entity<Appsetting>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("appsettings");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.KeyName).HasMaxLength(250);

                entity.Property(e => e.Val).HasMaxLength(250);
            });

            modelBuilder.Entity<Campaign>(entity =>
            {
                entity.Property(e => e.CampaignName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Commission).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.Cost).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EstimatedCost).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.IsCharged).HasDefaultValueSql("((0))");

                entity.Property(e => e.ScheduleDate).HasColumnType("datetime");

                entity.Property(e => e.TotalCost).HasColumnType("numeric(18, 3)");
            });

            modelBuilder.Entity<CampaignContact>(entity =>
            {
                entity.HasKey(e => e.CampaignNumberId)
                    .HasName("PK__Campaign__2BC9C5EFE1E9708F");

                entity.Property(e => e.Area).HasMaxLength(250);

                entity.Property(e => e.Cost).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeliveredTime).HasColumnType("datetime");

                entity.Property(e => e.FailedTime).HasColumnType("datetime");

                entity.Property(e => e.FirstName).HasMaxLength(250);

                entity.Property(e => e.LastName).HasMaxLength(250);

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.ReadTime).HasColumnType("datetime");

                entity.Property(e => e.SentTime).HasColumnType("datetime");

                entity.Property(e => e.SentTryTime).HasColumnType("datetime");

                entity.Property(e => e.WaId).HasMaxLength(250);
            });

            modelBuilder.Entity<CampaignParam>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ParamName).HasMaxLength(255);

                entity.Property(e => e.ParamValue).HasMaxLength(255);
            });

            modelBuilder.Entity<CampaignResponse>(entity =>
            {
                entity.Property(e => e.ContextWaId).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.WaId).HasMaxLength(250);
            });

            modelBuilder.Entity<CatalogImportHistory>(entity =>
            {
                entity.ToTable("CatalogImportHistory");

                entity.Property(e => e.CatalogPath).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeprecatedDate).HasColumnType("datetime");

                entity.Property(e => e.DescriptionAr).HasMaxLength(500);

                entity.Property(e => e.DescriptionEn).HasMaxLength(500);

                entity.Property(e => e.IntegerationId).HasMaxLength(100);

                entity.Property(e => e.NameAr).HasMaxLength(100);

                entity.Property(e => e.NameEn).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CategoryItemMap>(entity =>
            {
                entity.ToTable("CategoryItemMap");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeprecatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.AccessToken).HasMaxLength(250);

                entity.Property(e => e.AppId).HasMaxLength(100);

                entity.Property(e => e.AvailableBalance).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.Balance).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.BalanceAlertLimit).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.BusinessId).HasMaxLength(100);

                entity.Property(e => e.ClientAddress).HasMaxLength(250);

                entity.Property(e => e.ClientName).HasMaxLength(250);

                entity.Property(e => e.ContactPerson).HasMaxLength(150);

                entity.Property(e => e.ContactPersonEmail).HasMaxLength(150);

                entity.Property(e => e.ContactPersonPhone).HasMaxLength(150);

                entity.Property(e => e.CountryCode).HasMaxLength(3);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DefaultMarket).HasMaxLength(150);

                entity.Property(e => e.Prefix).HasMaxLength(3);

                entity.Property(e => e.Timezone).HasMaxLength(150);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ClientWallet>(entity =>
            {
                entity.ToTable("ClientWallet");

                entity.Property(e => e.Balance).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.CountryCode).HasMaxLength(3);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Currency).HasMaxLength(50);

                entity.Property(e => e.CurrencyCode).HasMaxLength(5);

                entity.Property(e => e.IsCoin).HasDefaultValueSql("((0))");

                entity.Property(e => e.UpdatedBy).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ClientWalletTransaction>(entity =>
            {
                entity.Property(e => e.AfterBalance).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.ApimessageId).HasColumnName("APIMessageId");

                entity.Property(e => e.Commission).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.ConversationType).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Currency).HasMaxLength(10);

                entity.Property(e => e.MetaCharges).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.PreviousBalance).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.TransactionType).HasMaxLength(6);

                entity.Property(e => e.UpdatedBy).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ClientWalletTransactionSummary>(entity =>
            {
                entity.HasKey(e => new { e.Date, e.ClientId, e.TransactionType, e.ConversationType })
                    .HasName("PK__ClientWa__A19BF92951A4CDB3");

                entity.ToTable("ClientWalletTransactionSummary");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.TransactionType).HasMaxLength(10);

                entity.Property(e => e.ConversationType).HasMaxLength(20);

                entity.Property(e => e.Currency).HasMaxLength(15);

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.TotalCommission).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.TotalMetaCharges).HasColumnType("decimal(18, 3)");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.Property(e => e.AreaName).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmailAddress).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(250);

                entity.Property(e => e.LastName).HasMaxLength(250);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasIndex(e => e.AgentId, "IX_Conversations_AgentId");

                entity.HasIndex(e => e.Id, "IX_Conversations_PhoneNumber");

                entity.Property(e => e.Commission).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.ConversationId).HasMaxLength(250);

                entity.Property(e => e.ConversationMode).HasMaxLength(50);

                entity.Property(e => e.Cost).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.ForceClosedDate).HasColumnType("datetime");

                entity.Property(e => e.FullName).HasMaxLength(200);

                entity.Property(e => e.IsCharged).HasDefaultValueSql("((0))");

                entity.Property(e => e.Language).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.Udf1)
                    .HasMaxLength(50)
                    .HasColumnName("UDF1");

                entity.Property(e => e.Udf2)
                    .HasMaxLength(50)
                    .HasColumnName("UDF2");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.WaId).HasMaxLength(250);
            });

            modelBuilder.Entity<ConversationAnalytic>(entity =>
            {
                entity.Property(e => e.Category).HasMaxLength(100);

                entity.Property(e => e.ConversationType).HasMaxLength(100);

                entity.Property(e => e.Cost).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ConversationLog>(entity =>
            {
                entity.Property(e => e.Comment).HasMaxLength(500);

                entity.Property(e => e.Commission).HasColumnType("numeric(18, 5)");

                entity.Property(e => e.Cost).HasColumnType("numeric(18, 5)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.WaId).HasMaxLength(50);
            });

            modelBuilder.Entity<ConversationLogsHist>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ConversationLogs_Hist");

                entity.Property(e => e.Commission).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.Cost).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.WaId).HasMaxLength(250);
            });

            modelBuilder.Entity<ConversationMessage>(entity =>
            {
                entity.HasKey(e => e.MessageId)
                    .HasName("PK_ConversationMessage");

                entity.HasIndex(e => e.ConversationId, "IX_ConversationMessages_ConversationId");

                entity.Property(e => e.ContextWaId).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveredDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveredTime).HasColumnType("datetime");

                entity.Property(e => e.FailedTime).HasColumnType("datetime");

                entity.Property(e => e.ModuleId).HasDefaultValueSql("((0))");

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.ReadDate).HasColumnType("datetime");

                entity.Property(e => e.ReadTime).HasColumnType("datetime");

                entity.Property(e => e.WaId).HasMaxLength(250);
            });

            modelBuilder.Entity<ConversationMessagesHist>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ConversationMessages_Hist");

                entity.Property(e => e.ContextWaId).HasMaxLength(255);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveredDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveredTime).HasColumnType("datetime");

                entity.Property(e => e.FailedTime).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.ReadDate).HasColumnType("datetime");

                entity.Property(e => e.ReadTime).HasColumnType("datetime");

                entity.Property(e => e.WaId).HasMaxLength(255);
            });

            modelBuilder.Entity<ConversationsAgentSummary>(entity =>
            {
                entity.ToTable("ConversationsAgentSummary");

                entity.Property(e => e.AgentName).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Rating).HasColumnType("numeric(18, 1)");

                entity.Property(e => e.RecordDate).HasColumnType("date");

                entity.Property(e => e.TimeZone).HasMaxLength(7);
            });

            modelBuilder.Entity<ConversationsHist>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Conversations_Hist");

                entity.Property(e => e.Commission).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ConversationId).HasMaxLength(255);

                entity.Property(e => e.ConversationMode).HasMaxLength(255);

                entity.Property(e => e.Cost).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.ForceClosedDate).HasColumnType("datetime");

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.Language)
                    .HasMaxLength(50)
                    .HasColumnName("language");

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.Udf1)
                    .HasMaxLength(50)
                    .HasColumnName("UDF1");

                entity.Property(e => e.Udf2)
                    .HasMaxLength(50)
                    .HasColumnName("UDF2");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.WaId).HasMaxLength(255);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Fbmarket)
                    .HasMaxLength(150)
                    .HasColumnName("FBMarket");

                entity.Property(e => e.Iso)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("iso")
                    .IsFixedLength();

                entity.Property(e => e.Iso3)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("iso3")
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Nicename)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("nicename");

                entity.Property(e => e.Numcode).HasColumnName("numcode");

                entity.Property(e => e.Phonecode).HasColumnName("phonecode");
            });

            modelBuilder.Entity<Flow>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DataApiVersion).HasMaxLength(50);

                entity.Property(e => e.EndpointUrl)
                    .HasMaxLength(250)
                    .HasColumnName("EndpointURL");

                entity.Property(e => e.FlowJson).HasColumnType("text");

                entity.Property(e => e.FlowLanguage).HasMaxLength(50);

                entity.Property(e => e.FlowName).HasMaxLength(50);

                entity.Property(e => e.IsPublished).HasDefaultValueSql("((0))");

                entity.Property(e => e.MetaFlowId).HasMaxLength(50);

                entity.Property(e => e.MetaFlowName).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Version).HasMaxLength(50);
            });

            modelBuilder.Entity<FlowChildren>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Required).HasDefaultValueSql("((0))");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<FlowOption>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Metadata).HasMaxLength(50);

                entity.Property(e => e.OptionId).HasMaxLength(150);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<FlowScreen>(entity =>
            {
                entity.ToTable("FlowScreen");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.Payload).HasMaxLength(50);

                entity.Property(e => e.RedirectionScreen).HasMaxLength(50);

                entity.Property(e => e.ScreenButtonText).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(250);

                entity.Property(e => e.Type).HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.GroupName).HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<InteractiveTemplate>(entity =>
            {
                entity.Property(e => e.BodyText).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FooterText).HasMaxLength(250);

                entity.Property(e => e.HeaderText).HasMaxLength(250);

                entity.Property(e => e.Language).HasMaxLength(50);

                entity.Property(e => e.TemplateName).HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<InteractiveTemplateButton>(entity =>
            {
                entity.HasKey(e => e.ButtonId)
                    .HasName("PK__InteractiveTemplate__C4B2843895F26480");

                entity.Property(e => e.ButtonText).HasMaxLength(150);

                entity.Property(e => e.ButtonValue).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<InteractiveTemplateParameter>(entity =>
            {
                entity.HasKey(e => e.ParamId)
                    .HasName("PK__InteractiveParameter__C4B2843895F26480");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ParamName).HasMaxLength(150);

                entity.Property(e => e.PersonalizationDefaultValue).HasMaxLength(200);

                entity.Property(e => e.PersonalizationField).HasMaxLength(200);
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.Property(e => e.ArflowId).HasColumnName("ARFlowId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeprecatedDate).HasColumnType("datetime");

                entity.Property(e => e.DescriptionAr).HasMaxLength(500);

                entity.Property(e => e.DescriptionEn).HasMaxLength(500);

                entity.Property(e => e.EnflowId).HasColumnName("ENFlowId");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(100)
                    .IsFixedLength();

                entity.Property(e => e.IntegrationId).HasMaxLength(100);

                entity.Property(e => e.NameAr).HasMaxLength(100);

                entity.Property(e => e.NameEn).HasMaxLength(100);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ItemModifierMap>(entity =>
            {
                entity.ToTable("ItemModifierMap");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeprecatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.LanguageCode).HasMaxLength(20);

                entity.Property(e => e.LanguageName).HasMaxLength(100);
            });

            modelBuilder.Entity<MasterDatum>(entity =>
            {
                entity.HasKey(e => e.MastId)
                    .HasName("PK__MasterDa__C58CEA2C0F81DE11");

                entity.Property(e => e.MastId).HasColumnName("Mast_Id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Media>(entity =>
            {
                entity.Property(e => e.ContentType).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.FileExtension).HasMaxLength(50);

                entity.Property(e => e.FileName).HasMaxLength(250);

                entity.Property(e => e.MediaId).HasMaxLength(250);

                entity.Property(e => e.MediaPath).HasMaxLength(250);

                entity.Property(e => e.MediaUrl)
                    .HasMaxLength(250)
                    .HasColumnName("MediaURL");

                entity.Property(e => e.WhatsAppBusinessAccountId).HasMaxLength(100);
            });

            modelBuilder.Entity<MessageReceivedLog>(entity =>
            {
                entity.Property(e => e.Commission).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.ContextWaId).HasMaxLength(250);

                entity.Property(e => e.ConversationId).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.MessageSentDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.WaId).HasMaxLength(250);
            });

            modelBuilder.Entity<MessageReceivedLogsHist>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MessageReceivedLogs_Hist");

                entity.Property(e => e.Commission).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ContextWaId).HasMaxLength(255);

                entity.Property(e => e.ConversationId).HasMaxLength(255);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.MessageSentDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.WaId).HasMaxLength(255);
            });

            modelBuilder.Entity<MessageSentLog>(entity =>
            {
                entity.HasIndex(e => e.Id, "IX_MessageSentLogs_WAID");

                entity.Property(e => e.Category).HasMaxLength(50);

                entity.Property(e => e.Commission).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveredDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveredMessage).HasMaxLength(250);

                entity.Property(e => e.EstPrice).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.FailedTime).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.PricingModel).HasMaxLength(100);

                entity.Property(e => e.ReadDate).HasColumnType("datetime");

                entity.Property(e => e.ReadMessage).HasMaxLength(250);

                entity.Property(e => e.SentDate).HasColumnType("datetime");

                entity.Property(e => e.SentMessage).HasMaxLength(250);

                entity.Property(e => e.WaId).HasMaxLength(250);

                entity.Property(e => e.WaId2).HasMaxLength(250);
            });

            modelBuilder.Entity<MessageSentLogsHist>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MessageSentLogs_Hist");

                entity.Property(e => e.Commission).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveredDate).HasColumnType("datetime");

                entity.Property(e => e.EstPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.FailedTime).HasColumnType("datetime");

                entity.Property(e => e.ReadDate).HasColumnType("datetime");

                entity.Property(e => e.SentDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<MessagesentlogsStatusUpdate>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Messagesentlogs_StatusUpdate");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EventTime).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Message).HasMaxLength(250);

                entity.Property(e => e.WaId).HasMaxLength(250);
            });

            modelBuilder.Entity<MessagesentlogsStatusUpdateHist>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Messagesentlogs_StatusUpdate_Hist");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EventTime).HasColumnType("datetime");

                entity.Property(e => e.Message).HasMaxLength(250);

                entity.Property(e => e.WaId).HasMaxLength(250);
            });

            modelBuilder.Entity<ModifierGroup>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeprecatedDate).HasColumnType("datetime");

                entity.Property(e => e.DescriptionAr).HasMaxLength(500);

                entity.Property(e => e.DescriptionEn).HasMaxLength(500);

                entity.Property(e => e.IntegrationId).HasMaxLength(100);

                entity.Property(e => e.NameAr).HasMaxLength(100);

                entity.Property(e => e.NameEn).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ModifierItemMap>(entity =>
            {
                entity.ToTable("ModifierItemMap");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeprecatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.ToTable("Module");

                entity.HasIndex(e => e.ModuleCode, "Module_ModuleCode")
                    .IsUnique();

                entity.HasIndex(e => e.ModuleId, "Module_ModuleId")
                    .IsUnique();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModuleCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ModuleName).HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveryCharges).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.Discount).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.Language).HasMaxLength(2);

                entity.Property(e => e.LastTouchDate).HasColumnType("datetime");

                entity.Property(e => e.MetaOrderId).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(150);

                entity.Property(e => e.NextReminderTime).HasColumnType("datetime");

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentGatewayType).HasMaxLength(20);

                entity.Property(e => e.PaymentRefNo).HasMaxLength(200);

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.RetryPostCount).HasDefaultValueSql("((0))");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.Subtotal).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.Total).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.WaId).HasMaxLength(150);
            });

            modelBuilder.Entity<OrderAddress>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PK__OrderAdd__C3905BCF54BCA6B1");

                entity.ToTable("OrderAddress");

                entity.Property(e => e.OrderId).ValueGeneratedNever();

                entity.Property(e => e.Block).HasMaxLength(100);

                entity.Property(e => e.Cordinates).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Direction).HasMaxLength(250);

                entity.Property(e => e.FlatNo).HasMaxLength(100);

                entity.Property(e => e.Floor).HasMaxLength(100);

                entity.Property(e => e.House).HasMaxLength(100);

                entity.Property(e => e.Street).HasMaxLength(100);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ItemName).HasMaxLength(100);

                entity.Property(e => e.Price).HasColumnType("numeric(18, 3)");
            });

            modelBuilder.Entity<OrderStep>(entity =>
            {
                entity.Property(e => e.BodyText).HasMaxLength(250);

                entity.Property(e => e.ButtonText).HasMaxLength(20);

                entity.Property(e => e.FlowToken).HasMaxLength(250);

                entity.Property(e => e.ItemName).HasMaxLength(200);

                entity.Property(e => e.Language).HasMaxLength(2);

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.StepName).HasMaxLength(250);
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permission");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<PermissionTask>(entity =>
            {
                entity.Property(e => e.PermissionTaskId).ValueGeneratedNever();

                entity.Property(e => e.PermissionTaskName).HasMaxLength(200);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.RoleName).HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<SenderName>(entity =>
            {
                entity.HasKey(e => e.SenderId)
                    .HasName("PK__SenderNa__484F1F3C90706B69");

                entity.ToTable("SenderName");

                entity.Property(e => e.AccessToken).HasMaxLength(250);

                entity.Property(e => e.AppId).HasMaxLength(100);

                entity.Property(e => e.BusinessAccountId).HasMaxLength(50);

                entity.Property(e => e.BusinessId).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Limit).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.PhoneNumberId).HasMaxLength(50);

                entity.Property(e => e.PrivateCertificate).HasMaxLength(4000);

                entity.Property(e => e.PublicCertificate).HasMaxLength(4000);

                entity.Property(e => e.Quality).HasMaxLength(50);

                entity.Property(e => e.SenderName1)
                    .HasMaxLength(250)
                    .HasColumnName("SenderName");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.WebsiteUrl).HasMaxLength(200);
            });

            modelBuilder.Entity<SignalRqueue>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SignalRQueue");

                entity.Property(e => e.AgentName).HasMaxLength(250);

                entity.Property(e => e.CompletedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FullName).HasMaxLength(250);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.SenderName).HasMaxLength(250);

                entity.Property(e => e.SenderPhoneNumber).HasMaxLength(250);

                entity.Property(e => e.SignalName).HasMaxLength(100);
            });

            modelBuilder.Entity<Survey>(entity =>
            {
                entity.ToTable("Survey");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.MetaFlowId).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<SurveyResponse>(entity =>
            {
                entity.ToTable("SurveyResponse");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FlowToken).HasMaxLength(250);

                entity.Property(e => e.MetaFlowId).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            });

            modelBuilder.Entity<SurveyResponseDetail>(entity =>
            {
                entity.Property(e => e.AnswerKey).HasMaxLength(500);

                entity.Property(e => e.QuestionKey).HasMaxLength(500);

                entity.Property(e => e.Type).HasMaxLength(50);
            });

            modelBuilder.Entity<SystemAction>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ActionName).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.SystemActionId).ValueGeneratedOnAdd();

                entity.Property(e => e.ThirdPartyUrl)
                    .HasMaxLength(250)
                    .HasColumnName("ThirdPartyURL");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Template>(entity =>
            {
                entity.Property(e => e.BodyText).HasMaxLength(500);

                entity.Property(e => e.Category).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FooterText).HasMaxLength(250);

                entity.Property(e => e.HeaderText).HasMaxLength(250);

                entity.Property(e => e.Language).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.SubCategory).HasMaxLength(50);

                entity.Property(e => e.TemplateId).HasMaxLength(50);

                entity.Property(e => e.TemplateName).HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TemplateAnalytic>(entity =>
            {
                entity.Property(e => e.AmountSpent)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Amount_Spent");

                entity.Property(e => e.CostPerDelivered).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.CostPerUrlButtonClick).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RecordDate).HasColumnType("date");

                entity.Property(e => e.TemplateId).HasMaxLength(250);
            });

            modelBuilder.Entity<TemplateButton>(entity =>
            {
                entity.HasKey(e => e.ButtonId)
                    .HasName("PK__TemplateButton__C5B2843895F26480");

                entity.Property(e => e.ButtonText).HasMaxLength(150);

                entity.Property(e => e.ButtonValue).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TemplateParameter>(entity =>
            {
                entity.HasKey(e => e.ParamId)
                    .HasName("PK__Template__C4B2843895F26480");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ParamDefaultValue).HasMaxLength(50);

                entity.Property(e => e.ParamName).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TemplateResponse>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.WaId).HasMaxLength(250);
            });

            modelBuilder.Entity<TemplateResponsesHist>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TemplateResponses_Hist");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.WaId).HasMaxLength(250);
            });

            modelBuilder.Entity<TemplateScreen>(entity =>
            {
                entity.Property(e => e.BodyText).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FooterText).HasMaxLength(250);

                entity.Property(e => e.HeaderText).HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TemplatesAnalyticsDetail>(entity =>
            {
                entity.Property(e => e.ButtonText).HasMaxLength(150);

                entity.Property(e => e.ButtonType).HasMaxLength(150);
            });

            modelBuilder.Entity<UnsubscribedNumber>(entity =>
            {
                entity.Property(e => e.BlockType).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.UserName, "UserNameUnique")
                    .IsUnique();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmailAddress).HasMaxLength(250);

                entity.Property(e => e.FullName).HasMaxLength(250);

                entity.Property(e => e.Hash).HasMaxLength(250);

                entity.Property(e => e.Password).HasMaxLength(250);

                entity.Property(e => e.PhoneNumber).HasMaxLength(250);

                entity.Property(e => e.RefreshTokenExpiry).HasColumnType("datetime");

                entity.Property(e => e.Salt).HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(250);
            });

            modelBuilder.Entity<UsersRole>(entity =>
            {
                entity.HasKey(e => e.UserRoleId)
                    .HasName("PK__UsersRol__134E488CB5536341");

                entity.Property(e => e.CreatedBy).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<WhatsAppPricing>(entity =>
            {
                entity.HasKey(e => e.PricingId);

                entity.ToTable("WhatsAppPricing");

                entity.Property(e => e.Authentication).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.AuthenticationCoin).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.AuthenticationCommission).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.AuthenticationInternational).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.AuthenticationInternationalCommission).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Currency).HasMaxLength(50);

                entity.Property(e => e.Market).HasMaxLength(250);

                entity.Property(e => e.Marketing).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.MarketingCoin).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.MarketingCommission).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Service).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.ServiceCoin).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.ServiceCommission).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Utility).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.UtilityCoin).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.UtilityCommission).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Waorder)
                    .HasColumnType("decimal(18, 3)")
                    .HasColumnName("WAOrder");

                entity.Property(e => e.WaorderCoin)
                    .HasColumnType("decimal(18, 3)")
                    .HasColumnName("WAOrderCoin");

                entity.Property(e => e.WaorderCommission)
                    .HasColumnType("decimal(18, 3)")
                    .HasColumnName("WAOrderCommission");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
