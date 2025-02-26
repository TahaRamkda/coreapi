using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class WhatsAppSolutionQAContext : DbContext
    {
        public WhatsAppSolutionQAContext()
        {
        }

        public WhatsAppSolutionQAContext(DbContextOptions<WhatsAppSolutionQAContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Agent> Agents { get; set; }
        public virtual DbSet<AgentChatReasonMap> AgentChatReasonMaps { get; set; }
        public virtual DbSet<AgentLog> AgentLogs { get; set; }
        public virtual DbSet<AgentSenderMap> AgentSenderMaps { get; set; }
        public virtual DbSet<AgentTiming> AgentTimings { get; set; }
        public virtual DbSet<Apimessage> Apimessages { get; set; }
        public virtual DbSet<Appsetting> Appsettings { get; set; }
        public virtual DbSet<Campaign> Campaigns { get; set; }
        public virtual DbSet<CampaignContact> CampaignContacts { get; set; }
        public virtual DbSet<CampaignParam> CampaignParams { get; set; }
        public virtual DbSet<CampaignResponse> CampaignResponses { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Conversation> Conversations { get; set; }
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
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<MasterDatum> MasterData { get; set; }
        public virtual DbSet<Media> Medias { get; set; }
        public virtual DbSet<MessageReceivedLog> MessageReceivedLogs { get; set; }
        public virtual DbSet<MessageReceivedLogsHist> MessageReceivedLogsHists { get; set; }
        public virtual DbSet<MessageSentLog> MessageSentLogs { get; set; }
        public virtual DbSet<MessageSentLogsHist> MessageSentLogsHists { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PermissionTask> PermissionTasks { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SenderName> SenderNames { get; set; }
        public virtual DbSet<Survey> Surveys { get; set; }
        public virtual DbSet<SurveyResponse> SurveyResponses { get; set; }
        public virtual DbSet<SurveyResponseDetail> SurveyResponseDetails { get; set; }
        public virtual DbSet<SystemAction> SystemActions { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<TemplateButton> TemplateButtons { get; set; }
        public virtual DbSet<TemplateParameter> TemplateParameters { get; set; }
        public virtual DbSet<TemplateParametersBak> TemplateParametersBaks { get; set; }
        public virtual DbSet<TemplateResponse> TemplateResponses { get; set; }
        public virtual DbSet<TemplatesBak> TemplatesBaks { get; set; }
        public virtual DbSet<UnsubscribedNumber> UnsubscribedNumbers { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersRole> UsersRoles { get; set; }
        public virtual DbSet<VwUsersRole> VwUsersRoles { get; set; }
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

                entity.Property(e => e.FirstName).HasMaxLength(250);

                entity.Property(e => e.LastName).HasMaxLength(250);

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.ReadTime).HasColumnType("datetime");

                entity.Property(e => e.SentTime).HasColumnType("datetime");

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

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DefaultMarket).HasMaxLength(150);

                entity.Property(e => e.Prefix).HasMaxLength(3);

                entity.Property(e => e.Timezone).HasMaxLength(150);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
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
                entity.Property(e => e.Commission).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.ConversationId).HasMaxLength(250);

                entity.Property(e => e.ConversationMode).HasMaxLength(50);

                entity.Property(e => e.Cost).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.ForceClosedDate).HasColumnType("datetime");

                entity.Property(e => e.FullName).HasMaxLength(200);

                entity.Property(e => e.Language).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.WaId).HasMaxLength(250);
            });

            modelBuilder.Entity<ConversationLog>(entity =>
            {
                entity.Property(e => e.Comment).HasMaxLength(500);

                entity.Property(e => e.Commission).HasColumnType("numeric(18, 5)");

                entity.Property(e => e.Cost).HasColumnType("numeric(18, 5)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.WaId).HasMaxLength(150);
            });

            modelBuilder.Entity<ConversationLogsHist>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ConversationLogs_Hist");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ConversationMessage>(entity =>
            {
                entity.HasKey(e => e.MessageId)
                    .HasName("PK_ConversationMessage");

                entity.Property(e => e.ContextWaId).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveredDate).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.ReadDate).HasColumnType("datetime");

                entity.Property(e => e.WaId).HasMaxLength(250);
            });

            modelBuilder.Entity<ConversationMessagesHist>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ConversationMessages_Hist");

                entity.Property(e => e.ContextWaId).HasMaxLength(255);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveredDate).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.ReadDate).HasColumnType("datetime");

                entity.Property(e => e.WaId).HasMaxLength(255);
            });

            modelBuilder.Entity<ConversationsAgentSummary>(entity =>
            {
                entity.HasNoKey();

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

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

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

                entity.Property(e => e.MetaFlowId).HasMaxLength(50);

                entity.Property(e => e.MetaFlowName).HasMaxLength(50);

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

                entity.Property(e => e.BusinessAccountId).HasMaxLength(50);

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

                entity.Property(e => e.ClientId).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FlowToken).HasMaxLength(250);

                entity.Property(e => e.MetaFlowId).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.SenderId).HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<SurveyResponseDetail>(entity =>
            {
                entity.Property(e => e.OptionId).HasMaxLength(250);
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

            modelBuilder.Entity<TemplateParametersBak>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TemplateParameters_BAK");

                entity.Property(e => e.ActionText).HasMaxLength(250);

                entity.Property(e => e.ButtonId).HasMaxLength(250);

                entity.Property(e => e.ContactLevelField).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ParamDefaultValue).HasMaxLength(50);

                entity.Property(e => e.ParamId).ValueGeneratedOnAdd();

                entity.Property(e => e.ParamName).HasMaxLength(50);

                entity.Property(e => e.ParamText).HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TemplateResponse>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.ResponseText).HasMaxLength(250);

                entity.Property(e => e.WaId).HasMaxLength(250);
            });

            modelBuilder.Entity<TemplatesBak>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Templates_BAK");

                entity.Property(e => e.BodyText).HasMaxLength(500);

                entity.Property(e => e.Category).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FooterText).HasMaxLength(250);

                entity.Property(e => e.HeaderText).HasMaxLength(250);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IntegrationId).HasMaxLength(50);

                entity.Property(e => e.Language).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.SubCategory).HasMaxLength(50);

                entity.Property(e => e.TemplateAttachmentUrl)
                    .HasMaxLength(250)
                    .HasColumnName("TemplateAttachmentURL");

                entity.Property(e => e.TemplateId).HasMaxLength(50);

                entity.Property(e => e.TemplateName).HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
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

            modelBuilder.Entity<VwUsersRole>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_UsersRoles");

                entity.Property(e => e.CreatedBy).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.RoleName).HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<WhatsAppPricing>(entity =>
            {
                entity.HasKey(e => e.PricingId);

                entity.ToTable("WhatsAppPricing");

                entity.Property(e => e.Authentication).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.AuthenticationCommission).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.AuthenticationInternational).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.AuthenticationInternationalCommission).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Currency).HasMaxLength(50);

                entity.Property(e => e.Market).HasMaxLength(250);

                entity.Property(e => e.Marketing).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.MarketingCommission).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Service).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.ServiceCommission).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Utility).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.UtilityCommission).HasColumnType("decimal(18, 3)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
