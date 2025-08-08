namespace WhatsAppAPISolutionDL.Enum
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
        AgentConversationList = 5,

        AddUserToken = 6,
        GetPurchaseHeader = 6,
        GetQuotationHeader = 6,
        UpdateTemplateStatus = 6,
        SetAgentStatus = 6,
        ConversationListByConversation = 6,
        CampaignContactStats = 6,
        GetAgentInteractiveTemplates = 6,
        GetTemplateAnalyticsDetails=6,

        CancelOrder = 7,
        SettleDistributorPurchaseWithAmount = 7,
        UpdateQuotationStatus = 7,
        GetTemplateParameterDetails = 7,
        AddConversationToQueue = 7,
        SetAgentEnableDisable = 7,
        GetAgentInteractiveTemplatesWithoutParam = 7,

        PurchaseReturn = 8,
        TransferConversationToAgent = 8,
        GetById = 8,

        AssignConversationToAgent = 9,
        GetAppSettingByKeyName = 9,
        WalleteCheckBalance = 9,
        ResetPassword = 9,

        GetAllLoanTransactions = 10,
        GetLatestConversationByConversationId = 10,

        GetConversationByMessageId = 15,
        CloseChatBySupervisor = 16,

        GetConversationLogs = 17,

        GetAgentSupervisorReport = 40,

        GetDetails = 41,
        GetAgentDetailSupervisorReport = 41,


        GetEntities = 50,

        GetAssetPaymentTypes = 51,
        GetTemplateCategories = 51,
        ConversationReportList = 51,
        GetActiveAgents = 51,
        GetAppSettings = 52,



        GetLanguages = 52,
        ConversationDetailReportList = 52,

        GetDefaultTemplates = 53,
        GetConversationStatistics = 53,

        UpdateBankAccountAmount = 60,
        GetProductModuleType = 60,
        GetQuotationStatus = 60,

        DeleteFreqContactedContacts = 61,
        UpdateMedia = 6,
        CheckActiveConversation = 19
    }
}
