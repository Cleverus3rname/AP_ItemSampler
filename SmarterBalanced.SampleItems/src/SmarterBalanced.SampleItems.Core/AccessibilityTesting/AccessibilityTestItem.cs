using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using SmarterBalanced.SampleItems.Dal.Providers;

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
        public string SiwUrl { get; }
        public string IvsUrl { get; }

        public AccessibilityTestItem(
            string resourceUnderTest,
            string subjectCode,
            GradeLevels grade,
            string claimLabel,
            int bankKey,
            int itemKey,
            string siwurl,
            string ivsurl)
        {
            ResourceUnderTest = resourceUnderTest;
            SubjectCode = subjectCode;
            Grade = grade;
            ClaimLabel = claimLabel;
            BankKey = bankKey;
            ItemKey = itemKey;
            SiwUrl = siwurl;
            IvsUrl = ivsurl;
        }

        public static AccessibilityTestItem FromBriefSampleItem(BriefSampleItem briefItem, string resourceUnderTest, SampleItemsContext context)
        {
            var resourceIsaapCode = briefItem.AccessibilityResources.Where(res => res.Label == resourceUnderTest).Select(res => res.CurrentSelectionCode).First();
            return new AccessibilityTestItem(
                resourceUnderTest: resourceUnderTest,
                subjectCode: briefItem.SubjectCode,
                grade: briefItem.Grade,
                claimLabel: briefItem.ClaimLabel,
                bankKey: briefItem.BankKey,
                itemKey: briefItem.ItemKey,
                siwurl: $"{briefItem.Url}?isaap={context.AppSettings.SettingsConfig.DefaultIsaapCodes}{resourceIsaapCode};",
                ivsurl: $"{context.AppSettings.SettingsConfig.ItemViewerServiceURL}//items?ids={briefItem.BankKey}-{briefItem.ItemKey}&isaap={context.AppSettings.SettingsConfig.DefaultIsaapCodes}{resourceIsaapCode};");
        }
    }
}