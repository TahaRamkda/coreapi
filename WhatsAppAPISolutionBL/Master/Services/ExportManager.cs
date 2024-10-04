using OfficeOpenXml;
using OfficeOpenXml.Style;
using WhatsAppAPISolutionBL.Master.Help;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.Extensions;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class ExportManager : IExportManager
    {
        private readonly WhatsAppAPISolutionContext _dbContext;

        public ExportManager(WhatsAppAPISolutionContext dbContext)
        {
            _dbContext = dbContext;
        }

        //public virtual byte[] ExportBankAccountsToXlsx(IEnumerable<UBankAccount> bankAccount)
        //{
        //    //property array
        //    var properties = new[]
        //    {
        //        new PropertyByName<UBankAccount>("AccountName", p => p.Account_Name),
        //        new PropertyByName<UBankAccount>("BankName", p => p.Bank_Name),
        //        new PropertyByName<UBankAccount>("AccountNumber", p => p.Account_Number),
        //        new PropertyByName<UBankAccount>("IBAN", p => p.IBAN),
        //        new PropertyByName<UBankAccount>("BranchCode", p => p.BranchCode),
        //        new PropertyByName<UBankAccount>("OpeningBalance", p => p.Opening_Balance),
        //        new PropertyByName<UBankAccount>("CurrentBalance", p => p.Current_Balance),
        //        new PropertyByName<UBankAccount>("CreatedDate", p => p.Created_Date.Split(new[] { ' ' }, 2)[0]),
        //        new PropertyByName<UBankAccount>("CreatedTime", p => p.Created_Date.Split(new[] { ' ' }, 2)[1])
        //    };

        //    return ExportToXlsx(properties, bankAccount);
        //}

        //public virtual byte[] ExportAssetsToXlsx(IEnumerable<UAssets> assets)
        //{
        //    //property array
        //    var properties = new[]
        //    {
        //        new PropertyByName<UAssets>("AssetsName", p => p.Assets_Name),
        //        new PropertyByName<UAssets>("AssetsType", p => Enum.GetName(typeof(AssetTypeEnum), p.Assets_Type)),
        //        new PropertyByName<UAssets>("AssetCost", p => p.Asset_Cost),
        //        new PropertyByName<UAssets>("PurchasedAt", p => p.Purchased_At.Format()),
        //        new PropertyByName<UAssets>("BankacName", p => p.Bankac_Id>0?_dbContext.BankAccounts.Where(x => x.BankacId == p.Bankac_Id).FirstOrDefault()?.BankName:""),
        //        new PropertyByName<UAssets>("AssetPaymentType", p => Enum.GetName(typeof(AssetPaymentTypeEnum), p.Asset_Payment_Type)),
        //        new PropertyByName<UAssets>("Remarks", p => p.Remarks),
        //        new PropertyByName<UAssets>("ReferenceNo1", p => p.Reference_No1),
        //        new PropertyByName<UAssets>("ReferenceNo2", p => p.Reference_No2),
        //        new PropertyByName<UAssets>("CreatedDate", p => p.Created_Date.Split(new[] { ' ' }, 2)[0]),
        //        new PropertyByName<UAssets>("CreatedTime", p => p.Created_Date.Split(new[] { ' ' }, 2)[1])
        //    };

        //    return ExportToXlsx(properties, assets);
        //}

        public virtual byte[] ExportToXlsx<T>(PropertyByName<T>[] properties, IEnumerable<T> itemsToExport)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    //var worksheet = xlPackage.Workbook.Worksheets.Add(typeof(T).Name);
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Sheet1");
                    var manager = new PropertyManager<T>(properties);
                    manager.WriteCaption(worksheet, SetCaptionStyle);

                    var row = 2;
                    foreach (var items in itemsToExport)
                    {
                        manager.CurrentObject = items;
                        manager.WriteToXlsx(worksheet, row++);
                    }
                    worksheet.Cells.AutoFitColumns();
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
        private void SetCaptionStyle(ExcelStyle style)
        {
            style.Fill.PatternType = ExcelFillStyle.Solid;
            style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(184, 204, 228));
            style.Font.Bold = true;
        }
    }
}
