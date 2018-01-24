using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmarterBalanced.SampleItems.Dal.Providers.Models
{
    public sealed class ItemCardViewModel
    {
        public int BankKey { get; }
        public int ItemKey { get; }
        public string Title { get; }
        public GradeLevels Grade { get; }
        public string GradeLabel { get; }
        public string SubjectCode { get; }
        public string SubjectLabel { get; }
        public string ClaimCode { get; }
        public string ClaimLabel { get; }
        public int TargetHash { get; }
        public string TargetId { get; }
        public string TargetShortName { get; }
        public string TargetDescription { get; }
        public string InteractionTypeCode { get; }
        public string InteractionTypeLabel { get; }
        public bool IsPerformanceItem { get; }
        public bool BrailleOnlyItem { get; }
        public string Domain { get; }
        public string DepthOfKnowledge { get; }
        public string CommonCoreStandardId { get; }
        public string CcssDescription { get; }
        public bool Calculator { get; }

        public ItemCardViewModel(
            int bankKey,
            int itemKey,
            string title,
            GradeLevels grade,
            string gradeLabel,
            string subjectCode,
            string subjectLabel,
            string claimCode,
            string claimLabel,
            int targetHash,
            string targetId,
            string targetShortName,
            string targetDesc,
            string interactionTypeCode,
            string interactionTypeLabel,
            bool isPerformanceItem,
            bool brailleOnlyItem,
            string domain,
            string depthOfKnowledge,
            string ccss,
            string ccssDesc,
            bool calculator)
        {
            TargetDescription = targetDesc;
            BankKey = bankKey;
            ItemKey = itemKey;
            Title = title;
            Grade = grade;
            GradeLabel = gradeLabel;
            SubjectCode = subjectCode;
            SubjectLabel = subjectLabel;
            ClaimCode = claimCode;
            ClaimLabel = claimLabel;
            TargetHash = targetHash;
            TargetShortName = targetShortName;
            InteractionTypeCode = interactionTypeCode;
            InteractionTypeLabel = interactionTypeLabel;
            IsPerformanceItem = isPerformanceItem;
            BrailleOnlyItem = brailleOnlyItem;
            DepthOfKnowledge = depthOfKnowledge;
            Domain = domain;
            CommonCoreStandardId = ccss;
            CcssDescription = ccssDesc;
            TargetId = targetId;
            Calculator = calculator;
        }

        /// <summary>
        /// Used for testing so that it's not necessary to specify all parameters.
        /// </summary>
        public static ItemCardViewModel Create(
           int bankKey = -1,
           int itemKey = -1,
           string title = "",
           GradeLevels grade = GradeLevels.NA,
           string gradeLabel = "",
           string subjectCode = "",
           string subjectLabel = "",
           string claimCode = "",
           string claimLabel = "",
           int targetHash = -1,
           string targetId = "",
           string targetShortName = "",
           string targetDesc = "",
           string interactionTypeCode = "",
           string interactionTypeLabel = "",
           bool isPerformanceItem = false,
           bool brailleOnlyitem = false,
           string domain = "",
           string depthOfKnowledge = "",
           string ccss = "",
           string ccssDesc = "",
           bool calculator = false)
        {
            return new ItemCardViewModel(
                bankKey: bankKey,
                itemKey: itemKey,
                title: title,
                grade: grade,
                gradeLabel: gradeLabel,
                subjectCode: subjectCode,
                subjectLabel: subjectLabel,
                claimCode: claimCode,
                claimLabel: claimLabel,
                targetHash: targetHash,
                targetShortName: targetShortName,
                targetDesc: targetDesc,
                interactionTypeCode: interactionTypeCode,
                interactionTypeLabel: interactionTypeLabel,
                isPerformanceItem: isPerformanceItem,
                brailleOnlyItem: brailleOnlyitem,
                domain: domain,
                depthOfKnowledge: depthOfKnowledge,
                ccss: ccss,
                ccssDesc: ccssDesc,
                targetId: targetId,
                calculator: calculator);
        }
    }

    public class MoreLikeThisComparer : IComparer<ItemCardViewModel>
    {
        private readonly string subjectCode;
        private readonly string claimCode;

        public MoreLikeThisComparer(string subjectCode, string claimCode)
        {
            this.subjectCode = subjectCode;
            this.claimCode = claimCode;
        }

        private int Weight(ItemCardViewModel itemCardVM)
        {
            int weight = 2;
            if (itemCardVM.SubjectCode == subjectCode)
                weight--;

            if (itemCardVM.ClaimCode == claimCode)
                weight--;

            return weight;
        }

        /// <summary>
        /// Compares ItemCardViewModel by subject and claim similarity
        /// </summary>
        /// <remarks>
        /// positive return value: x is bigger (x - y)
        /// negative return value: y is bigger
        /// "bigger" means it will appear later when sorted in ascending order
        /// </remarks>
        public int Compare(ItemCardViewModel x, ItemCardViewModel y)
        {
            int weightDiff = Weight(x) - Weight(y);

            return weightDiff;
        }

    }

}
