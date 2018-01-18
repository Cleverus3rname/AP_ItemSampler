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
        public string Url { get; }
        public ImmutableArray<AccessibilityResource> AccessibilityResources { get; }

        public BriefSampleItem(
            int bankKey,
            int itemKey,
            GradeLevels grade,
            string subjectCode,
            string claimLabel,
            string url,
            ImmutableArray<AccessibilityResource> accessibilityResource)
        {
            BankKey = bankKey;
            ItemKey = itemKey;
            Grade = grade;
            SubjectCode = subjectCode;
            ClaimLabel = claimLabel;
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
                url: $"{baseUrl}/Item/{sampleItem.BankKey}-{sampleItem.ItemKey}",
                accessibilityResource: sampleItem.AccessibilityResourceGroups
                    .SelectMany(group => group.AccessibilityResources).ToImmutableArray());
        }

        // public static BriefSampleItem PrependURL(this BriefSampleItem briefItem, string baseUrl)
        // {
        //     return new BriefSampleItem(
        //         bankKey: briefItem.BankKey,
        //         itemKey: briefItem.ItemKey,
        //         grade: briefItem.Grade,
        //         subjectCode: briefItem.SubjectCode,
        //         claimLabel: briefItem.ClaimLabel,
        //         url: $"{baseUrl}/Item/?bankKey={briefItem.BankKey}&itemKey={briefItem.ItemKey}",
        //         briefResources: briefItem.BriefResources);
        // }
    }


    // public class DisabledResourcesComparer : IComparer<BriefSampleItem>
    // {
    //     private readonly int numDisabledResources;
    //     public DisabledResourcesComparer(int numDisabledResources)
    //     {
    //         this.numDisabledResources = numDisabledResources;
    //     }

    //     private int Weight(BriefSampleItem checkItem)
    //     {
    //         int weight = 2;
    //         if (checkItem.BriefResources.Length < numDisabledResources)
    //             weight--;
            
    //         if (checkItem.BriefResources.Length >= numDisabledResources)
    //             weight++;

    //         return weight;
    //     }

    //     public int Compare(BriefSampleItem x, BriefSampleItem y)
    //     {
    //         int weightDiff = Weight(x) - Weight(y);
    //         return weightDiff;
    //     }
    // }
}