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
        public virtual DbSet<UDashboardSummary> DashboardSummary { get; set; } = null!;
        public virtual DbSet<UDashboardReportSummary> DashboardReportSummary { get; set; } = null!;
        public virtual DbSet<UClient> Clients { get; set; } = null!;
        public virtual DbSet<UGroup> Groups { get; set; } = null!;
        public virtual DbSet<UContact> Contacts { get; set; } = null!;
        public virtual DbSet<USenderName> SenderNames { get; set; } = null!;
        public virtual DbSet<UTemplate> Templates { get; set; } = null!;
        public virtual DbSet<UTemplateParameter> TemplateParameters { get; set; } = null!;
        public virtual DbSet<UMediaUpload> UMediaUploads { get; set; } = null!;

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
            modelBuilder.Entity<UClient>().HasNoKey();
            modelBuilder.Entity<UGroup>().HasNoKey();
            modelBuilder.Entity<UContact>().HasNoKey();
            modelBuilder.Entity<USenderName>().HasNoKey();
            modelBuilder.Entity<UTemplate>().HasNoKey();
            modelBuilder.Entity<UTemplateParameter>().HasNoKey();
            modelBuilder.Entity<UMediaUpload>().HasNoKey();
        }
    }
}
