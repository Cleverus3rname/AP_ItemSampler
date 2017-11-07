using System.Collections.Generic;

namespace SmarterBalanced.SampleItems.Dal.Configurations.Models
{
    public class SettingsConfig
    {
        public string ItemViewerServiceURL { get; set; }
        public string AccessibilityCookie { get; set; }
        public string BrowserWarningCookie { get; set; }
        public string UserAgentRegex { get; set; }
        public int NumMoreLikeThisItems { get; set; }
        public string ELAPerformanceDescription { get; set; }
        public string MATHPerformanceDescription { get; set; }
        public string DefaultIsaapCodes { get; set; }
    }
}
