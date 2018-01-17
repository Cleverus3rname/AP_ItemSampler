using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using SmarterBalanced.SampleItems.Dal.Providers.Models;

namespace SmarterBalanced.SampleItems.Core.AccessibilityTesting
{
    public class AccessibilityTestItem
    {
        public string ResourceUnderTest { get; }
        public string SubjectCode { get; }
        public GradeLevels Grade { get; }
        public string ClaimLabel { get; }
        public int BankKey { get; }
        public int ItemKey { get; }
        public string Url { get; }

        public AccessibilityTestItem(
            string resourceUnderTest,
            string subjectCode,
            GradeLevels grade,
            string claimLabel,
            int bankKey,
            int itemKey,
            string url)
        {
            ResourceUnderTest = resourceUnderTest;
            SubjectCode = subjectCode;
            Grade = grade;
            ClaimLabel = claimLabel;
            BankKey = bankKey;
            ItemKey = itemKey;
            Url = url;
        }

        public static AccessibilityTestItem FromBriefSampleItem(BriefSampleItem briefItem, string resourceUnderTest)
        {
            return new AccessibilityTestItem(
                resourceUnderTest: resourceUnderTest,
                subjectCode: briefItem.SubjectCode,
                grade: briefItem.Grade,
                claimLabel: briefItem.ClaimLabel,
                bankKey: briefItem.BankKey,
                itemKey: briefItem.ItemKey,
                url: briefItem.Url);
        }
    }
}