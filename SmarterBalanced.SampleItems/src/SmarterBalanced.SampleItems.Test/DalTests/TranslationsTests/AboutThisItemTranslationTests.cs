using SmarterBalanced.SampleItems.Dal.Providers.Models;
using SmarterBalanced.SampleItems.Dal.Translations;
using SmarterBalanced.SampleItems.Dal.Xml.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Xunit;
using System.Linq;
using SmarterBalanced.SampleItems.Dal.Exceptions;

namespace SmarterBalanced.SampleItems.Test.DalTests.TranslationsTests
{
    public class AboutThisItemTranslationTests
    {
        string TargetDescription, DOK, StandardsDescription, EducationalDifficulty, EvidenceStatement;
        ImmutableArray<ItemCardViewModel> AllItemCards;
        ImmutableArray<SampleItem> AllSampleItems;
        ImmutableArray<Rubric> Rubrics;
        Subject MathSubject;
        FieldTestUse TestUse;
        int BankKey = 154;
        List<int> AssoicatedItemKeys = new List<int> { 1, 2, 3, 4, 5 };
        List<int> AllItemKeys = new List<int> { 1, 2, 3, 4, 5, 100, 200, 300, 400, 500 };


        public AboutThisItemTranslationTests()
        {

            TargetDescription = "Target Description";
            StandardsDescription = "Standards Description";
            EducationalDifficulty = "Educational Difficulty";
            EvidenceStatement = "Evidence Statement";
            DOK = "1";

            Rubrics = ImmutableArray.Create(BuildRubric());
            TestUse = new FieldTestUse()
            {
                Code = "MATH",
                Section = "hi",
                QuestionNumber = 2
            };

            MathSubject = Subject.Create(
                code: "MATH",
                label: "Mathematics",
                shortLabel: "Math",
                claims: ImmutableArray<Claim>.Empty,
                interactionTypeCodes: ImmutableArray<string>.Empty);

            List<SampleItem> sampleItems = new List<SampleItem>();
            List<ItemCardViewModel> itemCards = new List<ItemCardViewModel>();

            foreach (int i in AllItemKeys)
            {
                bool isPerformance = AssoicatedItemKeys.Contains(i);
                var item = SampleItem.Create(
                itemKey: i,
                bankKey: BankKey,
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
                isPerformanceItem: isPerformance,
                subject: MathSubject,
                fieldTestUse: TestUse);

                sampleItems.Add(item);
                itemCards.Add(item.ToItemCardViewModel());
            }


            AllItemCards = itemCards.ToImmutableArray();
            AllSampleItems = sampleItems.ToImmutableArray();
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

        #region FromSampleItem
        [Fact]
        public void TestFromSampleItem()
        {
            foreach (SampleItem si in AllSampleItems)
            {
                var itemCard = AllItemCards.FirstOrDefault(i => i.ItemKey == si.ItemKey && i.BankKey == si.BankKey);
                var aboutThisItemVM = AboutThisItemViewModelTranslations.FromSampleItem(
                    sampleItem: si,
                    itemCards: AllItemCards,
                    allSampleItems: AllSampleItems);

                Assert.NotNull(aboutThisItemVM);
                Assert.NotNull(itemCard);

                Assert.Equal(StandardsDescription, aboutThisItemVM.CommonCoreStandardsDescription);
                Assert.Equal(Rubrics, aboutThisItemVM.Rubrics);
                Assert.Equal(itemCard, aboutThisItemVM.ItemCardViewModel);
                Assert.Equal(TargetDescription, aboutThisItemVM.TargetDescription);
                Assert.Equal(DOK, aboutThisItemVM.DepthOfKnowledge);
                Assert.Equal(EducationalDifficulty, aboutThisItemVM.EducationalDifficulty);
                Assert.Equal(EvidenceStatement, aboutThisItemVM.EvidenceStatement);
            }

        }

        [Fact]
        public void TestFromSampleItemNoItemCards()
        {
            var sampleItem = AllSampleItems.FirstOrDefault();
            ImmutableArray<ItemCardViewModel> itemCards = new ImmutableArray<ItemCardViewModel>();
            Assert.Throws<SampleItemsContextException>(() => AboutThisItemViewModelTranslations.FromSampleItem(
                         sampleItem: sampleItem,
                         itemCards: itemCards,
                         allSampleItems: AllSampleItems));
        }

        [Fact]
        public void TestFromSampleItemNoSampleItems()
        {
            var sampleItem = AllSampleItems.FirstOrDefault();
            ImmutableArray<SampleItem> sampleItems = new ImmutableArray<SampleItem>();
            Assert.Throws<SampleItemsContextException>(() => AboutThisItemViewModelTranslations.FromSampleItem(
                         sampleItem: sampleItem,
                         itemCards: AllItemCards,
                         allSampleItems: sampleItems));
        }

        [Fact]
        public void TestFromSampleItemNoSampleItem()
        {
            ImmutableArray<ItemCardViewModel> itemCards = new ImmutableArray<ItemCardViewModel>();
            var aboutThisItemVM = AboutThisItemViewModelTranslations.FromSampleItem(
                         sampleItem: null,
                         itemCards: itemCards,
                         allSampleItems: AllSampleItems);

            Assert.Null(aboutThisItemVM);
        }

        [Fact]
        public void TestFromSampleAssociatedItem()
        {
            var sampleItem = AllSampleItems.FirstOrDefault(si => si.ItemKey == AssoicatedItemKeys.FirstOrDefault() && si.BankKey == BankKey);

            var aboutThisItemVM = AboutThisItemViewModelTranslations.FromSampleItem(
                         sampleItem: sampleItem,
                         itemCards: AllItemCards,
                         allSampleItems: AllSampleItems);

            Assert.NotNull(aboutThisItemVM);
            Assert.NotNull(aboutThisItemVM.AssociatedItems);
        }

        [Fact]
        public void TestFromSampleNonAssociatedItem()
        {
            var sampleItem = AllSampleItems.FirstOrDefault(si => AssoicatedItemKeys.Any(i => si.ItemKey != i) && si.BankKey == BankKey);
            var aboutThisItemVM = AboutThisItemViewModelTranslations.FromSampleItem(
                         sampleItem: sampleItem,
                         itemCards: AllItemCards,
                         allSampleItems: AllSampleItems);

            Assert.NotNull(aboutThisItemVM);
            Assert.NotNull(aboutThisItemVM.AssociatedItems);
        }


        #endregion

        #region GetAssociatedItems
        [Fact]
        public void TestGetAssociatedItemsMultiple()
        {
            var associatedSampleItems = AllSampleItems.Where(si => AssoicatedItemKeys.Contains(si.ItemKey) && si.BankKey == BankKey);
            var associatedItemNames = associatedSampleItems.Select(asi => asi.ToString());
            var nonAssociatedItemNames = AllSampleItems
                .Where(si => !associatedSampleItems.Contains(si))
                .Select(asi => asi.ToString());
            foreach (var sampleItem in associatedSampleItems)
            {
                string testAssociatedItems = AboutThisItemViewModelTranslations.GetAssociatedItems(
                    item: sampleItem,
                    allSampleItems: AllSampleItems);

                foreach (string itemName in associatedItemNames)
                {
                    Assert.Contains(itemName, testAssociatedItems);
                }

                foreach (string itemName in nonAssociatedItemNames)
                {
                    Assert.DoesNotContain(itemName, testAssociatedItems);
                }
            }
        }


        [Fact]
        public void TestGetAssociatedItemsSingle()
        {
            var sampleItem = AllSampleItems.FirstOrDefault(si => !AssoicatedItemKeys.Contains(si.ItemKey) && si.BankKey == BankKey);
            string associatedItems = AboutThisItemViewModelTranslations.GetAssociatedItems(
                item: sampleItem,
                allSampleItems: AllSampleItems);

            Assert.Equal(sampleItem.ToString(), associatedItems);
        }



        [Fact]
        public void TestGetAssociatedItemsNull()
        {
            string associatedItems = AboutThisItemViewModelTranslations.GetAssociatedItems(
                item: null,
                allSampleItems: AllSampleItems);

            Assert.Null(associatedItems);
        }



        [Fact]
        public void TestGetAssociatedItemsEmpty()
        {
            var sampleItem = AllSampleItems.FirstOrDefault(si => AssoicatedItemKeys.Contains(si.ItemKey) && si.BankKey == BankKey);
            var emptySampleItems = new ImmutableArray<SampleItem>();
            Assert.Throws<SampleItemsContextException>(() =>
            {
                AboutThisItemViewModelTranslations.GetAssociatedItems(
                    item: sampleItem,
                    allSampleItems: emptySampleItems);
            });
        }

        #endregion

    }
}
