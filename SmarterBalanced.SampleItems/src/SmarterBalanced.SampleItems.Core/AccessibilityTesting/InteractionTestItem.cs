using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using SmarterBalanced.SampleItems.Dal.Providers;

namespace SmarterBalanced.SampleItems.Core.AccessibilityTesting
{
    public class InteractionTestItem
    {
        public string InteractionUnderTest { get; }
        public string SubjectCode { get; }
        public GradeLevels Grade { get; }
        public string ClaimLabel { get; }
        public int BankKey { get; }
        public int ItemKey { get; }
        public string SiwUrl { get; }
        public string IvsUrl { get; }

        public InteractionTestItem(
            string interactionUnderTest,
            string subjectCode,
            GradeLevels grade,
            string claimLabel,
            int bankKey,
            int itemKey,
            string siwUrl,
            string ivsUrl)
        {
            InteractionUnderTest = interactionUnderTest;
            SubjectCode = subjectCode;
            Grade = grade;
            ClaimLabel = claimLabel;
            BankKey = bankKey;
            ItemKey = itemKey;
            SiwUrl = siwUrl;
            IvsUrl = ivsUrl;
        }

        public static InteractionTestItem FromBriefSampleItem(BriefSampleItem briefItem, InteractionType interaction, SampleItemsContext context)
        {
            return new InteractionTestItem(
                interactionUnderTest: interaction.Label,
                subjectCode: briefItem.SubjectCode,
                grade: briefItem.Grade,
                claimLabel: briefItem.ClaimLabel,
                bankKey: briefItem.BankKey,
                itemKey: briefItem.ItemKey,
                siwUrl: $"{briefItem.Url}?isaap={context.AppSettings.SettingsConfig.DefaultIsaapCodes}",
                ivsUrl: $"{context.AppSettings.SettingsConfig.ItemViewerServiceURL}//items?ids={briefItem.BankKey}-{briefItem.ItemKey}&isaap={context.AppSettings.SettingsConfig.DefaultIsaapCodes}");
        }
    }
}