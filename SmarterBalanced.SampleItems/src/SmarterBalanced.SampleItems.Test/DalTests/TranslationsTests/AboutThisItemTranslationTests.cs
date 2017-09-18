using SmarterBalanced.SampleItems.Dal.Providers.Models;
using SmarterBalanced.SampleItems.Dal.Translations;
using SmarterBalanced.SampleItems.Dal.Xml.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Xunit;

namespace SmarterBalanced.SampleItems.Test.DalTests.TranslationsTests
{
    public class AboutThisItemTranslationTests
    {
        SampleItem PerformanceDigest1, PerformanceDigest2;
        ItemCardViewModel PerformanceCard1, PerformanceCard2;
        int ItemKey1, ItemKey2, BankKey1, BankKey2;
        string TargetDescription, DOK, StandardsDescription, EducationalDifficulty, EvidenceStatement;
        ImmutableArray<ItemCardViewModel> AllItemCards;
        ImmutableArray<SampleItem> AllSampleItems;
        ImmutableArray<Rubric> Rubrics;
        Subject MathSubject;
        FieldTestUse TestUse;

        public AboutThisItemTranslationTests()
        {
            ItemKey1 = 1;
            BankKey1 = 1;
            ItemKey2 = 2;
            BankKey2 = 2;
            TargetDescription = "Target Description";
            StandardsDescription = "Standards Description";
            EducationalDifficulty = "Educational Difficulty";
            EvidenceStatement = "Evidence Statement";
            DOK = "1";
            PerformanceCard1 = ItemCardViewModel.Create(itemKey: ItemKey1, bankKey: BankKey1);
            PerformanceCard2 = ItemCardViewModel.Create(itemKey: ItemKey2, bankKey: BankKey2);
            Rubrics = ImmutableArray.Create(BuildRubric());
            TestUse = new FieldTestUse()
            {
                Code = "MATH",
                Section = "hi",
                QuestionNumber = 2
            };
            MathSubject = Subject.Create(
                code: "MATH", 
                label: "Mathematics" , 
                shortLabel: "Math", 
                claims: ImmutableArray<Claim>.Empty, 
                interactionTypeCodes: ImmutableArray<string>.Empty);
            PerformanceDigest1 = SampleItem.Create(
                itemKey: ItemKey1,
                bankKey: BankKey1,
                coreStandards: CoreStandards.Create(
                    target: Target.Create(
                        desc: TargetDescription
                    ),
                    commonCoreStandardsDescription: StandardsDescription
                ),
                depthOfKnowledge: DOK,
                educationalDifficulty: EducationalDifficulty,
                evidenceStatement: EvidenceStatement,
                associatedStimulus: 1,
                rubrics: Rubrics,
                grade: GradeLevels.Grade6,
                isPerformanceItem: true,
                subject: MathSubject,
                fieldTestUse: TestUse);
            PerformanceDigest2 = SampleItem.Create(
                itemKey: ItemKey2,
                bankKey: BankKey2,
                associatedStimulus: 1,
                grade: GradeLevels.Grade6,
                isPerformanceItem: true,
                subject: MathSubject,
                fieldTestUse: TestUse);
            AllItemCards = ImmutableArray.Create(PerformanceCard1, PerformanceCard2);
            AllSampleItems = ImmutableArray.Create(PerformanceDigest1, PerformanceDigest2);
        }

        private Rubric BuildRubric()
        {
            var rubricEntry = new RubricEntry
            {
                Scorepoint = "0",
                Name = "TestName",
                Value = "TestValue"
            };

            var sampleResponces = new List<SampleResponse>()
            {
                new SampleResponse()
                {
                    Purpose = "TestPurpose",
                    ScorePoint = "0",
                    Name = "TestName",
                    SampleContent = "TestSampleContent"
                },
                new SampleResponse()
                {
                    Purpose = "TestPurpose1",
                    ScorePoint = "1",
                    Name = "TestName1",
                    SampleContent = "TestSampleContent1"
                }
            };

            var rubricSample = new RubricSample
            {
                MaxValue = "MaxVal",
                MinValue = "MinVal",
                SampleResponses = sampleResponces
            };

            var entries = ImmutableArray.Create(rubricEntry);
            var samples = ImmutableArray.Create(rubricSample);
            var rubric = new Rubric("ENU", entries, samples);
            return rubric;
        }

        [Fact]
        public void TestFromSampleItem()
        {
            var atiVM = AboutThisItemViewModelTranslations.FromSampleItem(
                sampleItem: PerformanceDigest1,
                itemCards: AllItemCards,
                allSampleItems: AllSampleItems);

            Assert.NotNull(atiVM);
            Assert.Equal(StandardsDescription, atiVM.CommonCoreStandardsDescription);
            Assert.Equal(Rubrics, atiVM.Rubrics);
            Assert.Equal(PerformanceCard1, atiVM.ItemCardViewModel);
            Assert.Equal(TargetDescription, atiVM.TargetDescription);
            Assert.Equal(DOK, atiVM.DepthOfKnowledge);
            Assert.Equal(EducationalDifficulty, atiVM.EducationalDifficulty);
            Assert.Equal(EvidenceStatement, atiVM.EvidenceStatement);
        }

        [Fact]
        public void TestGetAssociatedItems()
        {
            string associatedItems = AboutThisItemViewModelTranslations.GetAssociatedItems(
                item: PerformanceDigest1,
                allSampleItems: AllSampleItems);

            Assert.Equal("1-1,2-2", associatedItems);
        }
    }
}
