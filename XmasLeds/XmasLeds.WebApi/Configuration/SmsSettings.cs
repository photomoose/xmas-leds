using System.Configuration;

namespace XmasLeds.WebApi.Configuration
{
    public class SmsSettings : ISmsSettings
    {
        public bool AllowResponses
        {
            get
            {
                var setting = ConfigurationManager.AppSettings["AllowSmsResponses"];
                bool value;

                return bool.TryParse(setting, out value) && value;
            }
        }
    }
}