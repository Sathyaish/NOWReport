using System.Configuration;
using Wintellect.Extensions;

namespace Wintellect.NOW.Configuration
{
    public class WintellectNOWSettings : ConfigurationSection
    {
        [ConfigurationProperty("Reporting", IsRequired = false)]
        public ReportingSettings Reporting
        {
            get
            {
                return (ReportingSettings)this["Reporting"];
            }
            set
            {
                typeof(ReportingSettings).IsAssignableFrom(value.GetType()).ThrowIfFalse();

                this["Reporting"] = value;
            }
        }
    }
}
