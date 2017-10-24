using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using SmarterBalanced.SampleItems.Dal.Xml.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace SmarterBalanced.SampleItems.Dal.Translations
{
    public class SampleItemsScoringTranslation
    {
        public static SampleItemScoring ToSampleItemsScore(
            ItemDigest digest,
            AppSettings settings,
            IList<InteractionType> interactionTypes)
        {
            string scoreAttribute = GetScoreAttribute(digest, interactionTypes);
            var scoringOptions = GetScoringOptions(digest, scoreAttribute);
            var rubrics = GetRubrics(digest, settings);
            var scoring = SampleItemScoring.Create(
                answerKey: scoreAttribute,
                hasMachineRubric: digest.HasMachineRubric,
                scoringOptions: scoringOptions,
                rubrics: rubrics);

            return scoring;
           
        }

        private static ImmutableArray<SmarterAppOption> GetScoringOptions(
            ItemDigest digest,
            string scoreAttribute)
        {
            var options = digest.Contents
                .SelectMany(c => c.ScoringOptions
                    .Select(so => 
                        so.WithOptions(IsCorrectResponse(so, scoreAttribute), c.Language)));

            return options.ToImmutableArray();
        }

        private static bool IsCorrectResponse(SmarterAppOption option, string scoreAttribute)
        {
            bool isCorrect = false;
            if (!String.IsNullOrEmpty(scoreAttribute))
            {
                string[] answers = scoreAttribute.Split(',');
                isCorrect = answers.Any(a => option.Name.Equals($"Option {a}"));
            }

            return isCorrect;
        }

        public static string GetScoreAttribute(ItemDigest digest, IList<InteractionType> interactionTypes)
        {
            string val = digest
                .ItemMetadataAttributes
                .FirstOrDefault(i => i.Code.Equals("itm_att_Answer Key"))?.Value;

            return interactionTypes
                .Any(i => i.Code.Equals(val))
                ? string.Empty : val;
        }

        public static ImmutableArray<Rubric> GetRubrics(ItemDigest digest, AppSettings settings)
        {
            int? maxPoints = digest.MaximumNumberOfPoints;
            var rubrics = digest.Contents
                .Select(c => 
                    ToRubric(c, maxPoints, settings))
                .Where(r => r != null).ToImmutableArray();
            return rubrics;
        }

        /// <summary>
        /// Returns a Single Rubric from content and filters out any placeholder text
        /// </summary>
        public static Rubric ToRubric(
            Content content,
            int? maxPoints,
            AppSettings appSettings)
        {
            if (appSettings == null || appSettings.SbContent.RubricPlaceHolderText == null || appSettings.SbContent == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            var placeholder = appSettings.SbContent.RubricPlaceHolderText;
            var languageToLabel = appSettings.SbContent.LanguageToLabel;

            if (content == null ||
                content.RubricList == null ||
                content.RubricList.Rubrics == null ||
                content.RubricList.RubricSamples == null)
            {
                return null;
            }

            int scorePoint;
            var rubricEntries = content.RubricList.Rubrics
                .Where(r => !string.IsNullOrWhiteSpace(r.Value)
                    && int.TryParse(r.Scorepoint, out scorePoint)
                    && scorePoint <= maxPoints
                    && !placeholder.RubricPlaceHolderContains.Any(s => r.Value.Contains(s))
                    && !placeholder.RubricPlaceHolderEquals.Any(s => r.Value.Equals(s))).ToImmutableArray();

            Predicate<SampleResponse> pred = (r => string.IsNullOrWhiteSpace(r.SampleContent)
                                                     || placeholder.RubricPlaceHolderContains.Any(s => r.SampleContent.Contains(s))
                                                     || placeholder.RubricPlaceHolderEquals.Any(s => r.SampleContent.Equals(s)));

            content.RubricList.RubricSamples.ForEach(t => t.SampleResponses.RemoveAll(pred));

            var samples = content.RubricList.RubricSamples.Where(t => t.SampleResponses.Count() > 0).ToImmutableArray();
            if (rubricEntries.Length == 0 && samples.Length == 0)
            {
                return null;
            }

            string languangeLabel = (string.IsNullOrEmpty(content.Language)) ? string.Empty :
                                                languageToLabel[content.Language.ToUpper()];

            var rubric = new Rubric(languangeLabel, rubricEntries, samples);
            return rubric;
        }

    }
}
