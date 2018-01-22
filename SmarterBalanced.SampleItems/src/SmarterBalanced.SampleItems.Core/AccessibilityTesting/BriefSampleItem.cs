using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using SmarterBalanced.SampleItems.Dal.Providers.Models;

namespace SmarterBalanced.SampleItems.Core.AccessibilityTesting
{
    public class BriefSampleItem
    {
        public int BankKey  { get; }
        public int ItemKey { get; }
        public GradeLevels Grade { get; }
        public string SubjectCode { get; }
        public string ClaimLabel { get; }
        public InteractionType InteractionType { get; }
        public string Url { get; }
        public ImmutableArray<AccessibilityResource> AccessibilityResources { get; }

        public BriefSampleItem(
            int bankKey,
            int itemKey,
            GradeLevels grade,
            string subjectCode,
            string claimLabel,
            InteractionType interactionType,
            string url,
            ImmutableArray<AccessibilityResource> accessibilityResource)
        {
            BankKey = bankKey;
            ItemKey = itemKey;
            Grade = grade;
            SubjectCode = subjectCode;
            ClaimLabel = claimLabel;
            InteractionType = interactionType;
            Url = url;
            AccessibilityResources = accessibilityResource;
        }

        public static BriefSampleItem FromSampleItem(SampleItem sampleItem, string baseUrl = "")
        {
            var disabledResources = sampleItem.AccessibilityResourceGroups
                .SelectMany(group => group.AccessibilityResources
                .Where(r => r.Disabled == true).ToImmutableArray()).ToImmutableArray();
            var resourceArray = disabledResources.Select(r => r.ToBriefAccessibilityResource()).ToImmutableArray();
            return new BriefSampleItem(
                bankKey: sampleItem.BankKey,
                itemKey: sampleItem.ItemKey,
                grade: sampleItem.Grade,
                subjectCode: sampleItem.Subject.Label,
                claimLabel: sampleItem.Claim.Label,
                interactionType: sampleItem.InteractionType,
                url: $"{baseUrl}/Item/{sampleItem.BankKey}-{sampleItem.ItemKey}",
                accessibilityResource: sampleItem.AccessibilityResourceGroups
                    .SelectMany(group => group.AccessibilityResources).ToImmutableArray());
        }
    }
}