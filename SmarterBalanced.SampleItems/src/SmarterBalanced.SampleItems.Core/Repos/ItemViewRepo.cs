using SmarterBalanced.SampleItems.Core.Repos.Models;
using SmarterBalanced.SampleItems.Dal.Providers;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using SmarterBalanced.SampleItems.Core.Translations;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using CoreFtp;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using SmarterBalanced.SampleItems.Core.Braille;
using SmarterBalanced.SampleItems.Dal.Translations;

namespace SmarterBalanced.SampleItems.Core.Repos
{
    public class ItemViewRepo : BrailleRepo, IItemViewRepo
    {
        private readonly SampleItemsContext context;
        private readonly ILogger logger;

        public ItemViewRepo(SampleItemsContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
        {
            this.context = context;
            logger = loggerFactory.CreateLogger<ItemViewRepo>();
        }

        public SampleItem GetSampleItem(int bankKey, int itemKey)
        {
            return context.GetSampleItem(bankKey, itemKey);
        }

        public ItemCardViewModel GetItemCardViewModel(int bankKey, int itemKey)
        {
            return context.ItemCards.SingleOrDefault(item => item.BankKey == bankKey && item.ItemKey == itemKey);
        }

        /// <summary>
        /// Constructs an itemviewerservice URL to access the 
        /// item corresponding to the given SampleItem.
        /// </summary>
        public string GetItemViewerUrl()
        {
            string baseUrl = context.AppSettings.SettingsConfig.ItemViewerServiceURL;
            return baseUrl;
        }


        private string GetPerformanceDescription(SampleItem item)
        {
            if (!item.IsPerformanceItem)
            {
                //No description for non performance items
                return string.Empty;
            }

            if (item.Subject.Code.ToLower() == "math")
            {
                return context.AppSettings.SettingsConfig.MATHPerformanceDescription;
            }

            else if (item.Subject.Code.ToLower() == "ela")
            {
                return context.AppSettings.SettingsConfig.ELAPerformanceDescription;
            }
            //Unknown subject
            return string.Empty;
        }

        public string GetItemNames(SampleItem item)
        {
            if (item == null)
            {
                return string.Empty;
            }

            var itemNames = item.ToString();

            if (item.IsPerformanceItem)
            {
                var aboutItem = context.AboutAllItems
                    .FirstOrDefault(ai => ai.ItemCardViewModel != null
                        && ai.ItemCardViewModel.ItemKey == item.ItemKey
                        && ai.ItemCardViewModel.BankKey == item.BankKey);

                if (aboutItem == null)
                {
                    return string.Empty;
                }

                itemNames = aboutItem.AssociatedItems;
            }

            return itemNames;
        }

        public ItemViewModel GetItemViewModel(int bankKey, int itemKey)
        {
            var sampleItem = GetSampleItem(bankKey, itemKey);
            if (sampleItem == null)
            {
                return null;
            }

            var associatedBraille = GetAssoicatedBrailleItem(sampleItem);

            ItemIdentifier nonBrailleItem = sampleItem.ToItemIdentifier();
            ItemIdentifier brailleItem = (associatedBraille ?? sampleItem).ToItemIdentifier();

            var itemViewModel = new ItemViewModel(
                itemViewerServiceUrl: context.AppSettings.SettingsConfig.ItemViewerServiceURL,
                itemNames: GetItemNames(sampleItem),
                brailleItemNames: GetBrailleItemNames(sampleItem),
                brailleItem: brailleItem,
                nonBrailleItem: nonBrailleItem,
                accessibilityCookieName: context.AppSettings.SettingsConfig.AccessibilityCookie,
                isPerformanceItem: sampleItem.IsPerformanceItem,
                moreLikeThisVM: GetMoreLikeThis(sampleItem),
                subject: sampleItem.Subject.Code,
                brailleItemCodes: sampleItem.BrailleItemCodes,
                braillePassageCodes: sampleItem.BraillePassageCodes,
                performanceItemDescription: GetPerformanceDescription(sampleItem),
                defaultIsaapCodes: context.AppSettings.SettingsConfig.DefaultIsaapCodes);

            return itemViewModel;
        }

        public AboutThisItemViewModel GetAboutThisItemViewModel(SampleItem sampleItem)
        {
            if (sampleItem == null)
            {
                return null;
            }

            var aboutThisItemViewModel = context.AboutAllItems.FirstOrDefault(item =>
                item.ItemCardViewModel?.BankKey == sampleItem.BankKey
                && item.ItemCardViewModel?.ItemKey == sampleItem.ItemKey);

            return aboutThisItemViewModel;
        }

        private MoreLikeThisColumn ToColumn(IEnumerable<ItemCardViewModel> itemCards, GradeLevels grade)
        {
            if (itemCards == null)
            {
                return null;
            }

            string label = grade.ToDisplayString();
            var column = new MoreLikeThisColumn(
                label: label, itemCards: itemCards.ToImmutableArray());

            return column;
        }

        /// <summary>
        /// Gets up to 3 items same grade, grade above, and grade below. All items 
        /// </summary>
        /// <param name="grade"></param>
        /// <param name="subject"></param>
        /// <param name="claim"></param>
        public MoreLikeThisViewModel GetMoreLikeThis(SampleItem sampleItem)
        {
            var subjectCode = sampleItem.Subject.Code;
            var claimCode = sampleItem.Claim?.Code;
            var grade = sampleItem.Grade;
            var itemKey = sampleItem.ItemKey;
            var bankKey = sampleItem.BankKey;

            var matchingSubjectClaim = context.ItemCards.Where(i => i.SubjectCode == subjectCode && i.ClaimCode == claimCode);
            int numExpected = context.AppSettings.SettingsConfig.NumMoreLikeThisItems;

            var comparer = new MoreLikeThisComparer(subjectCode, claimCode);

            bool isHighSchool = GradeLevels.High.Contains(grade);
            GradeLevels gradeBelow = isHighSchool ? GradeLevels.Grade8 : grade.GradeBelow();
            GradeLevels gradeAbove = grade.GradeAbove();

            IEnumerable<ItemCardViewModel> cardsGradeAbove = null;

            // Only display above if not a high school grade
            if (!isHighSchool)
            {
                // take grade 11 if gradeabove is high school (only high school items are grade 11)
                gradeAbove = GradeLevels.High.Contains(gradeAbove) ? GradeLevels.Grade11 : gradeAbove;
                cardsGradeAbove = context.ItemCards
                    .Where(i => i.Grade == gradeAbove)
                    .OrderBy(i => i, comparer)
                    .Take(numExpected);
            }

            var cardsSameGrade = context.ItemCards
                .Where(i => i.Grade == grade && i.ItemKey != itemKey)
                .OrderBy(i => i, comparer)
                .Take(numExpected);

            var cardsGradeBelow = context.ItemCards
                .Where(i => i.Grade == gradeBelow)
                .OrderBy(i => i, comparer)
                .Take(numExpected);

            var moreLikeThisVM = new MoreLikeThisViewModel(
                ToColumn(cardsGradeBelow, gradeBelow),
                ToColumn(cardsSameGrade, grade),
                ToColumn(cardsGradeAbove, gradeAbove)
                );

            return moreLikeThisVM;
        }

        public AboutThisItemViewModel GetAboutThisItemViewModel(int itemBank, int itemKey)
        {
            var sampleItem = context.GetSampleItem(itemBank, itemKey);
            var aboutThis = GetAboutThisItemViewModel(sampleItem);

            return aboutThis;
        }



        public ImmutableArray<AccessibilityResourceGroup> GetAccessibilityResourceGroup(
            int itemBank,
            int itemKey,
            string[] iSAAPCodes = default(string[]),
            Dictionary<string, string> cookiePreferences = default(Dictionary<string, string>))
        {
            var sampleItem = context.GetSampleItem(itemBank, itemKey);
            return sampleItem.AccessibilityResourceGroups.ApplyPreferences(iSAAPCodes, cookiePreferences);

        }

        public ImmutableArray<AccessibilityResourceGroup> GetAccessibilityResourceGroup(
            GradeLevels gradeLevels,
            string subjectCode,
            string interactionType,
            string claimId,
            bool? isPerformance,
            bool? aslSupported,
            bool? allowCalculator,
            string[] isaap,
            Dictionary<string, string> cookiePreferences = default(Dictionary<string, string>))

        {
            var accessibility = context.MergedAccessibilityFamilies;
            Claim claim = context.Claims.FirstOrDefault(c => c.Code == claimId);


            var resourceGroups = SampleItemTranslation.GetAccessibilityResourceGroups(
                    resourceFamilies: accessibility,
                    grade: gradeLevels,
                    subjectCode: subjectCode,
                    interactionType: interactionType,
                    settings: context.AppSettings,
                    claim: claim,
                    isPerformance: isPerformance,
                    aslSupported: aslSupported,
                    allowCalculator: allowCalculator
                );

            if (isaap.Length > 0)
            {
                return resourceGroups.ApplyIsaapPreferences(isaap);
            }
            else if (cookiePreferences.Count > 0)
            {
                return resourceGroups.ApplyCookiePreferences(cookiePreferences);
            }
            else
            {
                return resourceGroups;
            }
        }

    }

}
