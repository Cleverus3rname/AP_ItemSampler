﻿using System.Collections.Generic;

namespace SmarterBalanced.SampleItems.Dal.Configurations.Models
{
    public class SettingsConfig
    {
        public string ContentItemDirectory { get; set; }
        public string ContentRootDirectory { get; set; }
        public string BrailleFileManifest { get; set; }
        public string AccommodationsXMLPath { get; set; }
        public string InteractionTypesXMLPath { get; set; }
        public string ClaimsXMLPath { get; set; }
        public string PatchXMLPath { get; set; }
        public string AwsRegion { get; set; }
        public string AwsS3Bucket { get; set; }
        public string ItemViewerServiceURL { get; set; }
        public string AwsClusterName { get; set; }
        public string StatusUrl { get; set; }
        public string AccessibilityCookie { get; set; }
        public Dictionary<string, string> LanguageToLabel { get; set; }
        public Dictionary<string, string> InteractionTypesToItem { get; set; }
        public List<string> DictionarySupportedItemTypes { get; set; }
        public List<AccessibilityType> AccessibilityTypes { get; set; }
        public string BrowserWarningCookie { get; set; }
        public string UserAgentRegex { get; set; }
        public int NumMoreLikeThisItems { get; set; }
        public string SmarterBalancedFtpHost { get; set; }
        public string SmarterBalancedFtpUsername { get; set; }
        public string SmarterBalancedFtpPassword { get; set; }
        public string BrailleFtpBaseDirectory { get; set; }
        public string ELAPerformanceDescription { get; set; }
        public string MATHPerformanceDescription { get; set; }
        public string CoreStandardsXMLPath { get; set; }
        public string[] SupportedPublications { get; set; }
        public Dictionary<string, string> OldToNewInteractionType { get; set; }
    }
}
