using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SmarterBalanced.SampleItems.Dal.Providers.Models
{

    public class AboutThisItemViewModel
    {
        public ItemCardViewModel ItemCardViewModel { get; }
        public SampleItemScoring SampleItemScoring { get; }
        public string TargetDescription { get; }
        public string DepthOfKnowledge { get; }
        public string CommonCoreStandardsDescription { get; }
        public string EducationalDifficulty { get; }
        public string EvidenceStatement { get; }
        public string AssociatedItems { get; }

        public AboutThisItemViewModel(
            ItemCardViewModel itemCard,
            SampleItemScoring scoring,
            string targetDescription,
            string depthOfKnowledge,
            string commonCoreStandardsDescription,
            string educationalDifficulty,
            string evidenceStatement,
            string associatedItems
            )
        {
            ItemCardViewModel = itemCard;
            SampleItemScoring = scoring;
            TargetDescription = targetDescription;
            DepthOfKnowledge = depthOfKnowledge;
            CommonCoreStandardsDescription = commonCoreStandardsDescription;
            EducationalDifficulty = educationalDifficulty;
            EvidenceStatement = evidenceStatement;
            AssociatedItems = associatedItems;
        }

        public static AboutThisItemViewModel Create(
          ItemCardViewModel itemCard = null,
          SampleItemScoring scoring = null,
          string targetDescription = "",
          string depthOfKnowledge = "",
          string commonCoreStandardsDescription = "",
          string educationalDifficulty = "",
          string evidenceStatement = "",
          string associatedItems = "")
        {

            return new AboutThisItemViewModel(
                itemCard: itemCard,
                scoring: scoring,
                targetDescription: targetDescription,
                depthOfKnowledge: depthOfKnowledge,
                commonCoreStandardsDescription: commonCoreStandardsDescription,
                educationalDifficulty: educationalDifficulty,
                evidenceStatement: evidenceStatement,
                associatedItems: associatedItems
            );
        }
    }
}
