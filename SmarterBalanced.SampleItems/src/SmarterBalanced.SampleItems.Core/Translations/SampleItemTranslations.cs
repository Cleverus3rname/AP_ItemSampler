using SmarterBalanced.SampleItems.Core.Repos.Models;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmarterBalanced.SampleItems.Core.Translations
{
    public static class SampleItemTranslations
    {

        public static SampleItemViewModel ToSampleItemViewModel(this SampleItem sampleItem, string baseUrl)
        {
            string claimTitle = (string.IsNullOrEmpty(sampleItem.Claim?.ClaimNumber)) ? string.Empty : $"Claim {sampleItem.Claim.ClaimNumber}";
            string title = $"{sampleItem.Subject?.ShortLabel} {sampleItem.Grade.ToDisplayString()} {claimTitle}";
            string url = $"{baseUrl}/Item/Details?bankKey={sampleItem.BankKey}&itemKey={sampleItem.ItemKey}";
            
            var vm = SampleItemViewModel.Create(
             bankKey: sampleItem.BankKey,
             itemKey: sampleItem.ItemKey,
             title: title,
             gradeLabel: sampleItem.Grade.ToDisplayString(),
             subjectCode: sampleItem.Subject?.Code,
             subjectLabel: sampleItem.Subject?.ShortLabel,
             claimCode: sampleItem.Claim?.Code,
             claimLabel: sampleItem.Claim?.Label,
             target: sampleItem.CoreStandards?.Target?.IdLabel,
             interactionTypeCode: sampleItem.InteractionType?.Code,
             interactionTypeLabel: sampleItem.InteractionType?.Label,
             isPerformanceItem: sampleItem.IsPerformanceItem,
             aslSupported: sampleItem.AslSupported,
             stimulusKey: sampleItem.AssociatedStimulus,
             ccssDesc: sampleItem.CoreStandards?.CommonCoreStandardsDescription,
             targetDesc: sampleItem.CoreStandards?.Target.Descripton,
             url: url,
             depthOfKnowledge: sampleItem.DepthOfKnowledge,
             exemplar: GetFormattedExamples(sampleItem),
             rubric: GetFormattedRubric(sampleItem),
             answerKey: sampleItem.SampleItemScoring?.AnswerKey,
             answerOption: GetFormattedScoringOptions(sampleItem),
             hasMachineRubric: sampleItem.SampleItemScoring?.HasMachineRubric ?? false,
             publication: sampleItem.CoreStandards?.Publication
             );

            return vm;
        }

        private static string GetFormattedRubric(SampleItem sampleItem)
        {
            string rubricString = "";
            if(sampleItem.SampleItemScoring.Rubrics.Any() == true)
            {
                var rubricEntries = sampleItem.SampleItemScoring?
                    .Rubrics
                    .SelectMany(r => r.RubricEntries)
                    .Select(re => $"Score Point {re.Scorepoint}: {re.Value}\r\n");
                rubricString = String.Join("---\r\n", rubricEntries);
            }

            return rubricString;
        }

        private static string GetFormattedExamples(SampleItem sampleItem)
        {
            string examplesString = "";
            if (sampleItem.SampleItemScoring.Rubrics.Any() == true)
            {
              var examples = sampleItem.SampleItemScoring?.Rubrics
                    .SelectMany(r => r.Samples)
                    .SelectMany(r => r.SampleResponses)
                    .Select(sr => $"Score Point {sr.ScorePoint}: {sr.SampleContent}\r\n");
                examplesString = String.Join("---\r\n", examples);
            }

            return examplesString;
        }

        private static string GetFormattedScoringOptions(SampleItem sampleItem)
        {
            string scoringOptionString = "";
            if (sampleItem.SampleItemScoring.ScoringOptions.Any() == true)
            {
                var answerFeedback = sampleItem.SampleItemScoring?
                    .ScoringOptions
                    .Select(so => $"Answers {so.Feedback}\r\n");
                scoringOptionString = String.Join("---\r\n", answerFeedback);
            }

            return scoringOptionString;
        }

        public static ItemIdentifier ToItemIdentifier(this SampleItem sampleItem)
        {
            var item = new ItemIdentifier(
                    itemKey: sampleItem.ItemKey,
                    itemName: sampleItem.ToString(),
                    bankKey: sampleItem.BankKey);

            return item;
        }
    }
}
