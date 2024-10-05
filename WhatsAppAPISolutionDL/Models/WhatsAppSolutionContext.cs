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

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PermissionTask> PermissionTasks { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SenderName> SenderNames { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<TemplateParameter> TemplateParameters { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersRole> UsersRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:WhatsAppAPISolutionDataBase");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.AccessToken)
                    .HasMaxLength(250)
                    .HasColumnName("Access_Token");

                entity.Property(e => e.Balance).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.BalanceAlertLimit)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Balance_Alert_Limit");

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

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contact");

                entity.Property(e => e.ContactId).HasColumnName("Contact_Id");

                entity.Property(e => e.AreaName)
                    .HasMaxLength(50)
                    .HasColumnName("Area_Name");

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

            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("Group");

                entity.Property(e => e.GroupId).HasColumnName("Group_Id");

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

                entity.Property(e => e.AppId)
                    .HasMaxLength(50)
                    .HasColumnName("App_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.Limit).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.PhoneId)
                    .HasMaxLength(50)
                    .HasColumnName("Phone_Id");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .HasColumnName("Phone_Number");

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

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.IntegrationId)
                    .HasMaxLength(50)
                    .HasColumnName("Integration_Id");

                entity.Property(e => e.TemplateId)
                    .HasMaxLength(50)
                    .HasColumnName("Template_Id");

                entity.Property(e => e.TemplateName)
                    .HasMaxLength(50)
                    .HasColumnName("Template_Name");

                entity.Property(e => e.TemplateType).HasColumnName("Template_Type");

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

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.ParamDefaultValue)
                    .HasMaxLength(50)
                    .HasColumnName("Param_DefaultValue");

                entity.Property(e => e.ParamName)
                    .HasMaxLength(50)
                    .HasColumnName("Param_Name");

                entity.Property(e => e.ParamType).HasColumnName("param_Type");

                entity.Property(e => e.TemplatesId).HasColumnName("Templates_Id");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
