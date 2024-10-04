using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class WhatsAppAPISolutionContext : DbContext
    {
        public WhatsAppAPISolutionContext()
        {
        }

        public WhatsAppAPISolutionContext(DbContextOptions<WhatsAppAPISolutionContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApiUser> ApiUsers { get; set; }
        public virtual DbSet<Asset> Assets { get; set; }
        public virtual DbSet<AssetsTransactionDetail> AssetsTransactionDetails { get; set; }
        public virtual DbSet<BankAccount> BankAccounts { get; set; }
        public virtual DbSet<BankAccountsTransaction> BankAccountsTransactions { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<ClientInvoice> ClientInvoices { get; set; }
        public virtual DbSet<CommonAttribute> CommonAttributes { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerTransaction> CustomerTransactions { get; set; }
        public virtual DbSet<Distributor> Distributors { get; set; }
        public virtual DbSet<DistributorProduct> DistributorProducts { get; set; }
        public virtual DbSet<DistributorTransaction> DistributorTransactions { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeTransaction> EmployeeTransactions { get; set; }
        public virtual DbSet<EmployeeTransactionReason> EmployeeTransactionReasons { get; set; }
        public virtual DbSet<ExpenseAccount> ExpenseAccounts { get; set; }
        public virtual DbSet<InvestmentTransaction> InvestmentTransactions { get; set; }
        public virtual DbSet<Loan> Loans { get; set; }
        public virtual DbSet<LoanTransaction> LoanTransactions { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<MasterDatum> MasterData { get; set; }
        public virtual DbSet<MasterProduct> MasterProducts { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Number> Numbers { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<PartialPayment> PartialPayments { get; set; }
        public virtual DbSet<PartialPaymentModule> PartialPaymentModules { get; set; }
        public virtual DbSet<Partner> Partners { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PermissionTask> PermissionTasks { get; set; }
        public virtual DbSet<Pnl> Pnls { get; set; }
        public virtual DbSet<PnlBankAccount> PnlBankAccounts { get; set; }
        public virtual DbSet<PnlPartnerProfit> PnlPartnerProfits { get; set; }
        public virtual DbSet<PnlSubModuleSummary> PnlSubModuleSummaries { get; set; }
        public virtual DbSet<PrintingItemsLine> PrintingItemsLines { get; set; }
        public virtual DbSet<PrintingOrder> PrintingOrders { get; set; }
        public virtual DbSet<PrintingOrdersLine> PrintingOrdersLines { get; set; }
        public virtual DbSet<PrintingPurchase> PrintingPurchases { get; set; }
        public virtual DbSet<PrintingPurchaseItem> PrintingPurchaseItems { get; set; }
        public virtual DbSet<PrintingUsage> PrintingUsages { get; set; }
        public virtual DbSet<PrintingUsageLine> PrintingUsageLines { get; set; }
        public virtual DbSet<ProductStock> ProductStocks { get; set; }
        public virtual DbSet<Quotation> Quotations { get; set; }
        public virtual DbSet<QuotationLine> QuotationLines { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<StickerItemsLine> StickerItemsLines { get; set; }
        public virtual DbSet<StickerOrder> StickerOrders { get; set; }
        public virtual DbSet<StickerOrdersLine> StickerOrdersLines { get; set; }
        public virtual DbSet<StickerPurchase> StickerPurchases { get; set; }
        public virtual DbSet<StickerPurchaseItem> StickerPurchaseItems { get; set; }
        public virtual DbSet<StickerUsage> StickerUsages { get; set; }
        public virtual DbSet<StickerUsageLine> StickerUsageLines { get; set; }
        public virtual DbSet<SubModule> SubModules { get; set; }
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
            modelBuilder.Entity<ApiUser>(entity =>
            {
                entity.Property(e => e.ApiUserId).HasColumnName("Api_User_Id");
            });

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.HasKey(e => e.AssetsId)
                    .HasName("PK__Assets__7AFA02E9D47AEC7C");

                entity.Property(e => e.AssetsId).HasColumnName("Assets_Id");

                entity.Property(e => e.AssetCost)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Asset_Cost");

                entity.Property(e => e.AssetPaymentType).HasColumnName("Asset_Payment_Type");

                entity.Property(e => e.AssetsName)
                    .HasMaxLength(250)
                    .HasColumnName("Assets_Name");

                entity.Property(e => e.AssetsType).HasColumnName("Assets_Type");

                entity.Property(e => e.BankacId).HasColumnName("Bankac_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.PurchasedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("Purchased_At");

                entity.Property(e => e.ReferenceNo1)
                    .HasMaxLength(250)
                    .HasColumnName("Reference_No1");

                entity.Property(e => e.ReferenceNo2)
                    .HasMaxLength(250)
                    .HasColumnName("Reference_No2");

                entity.Property(e => e.Remarks).HasMaxLength(250);

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<AssetsTransactionDetail>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__AssetsTr__9A8D5605223B7CC5");

                entity.ToTable("AssetsTransaction_Details");

                entity.Property(e => e.TransactionId).HasColumnName("Transaction_Id");

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.AssetsId).HasColumnName("Assets_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.PartnerId).HasColumnName("Partner_Id");

                entity.Property(e => e.Percentage).HasColumnType("numeric(18, 3)");
            });

            modelBuilder.Entity<BankAccount>(entity =>
            {
                entity.HasKey(e => e.BankacId)
                    .HasName("PK__Bank_Acc__0C2A0F66DDC65DB5");

                entity.ToTable("Bank_Accounts");

                entity.Property(e => e.BankacId).HasColumnName("Bankac_Id");

                entity.Property(e => e.AccountName)
                    .HasMaxLength(250)
                    .HasColumnName("Account_Name");

                entity.Property(e => e.AccountNumber)
                    .HasMaxLength(50)
                    .HasColumnName("Account_Number");

                entity.Property(e => e.BankName)
                    .HasMaxLength(250)
                    .HasColumnName("Bank_Name");

                entity.Property(e => e.BranchCode).HasMaxLength(50);

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.CurrentBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Current_Balance");

                entity.Property(e => e.Iban)
                    .HasMaxLength(100)
                    .HasColumnName("IBAN");

                entity.Property(e => e.OpeningBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Opening_Balance");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<BankAccountsTransaction>(entity =>
            {
                entity.HasKey(e => e.BankacTranId);

                entity.ToTable("Bank_Accounts_Transaction");

                entity.Property(e => e.BankacTranId).HasColumnName("Bankac_TranId");

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.BankacId).HasColumnName("Bankac_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.CurrentBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Current_Balance");

                entity.Property(e => e.ModuleId).HasColumnName("Module_Id");

                entity.Property(e => e.ModuleRefId).HasColumnName("Module_RefId");

                entity.Property(e => e.PreviousBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Previous_Balance");

                entity.Property(e => e.ReferenceNo).HasMaxLength(50);

                entity.Property(e => e.ReferenceNo2).HasMaxLength(50);

                entity.Property(e => e.Remarks).HasMaxLength(250);

                entity.Property(e => e.SubModuleId).HasColumnName("SubModule_Id");

                entity.Property(e => e.TransactionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Transaction_Date");

                entity.Property(e => e.TransactionType).HasColumnName("Transaction_Type");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.ClientLanguage).HasColumnName("Client_Language");

                entity.Property(e => e.ClientName)
                    .HasMaxLength(50)
                    .HasColumnName("Client_Name");

                entity.Property(e => e.ClientPrefix)
                    .HasMaxLength(3)
                    .HasColumnName("Client_Prefix");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");
            });

            modelBuilder.Entity<ClientInvoice>(entity =>
            {
                entity.ToTable("Client_Invoice");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.InvoicePrefix)
                    .HasMaxLength(100)
                    .HasColumnName("Invoice_Prefix");

                entity.Property(e => e.InvoiceTypeId).HasColumnName("InvoiceType_Id");

                entity.Property(e => e.LastInvoiceNumber).HasColumnName("Last_Invoice_Number");

                entity.Property(e => e.PreviousUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Previous_Updated_Date");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<CommonAttribute>(entity =>
            {
                entity.HasKey(e => e.AttributeId)
                    .HasName("PK__Common_A__6DC45AB7A916AAE5");

                entity.ToTable("Common_Attributes");

                entity.Property(e => e.AttributeId).HasColumnName("Attribute_Id");

                entity.Property(e => e.AttributeName)
                    .HasMaxLength(250)
                    .HasColumnName("Attribute_Name");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.TagId).HasColumnName("Tag_Id");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerId).HasColumnName("Customer_Id");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(250)
                    .HasColumnName("Customer_Name");

                entity.Property(e => e.Phone).HasMaxLength(250);

                entity.Property(e => e.Phone2).HasMaxLength(250);

                entity.Property(e => e.PrintingBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Printing_Balance");

                entity.Property(e => e.StickerBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Sticker_Balance");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<CustomerTransaction>(entity =>
            {
                entity.HasKey(e => e.CustTranId);

                entity.ToTable("Customer_Transactions");

                entity.Property(e => e.CustTranId).HasColumnName("Cust_TranId");

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.CurrentPrintingBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Current_Printing_Balance");

                entity.Property(e => e.CurrentStickerBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Current_Sticker_Balance");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_Id");

                entity.Property(e => e.ModuleTypeId).HasColumnName("ModuleType_Id");

                entity.Property(e => e.OrderId).HasColumnName("Order_Id");

                entity.Property(e => e.PreviousPrintingBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Previous_Printing_Balance");

                entity.Property(e => e.PreviousStickerBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Previous_Sticker_Balance");

                entity.Property(e => e.ReferenceNo).HasMaxLength(50);

                entity.Property(e => e.ReferenceNo2).HasMaxLength(50);

                entity.Property(e => e.Remarks).HasMaxLength(250);

                entity.Property(e => e.TransactionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Transaction_Date");

                entity.Property(e => e.TransactionType).HasColumnName("Transaction_Type");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<Distributor>(entity =>
            {
                entity.Property(e => e.DistributorId).HasColumnName("Distributor_Id");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.DistributorName)
                    .HasMaxLength(250)
                    .HasColumnName("Distributor_Name");

                entity.Property(e => e.Phone).HasMaxLength(250);

                entity.Property(e => e.Phone2).HasMaxLength(250);

                entity.Property(e => e.PrintingBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Printing_Balance");

                entity.Property(e => e.StickerBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Sticker_Balance");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<DistributorProduct>(entity =>
            {
                entity.ToTable("Distributor_Products");

                entity.Property(e => e.DistributorProductId).HasColumnName("Distributor_Product_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.DistributorId).HasColumnName("Distributor_Id");

                entity.Property(e => e.DistributorProductName)
                    .HasMaxLength(250)
                    .HasColumnName("Distributor_Product_Name");

                entity.Property(e => e.MasterProductId).HasColumnName("Master_Product_Id");

                entity.Property(e => e.ProductCode)
                    .HasMaxLength(250)
                    .HasColumnName("Product_Code");

                entity.Property(e => e.ProductColor)
                    .HasMaxLength(250)
                    .HasColumnName("Product_Color");

                entity.Property(e => e.ProductModuleTypeId).HasColumnName("Product_Module_TypeId");

                entity.Property(e => e.QuantityPerUnit)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Quantity_Per_Unit");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<DistributorTransaction>(entity =>
            {
                entity.HasKey(e => e.DistTranId)
                    .HasName("PK_Distributors_Transaction");

                entity.ToTable("Distributor_Transactions");

                entity.Property(e => e.DistTranId).HasColumnName("Dist_TranId");

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.CurrentPrintingBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Current_Printing_Balance");

                entity.Property(e => e.CurrentStickerBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Current_Sticker_Balance");

                entity.Property(e => e.DistributorId).HasColumnName("Distributor_Id");

                entity.Property(e => e.ModuleTypeId).HasColumnName("ModuleType_Id");

                entity.Property(e => e.PreviousPrintingBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Previous_Printing_Balance");

                entity.Property(e => e.PreviousStickerBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Previous_Sticker_Balance");

                entity.Property(e => e.PurchaseId).HasColumnName("Purchase_Id");

                entity.Property(e => e.ReferenceNo).HasMaxLength(50);

                entity.Property(e => e.ReferenceNo2).HasMaxLength(50);

                entity.Property(e => e.Remarks).HasMaxLength(250);

                entity.Property(e => e.TransactionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Transaction_Date");

                entity.Property(e => e.TransactionType).HasColumnName("Transaction_Type");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.EmployeeId).HasColumnName("Employee_Id");

                entity.Property(e => e.AdvanceAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Advance_Amount");

                entity.Property(e => e.BalanceAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Balance_Amount");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.EmployeeName)
                    .HasMaxLength(250)
                    .HasColumnName("Employee_Name");

                entity.Property(e => e.JoinedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Joined_Date");

                entity.Property(e => e.Salary).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.SeparationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Separation_Date");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<EmployeeTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__Employee__9A8D5605C45509C1");

                entity.ToTable("Employee_Transaction");

                entity.Property(e => e.TransactionId).HasColumnName("Transaction_Id");

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.BankacId).HasColumnName("Bankac_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.CurrentAdvanceAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Current_Advance_Amount");

                entity.Property(e => e.CurrentBalanceAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Current_Balance_Amount");

                entity.Property(e => e.EmployeeId).HasColumnName("Employee_Id");

                entity.Property(e => e.NextPaydate)
                    .HasColumnType("datetime")
                    .HasColumnName("Next_Paydate");

                entity.Property(e => e.PreviousAdvanceAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Previous_Advance_Amount");

                entity.Property(e => e.PreviousBalanceAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Previous_Balance_Amount");

                entity.Property(e => e.ReasonType).HasColumnName("Reason_Type");

                entity.Property(e => e.ReferenceNo).HasMaxLength(250);

                entity.Property(e => e.ReferenceNo2).HasMaxLength(250);

                entity.Property(e => e.Remarks).HasMaxLength(500);

                entity.Property(e => e.TransactionType).HasColumnName("Transaction_Type");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<EmployeeTransactionReason>(entity =>
            {
                entity.ToTable("Employee_Transaction_Reason");

                entity.Property(e => e.ReasonId).HasColumnName("Reason_Id");

                entity.Property(e => e.ReasonName)
                    .HasMaxLength(100)
                    .HasColumnName("Reason_Name");
            });

            modelBuilder.Entity<ExpenseAccount>(entity =>
            {
                entity.HasKey(e => e.ExpenseId)
                    .HasName("PK__Expense___D56F7ABD076AB527");

                entity.ToTable("Expense_Account");

                entity.Property(e => e.ExpenseId).HasColumnName("Expense_Id");

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.BankacId).HasColumnName("Bankac_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.ExpenseDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Expense_Date");

                entity.Property(e => e.ExpenseName)
                    .HasMaxLength(250)
                    .HasColumnName("Expense_Name");

                entity.Property(e => e.ReferenceNo).HasMaxLength(250);

                entity.Property(e => e.Remarks).HasMaxLength(250);

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<InvestmentTransaction>(entity =>
            {
                entity.HasKey(e => e.ItransactionId)
                    .HasName("PK__Investme__8668FA0A9B112F00");

                entity.ToTable("Investment_Transaction");

                entity.Property(e => e.ItransactionId).HasColumnName("ITransaction_Id");

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.BankacId).HasColumnName("Bankac_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.CurrentInvestment)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Current_Investment");

                entity.Property(e => e.InvestmentDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Investment_Date");

                entity.Property(e => e.PartnerId).HasColumnName("Partner_Id");

                entity.Property(e => e.PreviousInvestment)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Previous_Investment");

                entity.Property(e => e.ReferenceNo1)
                    .HasMaxLength(250)
                    .HasColumnName("Reference_No1");

                entity.Property(e => e.ReferenceNo2)
                    .HasMaxLength(250)
                    .HasColumnName("Reference_No2");

                entity.Property(e => e.Remarks).HasMaxLength(250);

                entity.Property(e => e.TransactionType).HasColumnName("Transaction_Type");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.Property(e => e.LoanId).HasColumnName("Loan_Id");

                entity.Property(e => e.BankacId).HasColumnName("Bankac_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.LoanAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Loan_Amount");

                entity.Property(e => e.LoanDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Loan_Date");

                entity.Property(e => e.LoanPeriod)
                    .HasMaxLength(250)
                    .HasColumnName("Loan_Period");

                entity.Property(e => e.LoanRepaid).HasColumnName("Loan_Repaid");

                entity.Property(e => e.LoanTypeId).HasColumnName("LoanType_Id");

                entity.Property(e => e.LoaneeName)
                    .HasMaxLength(250)
                    .HasColumnName("Loanee_Name");

                entity.Property(e => e.LoanerName)
                    .HasMaxLength(250)
                    .HasColumnName("Loaner_Name");

                entity.Property(e => e.PaidAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Paid_Amount");

                entity.Property(e => e.ReferenceNo1)
                    .HasMaxLength(250)
                    .HasColumnName("Reference_No1");

                entity.Property(e => e.ReferenceNo2)
                    .HasMaxLength(250)
                    .HasColumnName("Reference_No2");

                entity.Property(e => e.RemainingAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Remaining_Amount");

                entity.Property(e => e.Remarks).HasMaxLength(250);

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<LoanTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__Loan_Tra__9A8D5605842F4DE8");

                entity.ToTable("Loan_Transaction");

                entity.Property(e => e.TransactionId).HasColumnName("Transaction_Id");

                entity.Property(e => e.BankacId).HasColumnName("Bankac_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.LoanId).HasColumnName("Loan_Id");

                entity.Property(e => e.PaidAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Paid_Amount");

                entity.Property(e => e.PaidDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Paid_Date");

                entity.Property(e => e.PreviousAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Previous_Amount");

                entity.Property(e => e.ReferanceNo1)
                    .HasMaxLength(250)
                    .HasColumnName("Referance_No1");

                entity.Property(e => e.ReferanceNo2)
                    .HasMaxLength(250)
                    .HasColumnName("Referance_No2");

                entity.Property(e => e.RemainingAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Remaining_Amount");

                entity.Property(e => e.Remarks).HasMaxLength(250);

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

            modelBuilder.Entity<MasterDatum>(entity =>
            {
                entity.HasKey(e => e.MastId)
                    .HasName("PK__MasterDa__C58CEA2C58E9D693");

                entity.HasIndex(e => new { e.Id, e.Type }, "UQ__MasterDa__8D8F664E1D916338")
                    .IsUnique();

                entity.Property(e => e.MastId).HasColumnName("Mast_Id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<MasterProduct>(entity =>
            {
                entity.ToTable("Master_Products");

                entity.Property(e => e.MasterProductId).HasColumnName("Master_Product_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.MasterProductName)
                    .HasMaxLength(250)
                    .HasColumnName("Master_Product_Name");

                entity.Property(e => e.ProductModuleTypeId).HasColumnName("Product_Module_TypeId");

                entity.Property(e => e.QuantityPerUnit)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Quantity_Per_Unit");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
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

            modelBuilder.Entity<PartialPayment>(entity =>
            {
                entity.ToTable("Partial_Payment");

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.BankacId).HasColumnName("Bankac_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.PartialPaymentModuleId).HasColumnName("Partial_Payment_Module_Id");

                entity.Property(e => e.Reference1).HasMaxLength(250);

                entity.Property(e => e.Reference2).HasMaxLength(250);

                entity.Property(e => e.ReferenceId).HasColumnName("Reference_Id");

                entity.Property(e => e.Remarks).HasMaxLength(250);

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<PartialPaymentModule>(entity =>
            {
                entity.ToTable("Partial_Payment_Module");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.ModuleCode)
                    .HasMaxLength(100)
                    .HasColumnName("Module_Code");

                entity.Property(e => e.ModuleId).HasColumnName("Module_Id");

                entity.Property(e => e.ModuleName)
                    .HasMaxLength(100)
                    .HasColumnName("Module_Name");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<Partner>(entity =>
            {
                entity.Property(e => e.PartnerId).HasColumnName("Partner_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.CurrentInvestment)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Current_Investment")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.JoinedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Joined_Date");

                entity.Property(e => e.PartnerName)
                    .HasMaxLength(250)
                    .HasColumnName("Partner_Name");

                entity.Property(e => e.SplitPercent)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Split_Percent");

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

            modelBuilder.Entity<Pnl>(entity =>
            {
                entity.ToTable("PNL");

                entity.Property(e => e.PnlId).HasColumnName("PNL_Id");

                entity.Property(e => e.ApprovedBy).HasColumnName("Approved_By");

                entity.Property(e => e.ApprovedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Approved_Date");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.Profit).HasColumnType("numeric(18, 3)");
            });

            modelBuilder.Entity<PnlBankAccount>(entity =>
            {
                entity.ToTable("PNL_Bank_Accounts");

                entity.Property(e => e.BankacId).HasColumnName("Bankac_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.ClosingBalance)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Closing_Balance");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.PnlId).HasColumnName("PNL_Id");
            });

            modelBuilder.Entity<PnlPartnerProfit>(entity =>
            {
                entity.ToTable("PNL_Partner_Profit");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.PartnerId).HasColumnName("Partner_Id");

                entity.Property(e => e.PnlId).HasColumnName("PNL_Id");

                entity.Property(e => e.Profit).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.SplitPercent)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Split_Percent");
            });

            modelBuilder.Entity<PnlSubModuleSummary>(entity =>
            {
                entity.ToTable("PNL_SubModule_Summary");

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.ImpactOnPnl).HasColumnName("ImpactOnPNL");

                entity.Property(e => e.ModuleId).HasColumnName("Module_Id");

                entity.Property(e => e.PnlId).HasColumnName("PNL_Id");

                entity.Property(e => e.SubModuleId).HasColumnName("SubModule_Id");

                entity.Property(e => e.TransactionType).HasColumnName("Transaction_Type");
            });

            modelBuilder.Entity<PrintingItemsLine>(entity =>
            {
                entity.HasKey(e => e.LineId);

                entity.ToTable("Printing_Items_Lines");

                entity.Property(e => e.LineId).HasColumnName("Line_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.DistributorProductId).HasColumnName("Distributor_Product_Id");

                entity.Property(e => e.IsReturned).HasColumnName("Is_Returned");

                entity.Property(e => e.MasterProductId).HasColumnName("Master_Product_Id");

                entity.Property(e => e.PurchaseId).HasColumnName("Purchase_Id");

                entity.Property(e => e.PurchaseItemId).HasColumnName("Purchase_Item_Id");

                entity.Property(e => e.SeqNo).HasColumnName("Seq_No");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");

                entity.Property(e => e.VoucherNo).HasMaxLength(100);
            });

            modelBuilder.Entity<PrintingOrder>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PK__Printing__F1E4607B745A8946");

                entity.ToTable("Printing_Orders");

                entity.Property(e => e.OrderId).HasColumnName("Order_Id");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.CancellationDate).HasColumnType("datetime");

                entity.Property(e => e.CancellationRemarks).HasMaxLength(500);

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.CreditAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Credit_Amount");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_Id");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(250)
                    .HasColumnName("Customer_Name");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.Discount).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.InvoiceNo)
                    .HasMaxLength(300)
                    .HasColumnName("Invoice_No");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Order_Date");

                entity.Property(e => e.OtherCharges)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Other_Charges");

                entity.Property(e => e.PaidAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Paid_Amount");

                entity.Property(e => e.PaidFromBankacId).HasColumnName("Paid_From_BankacId");

                entity.Property(e => e.Phone).HasMaxLength(250);

                entity.Property(e => e.Phone2).HasMaxLength(250);

                entity.Property(e => e.SubTotal)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Sub_Total");

                entity.Property(e => e.Total).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<PrintingOrdersLine>(entity =>
            {
                entity.HasKey(e => e.OrderLine)
                    .HasName("PK__Printing__D2211B748F6BCE07");

                entity.ToTable("Printing_Orders_Line");

                entity.Property(e => e.OrderLine).HasColumnName("Order_Line");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.OrderId).HasColumnName("Order_Id");

                entity.Property(e => e.Rate).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.Total).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<PrintingPurchase>(entity =>
            {
                entity.HasKey(e => e.PurchaseId);

                entity.ToTable("Printing_Purchase");

                entity.Property(e => e.PurchaseId).HasColumnName("Purchase_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.CreditAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Credit_Amount");

                entity.Property(e => e.Discount).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.DistributorId).HasColumnName("Distributor_Id");

                entity.Property(e => e.InvoiceNo)
                    .HasMaxLength(250)
                    .HasColumnName("Invoice_No");

                entity.Property(e => e.IsPartialReceived).HasColumnName("Is_Partial_Received");

                entity.Property(e => e.OtherCharges)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Other_Charges");

                entity.Property(e => e.PaidAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Paid_Amount");

                entity.Property(e => e.PurchaseDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Purchase_Date");

                entity.Property(e => e.SettlementAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Settlement_Amount");

                entity.Property(e => e.SubTotal)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Sub_Total");

                entity.Property(e => e.Total).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<PrintingPurchaseItem>(entity =>
            {
                entity.HasKey(e => e.PurchaseItemId);

                entity.ToTable("Printing_Purchase_Items");

                entity.Property(e => e.PurchaseItemId).HasColumnName("Purchase_Item_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.DistributorId).HasColumnName("Distributor_Id");

                entity.Property(e => e.DistributorProductId).HasColumnName("Distributor_Product_Id");

                entity.Property(e => e.IsPartialReceived).HasColumnName("Is_Partial_Received");

                entity.Property(e => e.IsReturned).HasColumnName("Is_Returned");

                entity.Property(e => e.MasterProductId).HasColumnName("Master_Product_Id");

                entity.Property(e => e.PendingQuantity).HasColumnName("Pending_Quantity");

                entity.Property(e => e.PurchaseId).HasColumnName("Purchase_Id");

                entity.Property(e => e.QuantityPerUnit)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Quantity_Per_Unit");

                entity.Property(e => e.Rate).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.ReceivedQuantity).HasColumnName("Received_Quantity");

                entity.Property(e => e.Total).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.TotalQuantity).HasColumnName("Total_Quantity");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<PrintingUsage>(entity =>
            {
                entity.HasKey(e => e.UsageId);

                entity.ToTable("Printing_Usage");

                entity.Property(e => e.UsageId).HasColumnName("Usage_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.InvoiceNo)
                    .HasMaxLength(250)
                    .HasColumnName("Invoice_No");

                entity.Property(e => e.IsWaste).HasColumnName("Is_Waste");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");

                entity.Property(e => e.UsageDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Usage_Date");
            });

            modelBuilder.Entity<PrintingUsageLine>(entity =>
            {
                entity.HasKey(e => e.UsageLineId);

                entity.ToTable("Printing_Usage_Lines");

                entity.Property(e => e.UsageLineId).HasColumnName("Usage_Line_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.LineId).HasColumnName("Line_Id");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");

                entity.Property(e => e.UsageId).HasColumnName("Usage_Id");
            });

            modelBuilder.Entity<ProductStock>(entity =>
            {
                entity.HasKey(e => e.StockId)
                    .HasName("PK__Product___EFA64E98971638A3");

                entity.ToTable("Product_Stock");

                entity.Property(e => e.StockId).HasColumnName("Stock_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Create_Date");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CurrentStock)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("Current_Stock");

                entity.Property(e => e.EntryId).HasColumnName("Entry_Id");

                entity.Property(e => e.EntryType).HasColumnName("Entry_Type");

                entity.Property(e => e.ItemId).HasColumnName("Item_Id");

                entity.Property(e => e.Quantity).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<Quotation>(entity =>
            {
                entity.Property(e => e.QuotationId).HasColumnName("Quotation_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(200)
                    .HasColumnName("Customer_Name");

                entity.Property(e => e.CustomerPhone)
                    .HasMaxLength(200)
                    .HasColumnName("Customer_Phone");

                entity.Property(e => e.Discount).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Expiry_Date");

                entity.Property(e => e.InvoiceNo)
                    .HasMaxLength(150)
                    .HasColumnName("Invoice_No");

                entity.Property(e => e.ModuleId).HasColumnName("Module_Id");

                entity.Property(e => e.OtherCharges)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Other_Charges");

                entity.Property(e => e.QuotationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Quotation_Date");

                entity.Property(e => e.SubTotal)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Sub_Total");

                entity.Property(e => e.Total).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<QuotationLine>(entity =>
            {
                entity.ToTable("Quotation_Lines");

                entity.Property(e => e.QuotationLineId).HasColumnName("Quotation_Line_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.QuotationId).HasColumnName("Quotation_Id");

                entity.Property(e => e.Rate).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.Total).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
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

            modelBuilder.Entity<StickerItemsLine>(entity =>
            {
                entity.HasKey(e => e.LineId);

                entity.ToTable("Sticker_Items_Lines");

                entity.Property(e => e.LineId).HasColumnName("Line_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CostPerMeter).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.DistributorProductId).HasColumnName("Distributor_Product_Id");

                entity.Property(e => e.IsReturned).HasColumnName("Is_Returned");

                entity.Property(e => e.MasterProductId).HasColumnName("Master_Product_Id");

                entity.Property(e => e.PurchaseId).HasColumnName("Purchase_Id");

                entity.Property(e => e.PurchaseItemId).HasColumnName("Purchase_Item_Id");

                entity.Property(e => e.SeqNo).HasColumnName("Seq_No");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");

                entity.Property(e => e.VoucherNo).HasMaxLength(100);
            });

            modelBuilder.Entity<StickerOrder>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PK__Sticker__F1E4607B745A8946");

                entity.ToTable("Sticker_Orders");

                entity.Property(e => e.OrderId).HasColumnName("Order_Id");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.CancellationDate).HasColumnType("datetime");

                entity.Property(e => e.CancellationRemarks).HasMaxLength(500);

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.CreditAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Credit_Amount");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_Id");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(250)
                    .HasColumnName("Customer_Name");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.Discount).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.InvoiceNo)
                    .HasMaxLength(300)
                    .HasColumnName("Invoice_No");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Order_Date");

                entity.Property(e => e.OtherCharges)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Other_Charges");

                entity.Property(e => e.PaidAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Paid_Amount");

                entity.Property(e => e.PaidFromBankacId).HasColumnName("Paid_From_BankacId");

                entity.Property(e => e.Phone).HasMaxLength(250);

                entity.Property(e => e.Phone2).HasMaxLength(250);

                entity.Property(e => e.SubTotal)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Sub_Total");

                entity.Property(e => e.Total).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<StickerOrdersLine>(entity =>
            {
                entity.HasKey(e => e.OrderLine)
                    .HasName("PK__Sticker___D2211B74988F9135");

                entity.ToTable("Sticker_Orders_Line");

                entity.Property(e => e.OrderLine).HasColumnName("Order_Line");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CostPerMeter)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Cost_Per_Meter");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.LineId).HasColumnName("Line_Id");

                entity.Property(e => e.OrderId).HasColumnName("Order_Id");

                entity.Property(e => e.ProfitPerMeter)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Profit_Per_Meter");

                entity.Property(e => e.Rate).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.Total).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<StickerPurchase>(entity =>
            {
                entity.HasKey(e => e.PurchaseId);

                entity.ToTable("Sticker_Purchase");

                entity.Property(e => e.PurchaseId).HasColumnName("Purchase_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.CreditAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Credit_Amount");

                entity.Property(e => e.Discount).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.DistributorId).HasColumnName("Distributor_Id");

                entity.Property(e => e.InvoiceNo)
                    .HasMaxLength(250)
                    .HasColumnName("Invoice_No");

                entity.Property(e => e.IsPartialReceived).HasColumnName("Is_Partial_Received");

                entity.Property(e => e.OtherCharges)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Other_Charges");

                entity.Property(e => e.PaidAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Paid_Amount");

                entity.Property(e => e.PurchaseDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Purchase_Date");

                entity.Property(e => e.SettlementAmount)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Settlement_Amount");

                entity.Property(e => e.SubTotal)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Sub_Total");

                entity.Property(e => e.Total).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<StickerPurchaseItem>(entity =>
            {
                entity.HasKey(e => e.PurchaseItemId);

                entity.ToTable("Sticker_Purchase_Items");

                entity.Property(e => e.PurchaseItemId).HasColumnName("Purchase_Item_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.DistributorId).HasColumnName("Distributor_Id");

                entity.Property(e => e.DistributorProductId).HasColumnName("Distributor_Product_Id");

                entity.Property(e => e.IsPartialReceived).HasColumnName("Is_Partial_Received");

                entity.Property(e => e.IsReturned).HasColumnName("Is_Returned");

                entity.Property(e => e.MasterProductId).HasColumnName("Master_Product_Id");

                entity.Property(e => e.PendingQuantity).HasColumnName("Pending_Quantity");

                entity.Property(e => e.PurchaseId).HasColumnName("Purchase_Id");

                entity.Property(e => e.QuantityPerUnit)
                    .HasColumnType("numeric(18, 3)")
                    .HasColumnName("Quantity_Per_Unit");

                entity.Property(e => e.Rate).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.ReceivedQuantity).HasColumnName("Received_Quantity");

                entity.Property(e => e.Total).HasColumnType("numeric(18, 3)");

                entity.Property(e => e.TotalQuantity).HasColumnName("Total_Quantity");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");
            });

            modelBuilder.Entity<StickerUsage>(entity =>
            {
                entity.HasKey(e => e.UsageId);

                entity.ToTable("Sticker_Usage");

                entity.Property(e => e.UsageId).HasColumnName("Usage_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.InvoiceNo)
                    .HasMaxLength(250)
                    .HasColumnName("Invoice_No");

                entity.Property(e => e.IsWaste).HasColumnName("Is_Waste");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");

                entity.Property(e => e.UsageDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Usage_Date");
            });

            modelBuilder.Entity<StickerUsageLine>(entity =>
            {
                entity.HasKey(e => e.UsageLineId);

                entity.ToTable("Sticker_Usage_Lines");

                entity.Property(e => e.UsageLineId).HasColumnName("Usage_Line_Id");

                entity.Property(e => e.ClientId).HasColumnName("Client_Id");

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.LineId).HasColumnName("Line_Id");

                entity.Property(e => e.UpdatedBy).HasColumnName("Updated_By");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Updated_Date");

                entity.Property(e => e.UsageId).HasColumnName("Usage_Id");
            });

            modelBuilder.Entity<SubModule>(entity =>
            {
                entity.ToTable("SubModule");

                entity.HasIndex(e => e.SubModuleCode, "SubModule_SubModuleCode")
                    .IsUnique();

                entity.HasIndex(e => new { e.ModuleId, e.SubModuleId }, "SubModule_Unique")
                    .IsUnique();

                entity.HasIndex(e => e.SubModuleId, "UC_SubModule_SubModule_Id")
                    .IsUnique();

                entity.Property(e => e.CreatedBy).HasColumnName("Created_By");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Created_Date");

                entity.Property(e => e.ImpactOnPnl).HasColumnName("ImpactOnPNL");

                entity.Property(e => e.ModuleId).HasColumnName("Module_Id");

                entity.Property(e => e.SubModuleCode)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("SubModule_Code");

                entity.Property(e => e.SubModuleId).HasColumnName("SubModule_Id");

                entity.Property(e => e.SubModuleName)
                    .HasMaxLength(250)
                    .HasColumnName("SubModule_Name");

                entity.Property(e => e.TransactionType).HasColumnName("Transaction_Type");

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
                    .HasName("PK__UsersRol__134E488CD3412323");

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
