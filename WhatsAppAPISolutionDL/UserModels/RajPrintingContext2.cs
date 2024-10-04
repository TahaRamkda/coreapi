using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class WhatsAppAPISolutionContext2 : DbContext
    {
        public WhatsAppAPISolutionContext2()
        {
        }

        public WhatsAppAPISolutionContext2(DbContextOptions<WhatsAppAPISolutionContext2> options)
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
        }
    }
}
