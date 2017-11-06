using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using System.Collections.Immutable;

namespace SmarterBalanced.SampleItems.Dal.Translations
{
    public static class AccessibilityResourceTranslation
    {
        public static ImmutableArray<AccessibilityResource> ToGlobalAccessibilityResources(
            this IEnumerable<XElement> singleSelectResources)
        {
            var accessibilityResources = singleSelectResources
                .Select(xs => AccessibilityResource.Create(xs))
                .Where(a => a.Selections.Any())
                .OrderBy(a => a.Order)
                .ToImmutableArray();

            return accessibilityResources;
        }

        public static ImmutableArray<AccessibilityFamily> ToAccessibilityResourceFamilies(
            this IEnumerable<XElement> elem)
        {
            var families = elem
                .Select(rf => AccessibilityFamily.Create(rf))
                .ToImmutableArray();

            return families;
        }

        /// <summary>
        /// Translates Partial family resources with global resources to full set of family resources
        /// </summary>
        public static MergedAccessibilityFamily MergeGlobalResources(
            this AccessibilityFamily family,
            IEnumerable<AccessibilityResource> globalResources)
        {
            var newResources = globalResources.Select(globalResource =>
            {
                var familyResource = family.Resources.FirstOrDefault(fr => fr.ResourceCode == globalResource.ResourceCode);
                if (familyResource == null)
                {
                    return globalResource;
                }
                else
                {
                    return MergeGlobalResource(familyResource, globalResource);
                }
            }).ToImmutableArray();

            var mergedFamily = new MergedAccessibilityFamily(
                subjects: family.Subjects,
                grades: family.Grades,
                resources: newResources);

            return mergedFamily;
        }

        private static AccessibilitySelection MergeSelection(AccessibilitySelection sel, AccessibilityFamilyResource familyResource)
        {
            var familySel = familyResource.Selections.SingleOrDefault(fs => fs.Code == sel.SelectionCode);
            var selDisabled = familyResource.Disabled || familySel == null || familySel.Hidden;
            string label = (!string.IsNullOrEmpty(familySel?.Label)) ? familySel.Label : sel.Label;

            var newSelection = new AccessibilitySelection(
                code: sel.SelectionCode,
                label: label,
                order: sel.Order,
                disabled: selDisabled,
                hidden: familySel?.Hidden ?? sel.Hidden);

            return newSelection;
        }

        public static AccessibilityResource MergeGlobalResource(AccessibilityFamilyResource familyResource, AccessibilityResource globalResource)
        {
            if (familyResource == null)
                throw new ArgumentNullException(nameof(familyResource));
            else if (globalResource == null)
                throw new ArgumentNullException(nameof(globalResource));

            var newSelections = globalResource.Selections
                .Select(sel => MergeSelection(sel, familyResource))
                .ToImmutableArray();

            string defaultSelection = (!string.IsNullOrEmpty(familyResource.DefaultSelection)) ? familyResource.DefaultSelection : globalResource.DefaultSelection;

            var matchingSelection = newSelections.FirstOrDefault(s => s.SelectionCode == defaultSelection);
            bool isDefaultInvalid = matchingSelection == null || matchingSelection.Disabled;

            string newDefault = isDefaultInvalid
                ? newSelections.FirstOrDefault(s => !s.Disabled && !s.Hidden)?.SelectionCode ?? defaultSelection
                : defaultSelection;

            var newResource = new AccessibilityResource(
                resourceCode: globalResource.ResourceCode,
                currentSelectionCode: newDefault,
                order: globalResource.Order,
                defaultSelection: newDefault,
                selections: newSelections,
                label: globalResource.Label,
                description: globalResource.Description,
                disabled: familyResource.Disabled,
                resourceType: globalResource.ResourceTypeId);

            return newResource;
        }

        public static ImmutableArray<MergedAccessibilityFamily> CreateMergedFamilies(XElement accessibilityXml)
        {
            ImmutableArray<AccessibilityResource> globalResources = accessibilityXml
                .Element("MasterResourceFamily")
                .Elements("SingleSelectResource")
                .ToGlobalAccessibilityResources();

            ImmutableArray<AccessibilityFamily> families = accessibilityXml
                .Elements("ResourceFamily")
                .ToAccessibilityResourceFamilies();

            ImmutableArray<MergedAccessibilityFamily> mergedFamilies = families
                .Select(f => MergeGlobalResources(f, globalResources))
                .ToImmutableArray();

            return mergedFamilies;
        }

        /// <summary>
        /// Disables accessibility resources for special cases:
        ///     - ASL is disabled by itemMetadata flag
        ///     - Calculator is disabled if metadata flag or resource is disabled
        ///     - GlobalNotes is only enabled for performance task items
        ///     - EnglishDictionary and Thesaurus are only enabled for WER items
        /// </summary>

        public static AccessibilityResource ApplyFlags(
            this AccessibilityResource resource,
            string subjectCode,
            List<string> dictionarySupportedItemTypes,
            IEnumerable<string> supportedBraille = default(IEnumerable<string>),
            Claim claim = null,
            bool? isPerformanceTask = null,
            string interactionType = default(string),
            bool? aslSupported = null,
            bool? allowCalculator = null)
        {
            if (string.IsNullOrEmpty(subjectCode) || resource.Disabled)
            {
                return resource;
            }

            bool isUnsupportedAsl = aslSupported.HasValue && !aslSupported.Value && resource.ResourceCode == "AmericanSignLanguage";
            bool isUnsupportedCalculator = allowCalculator.HasValue && (!allowCalculator.Value || resource.Disabled) && resource.ResourceCode == "Calculator";
            bool isUnsupportedGlobalNotes = isPerformanceTask.HasValue && !isPerformanceTask.Value && resource.ResourceCode == "GlobalNotes";
            bool isUnsupportedDictionaryThesaurus = !string.IsNullOrEmpty(interactionType) && !dictionarySupportedItemTypes.Any(s => s == interactionType)
                && (resource.ResourceCode == "EnglishDictionary" || resource.ResourceCode == "Thesaurus");
            bool isUnsupportedClosedCaptioning = !(claim?.ClaimNumber == "3" && subjectCode == "ELA") && resource.ResourceCode == "ClosedCaptioning";

            if (isUnsupportedAsl || isUnsupportedCalculator || isUnsupportedGlobalNotes || isUnsupportedDictionaryThesaurus || isUnsupportedClosedCaptioning) 
            {
                var newResource = resource.ToDisabled();
                return newResource;
            } else if(supportedBraille != null && resource.ResourceCode == "BrailleType")
            {
                return resource.DisableUnsupportedBraille(supportedBraille);
            }

            return resource;
        }

    }
}