using System.Configuration;

namespace Wintellect.NOW.Configuration
{
    public class ReportingSettings : ConfigurationElement
    {
        [ConfigurationProperty("logo", IsRequired = false)]
        public string Logo
        {
            get
            {
                return (string)this["logo"];
            }
            set
            {
                this["logo"] = value;
            }
        }

        [ConfigurationProperty("pageNumberContainerImage", IsRequired = false)]
        public string PageNumberContainerImage
        {
            get
            {
                return (string)this["pageNumberContainerImage"];
            }
            set
            {
                this["pageNumberContainerImage"] = value;
            }
        }
    }
}
