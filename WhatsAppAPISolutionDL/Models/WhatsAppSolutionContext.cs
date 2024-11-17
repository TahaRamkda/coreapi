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

        public virtual DbSet<Apimessage> Apimessages { get; set; }
        public virtual DbSet<Campaign> Campaigns { get; set; }
        public virtual DbSet<CampaignContact> CampaignContacts { get; set; }
        public virtual DbSet<CampaignParam> CampaignParams { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<ConversationMessage> ConversationMessages { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Media> Medias { get; set; }
        public virtual DbSet<MessageSentLog> MessageSentLogs { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PermissionTask> PermissionTasks { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SenderName> SenderNames { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<TemplateParameter> TemplateParameters { get; set; }
        public virtual DbSet<TemplateParametersConfig> TemplateParametersConfigs { get; set; }
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
            modelBuilder.Entity<Apimessage>(entity =>
            {
                entity.ToTable("APIMessages");

                entity.Property(e => e.ApimessageId).HasColumnName("APIMessageId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.ScheduleTime).HasColumnType("datetime");

                entity.Property(e => e.TrxType).HasMaxLength(250);

                entity.Property(e => e.Udf1)
                    .HasMaxLength(50)
                    .HasColumnName("UDF1");

                entity.Property(e => e.Udf2)
                    .HasMaxLength(50)
                    .HasColumnName("UDF2");

                entity.Property(e => e.Url)
                    .HasMaxLength(250)
                    .HasColumnName("URL");

                entity.Property(e => e.WaId)
                    .HasMaxLength(250)
                    .HasColumnName("WaID");
            });

            modelBuilder.Entity<Campaign>(entity =>
            {
                entity.Property(e => e.CampaignName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CampaignType).HasMaxLength(50);

                entity.Property(e => e.Commission).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.Cost).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EstimatedCost).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.ScheduleDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.TotalCost).HasColumnType("numeric(18, 3)");
            });

            modelBuilder.Entity<CampaignContact>(entity =>
            {
                entity.HasKey(e => e.CampaignNumberId)
                    .HasName("PK__Campaign__2BC9C5EFE1E9708F");

                entity.Property(e => e.Area).HasMaxLength(255);

                entity.Property(e => e.Cost).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeliveredTime).HasMaxLength(250);

                entity.Property(e => e.FirstName).HasMaxLength(255);

                entity.Property(e => e.LastName).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.SendStatus).HasMaxLength(50);

                entity.Property(e => e.SentTime).HasMaxLength(250);

                entity.Property(e => e.WaId).HasMaxLength(250);
            });

            modelBuilder.Entity<CampaignParam>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ParamDefaultValue).HasMaxLength(50);

                entity.Property(e => e.ParamName).HasMaxLength(255);

                entity.Property(e => e.ParamText).HasMaxLength(255);
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.AccessToken).HasMaxLength(250);

                entity.Property(e => e.AppId).HasMaxLength(100);

                entity.Property(e => e.AvailableBalance).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.Balance).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.BalanceAlertLimit).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.BusinessId).HasMaxLength(100);

                entity.Property(e => e.ClientName).HasMaxLength(50);

                entity.Property(e => e.ContactPerson).HasMaxLength(150);

                entity.Property(e => e.ContactPersonEmail).HasMaxLength(150);

                entity.Property(e => e.ContactPersonPhone).HasMaxLength(150);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DefaultMarket).HasMaxLength(150);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.Property(e => e.AreaName).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmailAddress).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ConversationMessage>(entity =>
            {
                entity.HasKey(e => e.MessageId);

                entity.ToTable("ConversationMessage");

                entity.Property(e => e.ConversationId).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.From).HasMaxLength(50);

                entity.Property(e => e.To).HasMaxLength(50);
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

            modelBuilder.Entity<Group>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.GroupName).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Action).HasMaxLength(200);

                entity.Property(e => e.ClientId)
                    .HasMaxLength(50)
                    .HasColumnName("Client_Id");

                entity.Property(e => e.Controller).HasMaxLength(200);

                entity.Property(e => e.Identifier).HasMaxLength(200);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            });

            modelBuilder.Entity<Media>(entity =>
            {
                entity.Property(e => e.ContentType).HasMaxLength(250);

                entity.Property(e => e.FileExtension).HasMaxLength(50);

                entity.Property(e => e.FileName).HasMaxLength(250);

                entity.Property(e => e.MediaId).HasMaxLength(250);

                entity.Property(e => e.MediaPath).HasMaxLength(250);

                entity.Property(e => e.MediaUrl)
                    .HasMaxLength(250)
                    .HasColumnName("MediaURL");

                entity.Property(e => e.WhatsAppBusinessAccountId).HasMaxLength(100);
            });

            modelBuilder.Entity<MessageSentLog>(entity =>
            {
                entity.Property(e => e.Category).HasMaxLength(50);

                entity.Property(e => e.Commission).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveredDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveredMessage).HasMaxLength(250);

                entity.Property(e => e.EstPrice).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.PhoneNumber).HasMaxLength(100);

                entity.Property(e => e.PricingModel).HasMaxLength(100);

                entity.Property(e => e.ReadDate).HasColumnType("datetime");

                entity.Property(e => e.ReadMessage).HasMaxLength(250);

                entity.Property(e => e.SentDate).HasColumnType("datetime");

                entity.Property(e => e.SentMessage).HasMaxLength(250);

                entity.Property(e => e.WaId).HasMaxLength(250);

                entity.Property(e => e.WaId2).HasMaxLength(250);
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

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.PhoneNumberId).HasMaxLength(50);

                entity.Property(e => e.Quality).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.SenderName1)
                    .HasMaxLength(50)
                    .HasColumnName("SenderName");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Template>(entity =>
            {
                entity.HasKey(e => e.TemplatesId)
                    .HasName("PK__Template__EF4B6BB8120F29E2");

                entity.Property(e => e.BodyText).HasMaxLength(250);

                entity.Property(e => e.Category).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FooterText).HasMaxLength(250);

                entity.Property(e => e.HeaderText).HasMaxLength(250);

                entity.Property(e => e.IntegrationId).HasMaxLength(50);

                entity.Property(e => e.Language).HasMaxLength(50);

                entity.Property(e => e.MediaId).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.SubCategory).HasMaxLength(50);

                entity.Property(e => e.TemplateAttachmentUrl)
                    .HasMaxLength(250)
                    .HasColumnName("TemplateAttachmentURL");

                entity.Property(e => e.TemplateId).HasMaxLength(50);

                entity.Property(e => e.TemplateName).HasMaxLength(50);

                entity.Property(e => e.TransactionType).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TemplateParameter>(entity =>
            {
                entity.HasKey(e => e.ParamId)
                    .HasName("PK__Template__C4B2843895F26480");

                entity.Property(e => e.ContactLevelField).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ParamDefaultValue).HasMaxLength(50);

                entity.Property(e => e.ParamName).HasMaxLength(50);

                entity.Property(e => e.ParamText).HasMaxLength(250);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TemplateParametersConfig>(entity =>
            {
                entity.ToTable("TemplateParametersConfig");

                entity.Property(e => e.ParamName).HasMaxLength(200);

                entity.Property(e => e.ParamText).HasMaxLength(100);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreatedBy).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FullName).HasMaxLength(250);

                entity.Property(e => e.Hash).HasMaxLength(250);

                entity.Property(e => e.Password).HasMaxLength(250);

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

                entity.Property(e => e.AuthenticationCommission).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.AuthenticationInternational).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.AuthenticationInternationalCommission)
                    .HasColumnType("decimal(18, 3)")
                    .HasColumnName("AuthenticationInternational_Commission");

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
