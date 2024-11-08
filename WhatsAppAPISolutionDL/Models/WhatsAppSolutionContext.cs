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

        public virtual DbSet<Campaign> Campaigns { get; set; }
        public virtual DbSet<CampaignContact> CampaignContacts { get; set; }
        public virtual DbSet<CampaignParam> CampaignParams { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Medium> Media { get; set; }
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
            modelBuilder.Entity<Campaign>(entity =>
            {
                entity.Property(e => e.CampaignId).HasColumnName("Campaign_Id");

                entity.Property(e => e.CampaignName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("Campaign_Name");

                entity.Property(e => e.CampaignType)
                    .HasMaxLength(50)
                    .HasColumnName("Campaign_Type");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.Commission).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.Cost).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeliveredCount).HasColumnName("Delivered_Count");

                entity.Property(e => e.EstimatedCost).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.FailedCount).HasColumnName("Failed_Count");

                entity.Property(e => e.ScheduleDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Schedule_Date");

                entity.Property(e => e.SenderId).HasColumnName("Sender_Id");

                entity.Property(e => e.SentCount).HasColumnName("Sent_Count");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.TemplateId).HasColumnName("Template_Id");

                entity.Property(e => e.TotalCost).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.UndeliveredCount).HasColumnName("Undelivered_Count");
            });

            modelBuilder.Entity<CampaignContact>(entity =>
            {
                entity.HasKey(e => e.CampaignNumberId)
                    .HasName("PK__Campaign__2BC9C5EFE1E9708F");

                entity.Property(e => e.CampaignNumberId).HasColumnName("CampaignNumber_Id");

                entity.Property(e => e.Area).HasMaxLength(255);

                entity.Property(e => e.CampaignId).HasColumnName("Campaign_Id");

                entity.Property(e => e.Cost).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeliveredTime).HasMaxLength(250);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255)
                    .HasColumnName("First_Name");

                entity.Property(e => e.GroupId).HasColumnName("Group_Id");

                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .HasColumnName("Last_Name");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .HasColumnName("Phone_Number");

                entity.Property(e => e.SendStatus)
                    .HasMaxLength(50)
                    .HasColumnName("Send_Status");

                entity.Property(e => e.SentTime).HasMaxLength(250);

                entity.Property(e => e.WaId)
                    .HasMaxLength(250)
                    .HasColumnName("wa_id");
            });

            modelBuilder.Entity<CampaignParam>(entity =>
            {
                entity.Property(e => e.CampaignParamId).HasColumnName("CampaignParam_Id");

                entity.Property(e => e.CampaignId).HasColumnName("Campaign_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ParamDefaultValue)
                    .HasMaxLength(50)
                    .HasColumnName("Param_Default_Value");

                entity.Property(e => e.ParamName)
                    .HasMaxLength(255)
                    .HasColumnName("Param_Name");

                entity.Property(e => e.ParamText)
                    .HasMaxLength(255)
                    .HasColumnName("Param_Text");

                entity.Property(e => e.ParamType).HasColumnName("Param_Type");
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.AccessToken)
                    .HasMaxLength(250)
                    .HasColumnName("Access_Token");

                entity.Property(e => e.AppId)
                    .HasMaxLength(100)
                    .HasColumnName("App_Id");

                entity.Property(e => e.AvailableBalance).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.Balance).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.BalanceAlertLimit)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Balance_Alert_Limit");

                entity.Property(e => e.BusinessId)
                    .HasMaxLength(100)
                    .HasColumnName("Business_Id");

                entity.Property(e => e.ClientAddress).HasColumnName("Client_Address");

                entity.Property(e => e.ClientLanguage).HasColumnName("Client_Language");

                entity.Property(e => e.ClientName)
                    .HasMaxLength(50)
                    .HasColumnName("Client_Name");

                entity.Property(e => e.ContactPerson)
                    .HasMaxLength(150)
                    .HasColumnName("Contact_Person");

                entity.Property(e => e.ContactPersonEmail)
                    .HasMaxLength(150)
                    .HasColumnName("Contact_Person_Email");

                entity.Property(e => e.ContactPersonPhone)
                    .HasMaxLength(150)
                    .HasColumnName("Contact_Person_Phone");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.DefaultMarket).HasMaxLength(150);

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contact");

                entity.Property(e => e.ContactId).HasColumnName("Contact_Id");

                entity.Property(e => e.AreaName)
                    .HasMaxLength(50)
                    .HasColumnName("Area_Name");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(50)
                    .HasColumnName("Email_Address");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasColumnName("First_Name");

                entity.Property(e => e.GroupId).HasColumnName("Group_Id");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .HasColumnName("Last_Name");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .HasColumnName("Phone_Number");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("countries");

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
                entity.ToTable("Group");

                entity.Property(e => e.GroupId).HasColumnName("Group_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.GroupName)
                    .HasMaxLength(50)
                    .HasColumnName("Group_Name");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
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

            modelBuilder.Entity<Medium>(entity =>
            {
                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.ContentType)
                    .HasMaxLength(250)
                    .HasColumnName("Content_Type");

                entity.Property(e => e.FileExtension)
                    .HasMaxLength(50)
                    .HasColumnName("File_Extension");

                entity.Property(e => e.FileName)
                    .HasMaxLength(250)
                    .HasColumnName("File_Name");

                entity.Property(e => e.FileSize).HasColumnName("File_Size");

                entity.Property(e => e.MediaId)
                    .HasMaxLength(250)
                    .HasColumnName("Media_Id");

                entity.Property(e => e.MediaPath)
                    .HasMaxLength(250)
                    .HasColumnName("Media_Path");

                entity.Property(e => e.MediaUrl)
                    .HasMaxLength(250)
                    .HasColumnName("Media_URL");

                entity.Property(e => e.SenderNameId).HasColumnName("Sender_Name_Id");

                entity.Property(e => e.WhatsAppBusinessAccountId)
                    .HasMaxLength(100)
                    .HasColumnName("WhatsApp_Business_Account_Id");
            });

            modelBuilder.Entity<MessageSentLog>(entity =>
            {
                entity.Property(e => e.Billable).HasColumnName("billable");

                entity.Property(e => e.Category)
                    .HasMaxLength(50)
                    .HasColumnName("category");

                entity.Property(e => e.Commission).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_at");

                entity.Property(e => e.DeliveredDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveredMessage).HasMaxLength(250);

                entity.Property(e => e.EstPrice).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.ParentId).HasColumnName("Parent_Id");

                entity.Property(e => e.PhoneNumber).HasMaxLength(100);

                entity.Property(e => e.PricingModel).HasMaxLength(100);

                entity.Property(e => e.ReadDate).HasColumnType("datetime");

                entity.Property(e => e.ReadMessage).HasMaxLength(250);

                entity.Property(e => e.SentDate).HasColumnType("datetime");

                entity.Property(e => e.SentMessage).HasMaxLength(250);

                entity.Property(e => e.WaId)
                    .HasMaxLength(250)
                    .HasColumnName("Wa_id");

                entity.Property(e => e.WaId2)
                    .HasMaxLength(250)
                    .HasColumnName("Wa_id2");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.ToTable("Module");

                entity.HasIndex(e => e.ModuleCode, "Module_ModuleCode")
                    .IsUnique();

                entity.HasIndex(e => e.ModuleId, "Module_ModuleId")
                    .IsUnique();

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.ModuleCode)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("Module_Code");

                entity.Property(e => e.ModuleId).HasColumnName("Module_Id");

                entity.Property(e => e.ModuleName)
                    .HasMaxLength(250)
                    .HasColumnName("Module_Name");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.Property(e => e.PageId).HasColumnName("Page_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.PageName).HasColumnName("Page_Name");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permission");

                entity.Property(e => e.CanCreate).HasColumnName("Can_Create");

                entity.Property(e => e.CanDelete).HasColumnName("Can_Delete");

                entity.Property(e => e.CanUpdate).HasColumnName("Can_Update");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.RoleId).HasColumnName("Role_Id");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<PermissionTask>(entity =>
            {
                entity.Property(e => e.PermissionTaskName).HasMaxLength(200);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId).HasColumnName("Role_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(250)
                    .HasColumnName("Role_Name");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<SenderName>(entity =>
            {
                entity.HasKey(e => e.SenderId)
                    .HasName("PK__SenderNa__484F1F3C90706B69");

                entity.ToTable("SenderName");

                entity.Property(e => e.SenderId).HasColumnName("Sender_Id");

                entity.Property(e => e.BusinessAccountId)
                    .HasMaxLength(50)
                    .HasColumnName("Business_Account_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.Limit).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .HasColumnName("Phone_Number");

                entity.Property(e => e.PhoneNumberId)
                    .HasMaxLength(50)
                    .HasColumnName("Phone_Number_Id");

                entity.Property(e => e.Quality).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.SenderName1)
                    .HasMaxLength(50)
                    .HasColumnName("Sender_Name");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<Template>(entity =>
            {
                entity.HasKey(e => e.TemplatesId)
                    .HasName("PK__Template__EF4B6BB8120F29E2");

                entity.Property(e => e.TemplatesId).HasColumnName("Templates_Id");

                entity.Property(e => e.BodyParamCount).HasColumnName("Body_Param_Count");

                entity.Property(e => e.BodyText)
                    .HasMaxLength(250)
                    .HasColumnName("Body_Text");

                entity.Property(e => e.Category).HasMaxLength(50);

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.FooterText)
                    .HasMaxLength(250)
                    .HasColumnName("Footer_Text");

                entity.Property(e => e.HeaderParamCount).HasColumnName("Header_Param_Count");

                entity.Property(e => e.HeaderText)
                    .HasMaxLength(250)
                    .HasColumnName("Header_Text");

                entity.Property(e => e.HeaderType).HasColumnName("Header_Type");

                entity.Property(e => e.IntegrationId)
                    .HasMaxLength(50)
                    .HasColumnName("Integration_Id");

                entity.Property(e => e.IsApproved).HasColumnName("Is_Approved");

                entity.Property(e => e.Language).HasMaxLength(50);

                entity.Property(e => e.MediaId)
                    .HasMaxLength(50)
                    .HasColumnName("Media_Id");

                entity.Property(e => e.SenderId).HasColumnName("Sender_Id");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.SubCategory).HasMaxLength(50);

                entity.Property(e => e.TemplateAttachmentUrl)
                    .HasMaxLength(250)
                    .HasColumnName("Template_Attachment_URL");

                entity.Property(e => e.TemplateId)
                    .HasMaxLength(50)
                    .HasColumnName("Template_Id");

                entity.Property(e => e.TemplateName)
                    .HasMaxLength(50)
                    .HasColumnName("Template_Name");

                entity.Property(e => e.TemplateSendType).HasColumnName("Template_Send_Type");

                entity.Property(e => e.TransactionType).HasMaxLength(50);

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<TemplateParameter>(entity =>
            {
                entity.HasKey(e => e.ParamId)
                    .HasName("PK__Template__C4B2843895F26480");

                entity.ToTable("Template_Parameters");

                entity.Property(e => e.ParamId).HasColumnName("Param_Id");

                entity.Property(e => e.ButtonType).HasColumnName("Button_Type");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.ContactLevelField).HasMaxLength(100);

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.ParamDefaultValue)
                    .HasMaxLength(50)
                    .HasColumnName("Param_Default_Value");

                entity.Property(e => e.ParamName)
                    .HasMaxLength(50)
                    .HasColumnName("Param_Name");

                entity.Property(e => e.ParamText)
                    .HasMaxLength(250)
                    .HasColumnName("Param_Text");

                entity.Property(e => e.ParamType).HasColumnName("Param_Type");

                entity.Property(e => e.TemplatesId).HasColumnName("Templates_Id");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<TemplateParametersConfig>(entity =>
            {
                entity.ToTable("template_Parameters_config");

                entity.Property(e => e.ParamName)
                    .HasMaxLength(200)
                    .HasColumnName("Param_Name");

                entity.Property(e => e.ParamSeq).HasColumnName("Param_Seq");

                entity.Property(e => e.ParamText)
                    .HasMaxLength(100)
                    .HasColumnName("param_Text");

                entity.Property(e => e.ParamType).HasColumnName("Param_Type");

                entity.Property(e => e.TemplateId).HasColumnName("Template_Id");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("User_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(250)
                    .HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.FullName).HasMaxLength(250);

                entity.Property(e => e.Hash).HasMaxLength(250);

                entity.Property(e => e.Password).HasMaxLength(250);

                entity.Property(e => e.RefreshTokenExpiry).HasColumnType("datetime");

                entity.Property(e => e.Salt).HasMaxLength(250);

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");

                entity.Property(e => e.UserName).HasMaxLength(250);
            });

            modelBuilder.Entity<UsersRole>(entity =>
            {
                entity.HasKey(e => e.UserRoleId)
                    .HasName("PK__UsersRol__134E488CB5536341");

                entity.Property(e => e.UserRoleId).HasColumnName("User_Role_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(250)
                    .HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<WhatsAppPricing>(entity =>
            {
                entity.HasKey(e => e.PricingId);

                entity.ToTable("WhatsAppPricing");

                entity.Property(e => e.PricingId).HasColumnName("Pricing_Id");

                entity.Property(e => e.Authentication).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.AuthenticationCommission)
                    .HasColumnType("decimal(18, 3)")
                    .HasColumnName("Authentication_Commission");

                entity.Property(e => e.AuthenticationInternational)
                    .HasColumnType("decimal(18, 3)")
                    .HasColumnName("Authentication_International");

                entity.Property(e => e.AuthenticationInternationalCommission)
                    .HasColumnType("decimal(18, 3)")
                    .HasColumnName("Authentication_International_Commission");

                entity.Property(e => e.Currency).HasMaxLength(50);

                entity.Property(e => e.Market).HasMaxLength(250);

                entity.Property(e => e.Marketing).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.MarketingCommission)
                    .HasColumnType("decimal(18, 3)")
                    .HasColumnName("Marketing_Commission");

                entity.Property(e => e.Service).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.ServiceCommission)
                    .HasColumnType("decimal(18, 3)")
                    .HasColumnName("Service_Commission");

                entity.Property(e => e.Utility).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.UtilityCommission)
                    .HasColumnType("decimal(18, 3)")
                    .HasColumnName("Utility_Commission");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
