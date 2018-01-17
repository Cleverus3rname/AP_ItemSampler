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
        public int ItemKey { get; }
        public GradeLevels Grade { get; }
        public string SubjectCode { get; }
        public string ClaimLabel { get; }
        public string InteractionTypeLabel { get; }
        public ImmutableArray<BriefAccessibilityResource> BriefResources { get; }

        public BriefSampleItem(
            int itemKey,
            GradeLevels grade,
            string subjectCode,
            string claimLabel,
            string interactionTypeLabel,
            ImmutableArray<BriefAccessibilityResource> briefResources)
        {
            ItemKey = itemKey;
            Grade = grade;
            SubjectCode = subjectCode;
            ClaimLabel = claimLabel;
            InteractionTypeLabel = interactionTypeLabel;
            BriefResources = briefResources;
        }

        public static BriefSampleItem FromSampleItem(SampleItem sampleItem)
        {
            var disabledResources = sampleItem.AccessibilityResourceGroups
                .SelectMany(group => group.AccessibilityResources
                .Where(r => r.Disabled == true).ToImmutableArray()).ToImmutableArray();
            var resourceArray = disabledResources.Select(r => r.ToBriefAccessibilityResource()).ToImmutableArray();
            return new BriefSampleItem(
                itemKey: sampleItem.ItemKey,
                grade: sampleItem.Grade,
                subjectCode: sampleItem.Subject.Label,
                claimLabel: sampleItem.Claim.Label,
                interactionTypeLabel: sampleItem.Claim.ClaimNumber,
                briefResources: resourceArray);
        }
    }


    public class DisabledResourcesComparer : IComparer<BriefSampleItem>
    {
        private readonly int numDisabledResources;
        public DisabledResourcesComparer(int numDisabledResources)
        {
            this.numDisabledResources = numDisabledResources;
        }

        private int Weight(BriefSampleItem checkItem)
        {
            int weight = 2;
            if (checkItem.BriefResources.Length < numDisabledResources)
                weight--;
            
            if (checkItem.BriefResources.Length >= numDisabledResources)
                weight++;

            return weight;
        }

        public int Compare(BriefSampleItem x, BriefSampleItem y)
        {
            int weightDiff = Weight(x) - Weight(y);
            return weightDiff;
        }
    }
}