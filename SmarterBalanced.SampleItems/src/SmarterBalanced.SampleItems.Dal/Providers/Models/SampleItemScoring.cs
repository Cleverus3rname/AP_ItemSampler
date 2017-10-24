using SmarterBalanced.SampleItems.Dal.Xml.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace SmarterBalanced.SampleItems.Dal.Providers.Models
{
    public class SampleItemScoring
    {
        /// <summary>
        /// The correct answer for smarter app options
        /// </summary>
        public string AnswerKey { get; }

        /// <summary>
        /// Item has machine rubric
        /// </summary>
        public bool HasMachineRubric { get; }

        /// <summary>
        /// Optional, Question options for an item
        /// </summary>
        public ImmutableArray<SmarterAppOption> ScoringOptions { get; }

        /// <summary>
        /// Optional, Item rubric and sample responses for an item
        /// </summary>
        public ImmutableArray<Rubric> Rubrics { get; }

        public SampleItemScoring(
            string answerKey,
            bool hasMachineRubric,
            ImmutableArray<SmarterAppOption> scoringOptions,
            ImmutableArray<Rubric> rubrics)
        {
            AnswerKey = answerKey;
            HasMachineRubric = hasMachineRubric;
            ScoringOptions = scoringOptions;
        }

        public static SampleItemScoring Create(
            string answerKey = "",
            bool hasMachineRubric = false,
            ImmutableArray<SmarterAppOption> scoringOptions = new ImmutableArray<SmarterAppOption>(),
            ImmutableArray<Rubric> rubrics = new ImmutableArray<Rubric>())
        {
            return new SampleItemScoring(
                    answerKey: answerKey,
                    hasMachineRubric: hasMachineRubric,
                    scoringOptions: scoringOptions,
                    rubrics: rubrics
                );

        }

    }

}
