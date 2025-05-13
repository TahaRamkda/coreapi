using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionBL.Helper
{
    public static class CacheKeys
    {
        #region Agents

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        public static string AGENTS_PATTERN_KEY => "Api.Agent.";

        /// <summary>
        /// {0} - ClientId
        /// {1} - AgentId
        /// </summary>
        public static string AGENTS_BY_ID_KEY => "Api.Agent.{0}-{1}";

        #endregion

        #region Client

        public static string CLIENT_PATTERN_KEY => "Api.Client.";

        /// <summary>
        /// {0} - ClientId
        /// </summary>
        public static string CLIENT_BY_ID_KEY => "Api.Client.{0}";

        /// <summary>
        /// {0} - ClientId
        /// {1} - SearchStr
        /// </summary>
        public static string CLIENT_DROPDOWN_KEY => "Api.Client.{0}-{1}";

        #endregion

        #region Contact

        public static string CONTACT_PATTERN_KEY => "Api.Contact.";

        /// <summary>
        /// {0} - ClientId 
        /// {1} - ContactId
        /// </summary>
        public static string CONTACT_BY_ID_KEY => "Api.Contact.{0}-{1}";

        #endregion

        #region Group

        public static string GROUP_PATTERN_KEY => "Api.Group.";

        /// <summary>
        /// {0} - ClientId
        /// {1} - SearchStr
        /// </summary>
        public static string GROUP_DROPDOWN_KEY => "Api.Group.{0}-{1}";

        /// <summary>
        /// {0} - ClientId
        /// {1} - GroupId
        /// </summary>
        public static string GROUP_BY_ID_KEY => "Api.Group.{0}-{1}";

        #endregion

        #region InteractiveTemplate

        public static string INTERACTIVE_TEMPLATE_PATTERN_KEY => "Api.InteractiveTemplate.";

        /// <summary>
        /// {0} - ClientId
        /// {1} - SenderId
        /// {2} - InteractiveTemplateId
        /// </summary>
        public static string INTERACTIVE_TEMPLATE_BY_ID_KEY => "Api.InteractiveTemplate.{0}-{1}-{2}";

        /// <summary>
        /// {0} - ClientId
        /// {1} - SenderId
        /// {2} - Language
        /// {3} - SearchStr
        /// </summary>
        public static string AGENT_INTERACTIVE_DROPDOWN_KEY => "Api.InteractiveTemplate.AgentTemplates.{0}-{1}-{2}-{3}";

        /// <summary>
        /// {0} - ClientId
        /// {1} - SenderId
        /// {2} - Language
        /// {3} - SearchStr
        /// </summary>
        public static string INTERACTIVE_WITHOUT_PARAM_DROPDOWN_KEY => "Api.InteractiveTemplate.WithoutParams.{0}-{1}-{2}-{3}";

        #endregion

        #region MasterData 

        public static string MASTERDATA_PATTERN_KEY => "Api.MasterData.";

        /// <summary>
        /// {0} - Key
        /// </summary>
        public static string MASTERDATA_DROPDOWN_KEY => "Api.MasterData.{0}";

        #endregion

        #region Permission

        public static string PERMISSION_PATTERN_KEY => "Api.Permission.";

        /// <summary>
        /// {0} - ClientId
        /// {1} - RoleId
        /// </summary>
        public static string PERMISSION_DROPDOWN_KEY => "Api.Permission.{0}-{1}";

        #endregion

        #region Role 

        public static string ROLE_PATTERN_KEY => "Api.Role.";

        /// <summary>
        /// {0} - ClientId
        /// {1} - RoleId
        /// </summary>
        public static string ROLE_BY_ID_KEY => "Api.Role.{0}-{1}";

        /// <summary>
        /// {0} - ClientId
        /// </summary>
        public static string ROLE_DROPDOWN_KEY => "Api.Role.Dropdown.{0}";

        /// <summary>
        /// {0} - ClientId
        /// {1} - SearchStr
        /// </summary>
        public static string ROLE_DROPDOWN_KEY2 => "Api.Role.Dropdown2.{0}-{1}";

        #endregion

        #region SenderName

        public static string SENDERNAME_PATTERN_KEY = "Api.SenderName.";

        /// <summary>
        /// {0} - ClientId
        /// {1} - SenderId
        /// </summary>
        public static string SENDERNAME_BY_ID_KEY = "Api.SenderName.{0}-{1}";

        /// <summary>
        /// {0} - ClientId
        /// </summary>
        public static string SENDERNAME_DROPDOWN_KEY = "Api.SenderName.Dropdown.{0}";

        /// <summary>
        /// {0} - ClientId
        /// {1} - SearchStr
        /// </summary>
        public static string SENDERNAME_DROPDOWN_KEY2 = "Api.SenderName.Dropdown2.{0}-{1}";
         
        #endregion

        #region SystemActions

        public static string SYSTEMACTIONS_PATTERN_KEY => "Api.SystemActions.";

        /// <summary>
        /// {0} - ClientId
        /// {1} - SearchStr
        /// </summary>
        public static string SYSTEMACTIONS_DROPDOWN_KEY => "Api.SystemActions.Dropdown.{0}-{1}";

        /// <summary>
        /// {0} - ClientId
        /// {1} - Id
        /// </summary>
        public static string SYSTEMACTIONS_BY_ID_KEY => "Api.SystemActions.{0}-{1}";

        #endregion

        #region Template

        public static string TEMPLATE_PATTERN_KEY => "Api.Template.";

        /// <summary>
        /// {0} - ClientId
        /// {1} - TemplateId
        /// </summary>
        public static string TEMPLATE_BY_ID_KEY => "Api.Template.{0}-{1}";

        /// <summary>
        /// {0} - ClientId
        /// {1} - SenderId
        /// {2} - SearchStr
        /// </summary>
        public static string TEMPLATE_DROPDOWN_KEY => "Api.Template.Dropdown.{0}-{1}-{2}";

        /// <summary>
        /// {0} - SearchStr
        /// </summary>
        public static string TEMPLATE_DROPDOWN_KEY2 => "Api.Template.Dropdown2.{0}";

        /// <summary>
        /// {0} - SearchStr
        /// </summary>        
        public static string TEMPLATE_DROPDOWN_KEY3 => "Api.Template.Dropdown3.{0}";

        #endregion

        #region UserService

        public static string USER_PATTERN_KEY => "Api.User.";

        /// <summary>
        /// {0} - ClientId
        /// {1} - UserId
        /// </summary>
        public static string USER_BY_ID_KEY => "Api.User.{0}-{1}";

        /// <summary>
        /// {0} - ClientId
        /// {0} - SearchStr
        /// </summary>
        public static string USER_DROPDOWN_KEY => "Api.User.Dropdown.{0}-{1}";

        #endregion

        #region AppSettings

        public static string APPSETTINGS_PATTERN_KEY => "Api.AppSettings.";

        /// <summary>
        /// {0} - ClientId
        /// {1} - Id
        /// </summary>
        public static string APPSETTINGS_BY_ID_KEY => "Api.AppSettings.{0}-{1}";

        /// <summary>
        /// {0} - ClientId
        /// {1} - SearchStr
        /// </summary>
        public static string APPSETTINGS_DROPDOWN_KEY => "Api.AppSettings.{0}-{1}";

        #endregion
    }
}
