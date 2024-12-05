namespace WhatsAppAPISolutionDL.Models
{
    public enum CrudEnum
    {
        Add = 1,
        AllocateSalary = 1,
        GetClientInvoiceNumber = 1,

        Update = 2,
        RepayLoan = 2,
        AllocateBonus = 2,
        GetPNL = 2,
        ActivateCampaign = 2,

        Delete = 3,
        AdvanceSalary = 3,
        AdditionalLoan = 3,
        UpdateCampaign = 3,

        List = 4,

        JoinedDate = 5,
        AddBalance = 5,
        UsersList = 5,
        FundTransfer = 5,
        PaymentAndAdvanceSalarySettlement = 5,
        GetPurchaseLine = 5,
        GetUsageLine = 5,
        GetQuotationLine = 5,
        ChangePassword = 5,
        SettleDistributorAmount = 5,
        SettleCustomerAmount = 5,
        SendCampaign = 5,
        BulkContact = 5,
        GetTemplateDetails = 5,
        SettleCampaign = 5,


        AddUserToken = 6,
        GetPurchaseHeader = 6,
        GetQuotationHeader = 6,
        UpdateTemplateStatus = 6,
        SetAgentStatus = 6,

        CancelOrder = 7,
        SettleDistributorPurchaseWithAmount = 7,
        UpdateQuotationStatus = 7,
        GetTemplateParameterDetails = 7,

        PurchaseReturn = 8,

        GetAllLoanTransactions = 10,

        GetEntities = 50,

        GetAssetPaymentTypes = 51,

        UpdateBankAccountAmount = 60,
        GetProductModuleType = 60,
        GetQuotationStatus = 60
    }
}
