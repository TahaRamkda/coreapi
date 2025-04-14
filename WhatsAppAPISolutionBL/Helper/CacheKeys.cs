using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionBL.Helper
{
    public static class CacheKeys
    {
        #region agents
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        public static string AGENTS_PATTERN_KEY => "Api.Agent.";

        /// <summary>
        /// {0} - ClientId
        /// </summary>
        public static string AGENTS_BY_ID_KEY => "Api.Agent.{0}-{1}";


        /// <summary>
        /// {0} - ClientId
        /// </summary>
        public static string AGENTS_DROPDOWN_KEY => "Api.Agent.{0}-{1}-{2}";

        #endregion

        #region Campaigns Controller
        /// <summary>
        /// Keys Pattern to clear cache
        public static string CAMPAIGNS_PATTERN_KEY => "Api.Campaign.";

        /// <summary>
        /// client - {0}
        /// campaignId - {1}
        public static string CAMPAIGNS_BY_ID_KEY => "Api.Campaign.{0}-{1}";
        #endregion


        #region CLIENT
        public static string CLIENT_PATTERN_KEY => "Api.CLIENT.";
        public static string CLIENT_BY_ID_KEY => "Api.CLIENT.{0}";
        public static string CLIENT_DROPDOWN_KEY => "Api.CLIENT.{0}-{1}";
        #endregion

        #region Contact
        public static string CONTACT_PATTERN_KEY => "Api.Contact.";
        public static string CONTACT_BY_ID_KEY => "Api.Contact.{0}-{1}";
        #endregion

        #region Conversation
        public static string CONVERSATION_DROPDOWN_KEY => "Api.Conversation.{0}-{1}";
        #endregion

        #region Dashboard
        public static string DASHBOARD_PATTERN_KEY = "Api.Dashboard.";

        #endregion

        #region Group
        public static string GROUP_PATTERN_KEY => "Api.Group.";
        public static string GROUP_DROPDOWN_KEY => "Api.Group.{0}-{1}";

        public static string GROUP_BY_ID_KEY => "Api.Group.{0}-{1}";

        #endregion

        #region InteractiveTemplate
        public static string INTERACTIVE_TEMPLATE_PATTERN_KEY => "Api.InteractiveTemplate.";
        public static string INTERACTIVE_TEMPLATE_BY_ID_KEY => "Api.InteractiveTemplate.{0}-{1}-{2}";
        public static string INTERACTIVE_TEMPLATE_DROPDOWN_KEY => "Api.InteractiveTemplate.{0}-{1}-{2}-{3}";
        #endregion

        #region AgentInteractiveTemplate
        public static string AGENT_INTERACTIVE_DROPDOWN_KEY => "Api.AgentInteractiveTemplate.{0}-{1}-{2}-{3}";

        #endregion

        #region MasterDataController
        public static string MASTERDATA_PATTERN_KEY => "Api.MasterData.";

        public static string MASTERDATA_DROPDOWN_KEY => "Api.MasterData.{0}";
        #endregion

        #region Permission
        public static string PERMISSION_DROPDOWN_KEY => "Api.Permission.{0}-{1}";
        #endregion

        #region RoleController
        public static string ROLE_PATTERN_KEY => "Api.Role.";
        public static string ROLE_BY_ID_KEY => "Api.Role.{0}-{1}";
        public static string ROLE_DROPDOWN_KEY => "Api.Role.{0}";
        public static string ROLE_DROPDOWN_KEY2 => "Api.Role.{0}-{1}";
        #endregion


        #region SenderName

        public static string SENDERNAME_PATTERN_KEY = "Api.SenderName.";
        public static string SENDERNAME_DROPDOWN_KEY = "Api.SenderName.{0}";
        public static string SENDERNAME_DROPDOWN_KEY2 = "Api.SenderName.{0}-{1}";
        public static string SENDERNAME_BY_ID_KEY = "Api.SenderName.{0}-{1}";
        #endregion


        //#region Media
        //public static string MEDIA_PATTERN_KEY => "Api.Media.";
        //public static string MEDIA_BY_ID_KEY => "Api.Media.{0}-{1}";
        //public static string MEDIA_DROPDOWN_KEY => "Api.Media.{0}-{1}-{2}";
        //#endregion

        #region Flow
        public static string FLOW_PATTERN_KEY => "Api.Flow.";
        public static string FLOW_BY_ID_KEY => "Api.Flow.{0}";

        public static string FLOW_DROPDOWN_KEY => "Api.Flow.{0}-{1}-{2}";
        #endregion

        #region SystemActions
        public static string SYSTEMACTIONS_PATTERN_KEY => "Api.SystemActions.";
        public static string SYSTEMACTIONS_DROPDOWN_KEY => "Api.SystemActions.{0}-{1}";
        public static string SYSTEMACTIONS_BY_ID_KEY => "Api.SystemActions.{0}-{1}";
        #endregion

        #region Template
        public static string Template_PATTERN_KEY => "Api.Template.";
        public static string Template_BY_ID_KEY => "Api.Template.{0}-{1}";

        public static string Template_DROPDOWN_KEY => "Api.Template.{0}-{1}";
        public static string Template_DROPDOWN_KEY2 => "Api.Template.{0}-{1}-{2}";
        public static string Template_DROPDOWN_KEY3 => "Api.Template.{0}";
        #endregion

        #region UserService
        public static string USER_PATTERN_KEY => "Api.User.";
        public static string USER_BY_ID_KEY => "Api.User.{0}-{1}";
        public static string USER_DROPDOWN_KEY => "Api.User.{0}-{1}";
        #endregion

        #region AppSettings
        public static string APPSETTINGS_PATTERN_KEY => "Api.AppSettings.";
        public static string APPSETTINGS_BY_ID_KEY => "Api.AppSettings.{0}-{1}";
        public static string APPSETTINGS_DROPDOWN_KEY => "Api.AppSettings.{0}-{1}";
        #endregion

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        //public static string SENDERS_PATTERN_KEY => "Bridge.Sender.";

        /// <summary>
        /// {0} - ClientId 
        /// {1} - SenderId
        /// </summary>
        //public static string SENDERS_BY_CLIENTID_SENDERID_KEY = "Bridge.Sender.{0}-{1}";
    }
}
