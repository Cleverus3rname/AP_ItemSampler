using System;
using System.Collections.Generic;
using System.Text;

namespace SmarterBalanced.SampleItems.Dal.Configurations.Models
{
    public class SbContentSettings
    {
        public string ContentRootDirectory { get; set; }
        public string AccommodationsXMLPath { get; set; }
        public string InteractionTypesXMLPath { get; set; }
        public string ClaimsXMLPath { get; set; }
        public string PatchXMLPath { get; set; }
        public string CoreStandardsXMLPath { get; set; }
        public Dictionary<string, string> InteractionTypesToItem { get; set; }
        public string[] SupportedPublications { get; set; }
        public Dictionary<string, string> OldToNewInteractionType { get; set; }
        public RubricPlaceHolderText RubricPlaceHolderText { get; set; }
        public List<string> DictionarySupportedItemTypes { get; set; }
        public List<AccessibilityType> AccessibilityTypes { get; set; }
        public Dictionary<string, string> LanguageToLabel { get; set; }
        public List<FilterCategorySettings> FilterCategories { get; set; }

    }
}
