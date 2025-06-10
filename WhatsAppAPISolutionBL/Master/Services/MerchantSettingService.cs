using Microsoft.Extensions.Options;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class MerchantSettingService : IMerchantSettingService
    {
        #region Fields
        private readonly MerchantSettingsConfigurationSettings _settings;
        #endregion

        #region Ctor
        public MerchantSettingService(IOptions<MerchantSettingsConfigurationSettings> options)
        {
            _settings = options.Value;
        }
        #endregion

        #region Method
        public async Task<MerchantSettingInfo> GetMerchantSetting(string domain)
        {
            var info = new MerchantSettingInfo();
            info.Domain = domain;

            if (_settings.Logos != null && _settings.Logos.ContainsKey(domain))
            {
                info.Logo = _settings.Logos[domain];
            }

            if (_settings.Names != null && _settings.Names.ContainsKey(domain))
            {
                info.Name = _settings.Names[domain];
            }

            if (_settings.Favicons != null && _settings.Favicons.ContainsKey(domain))
            {
                info.Favicon = _settings.Favicons[domain];
            }

            if (info.Logo == null && info.Name == null && info.Favicon == null)
            {
                return null;
            }

            return info;
        }
        #endregion

    }
}
