using Microsoft.Extensions.Logging;
using Moq;
using SmarterBalanced.SampleItems.Core.Repos;
using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using SmarterBalanced.SampleItems.Dal.Providers;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using SmarterBalanced.SampleItems.Dal.Xml.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;
namespace SmarterBalanced.SampleItems.Test.CoreTests.ReposTests
{

    public class ItemViewRepoTests
    {
        SampleItem MathDigest, ElaDigest, DuplicateDigest, PerformanceDigest, PerformanceDigestDuplicate, BrailleItem, BrailleItemDuplicate, BrailleItemReplace;

        Subject Math, Ela, NotASubject;
        Claim Claim1, Claim2;
        ImmutableArray<SampleItem> SampleItems;
        ItemViewRepo ItemViewRepo;
        SampleItemsContext Context;
        FieldTestUse fieldTestUseVar;
        int GoodItemKey;
        int BadItemKey;
        int GoodBankKey;
        int BadBankKey;
        int DuplicateItemKey, DuplicateBankKey;
        ItemCardViewModel MathCard, ElaCard, DuplicateCard, PerformanceCard;
        Rubric TestRubric;

        public ItemViewRepoTests()
        {
            GoodBankKey = 1;
            BadBankKey = 3;
            BadItemKey = 9;
            GoodItemKey = 4;
            DuplicateBankKey = 5;
            DuplicateItemKey = 6;
            MathCard = ItemCardViewModel.Create(bankKey: GoodBankKey, itemKey: GoodItemKey);
            ElaCard = ItemCardViewModel.Create(bankKey: BadBankKey, itemKey: BadItemKey);
            DuplicateCard = ItemCardViewModel.Create(bankKey: DuplicateBankKey, itemKey: DuplicateItemKey);
            MathDigest = SampleItem.Create(bankKey: GoodBankKey, itemKey: GoodItemKey);
            ElaDigest = SampleItem.Create(bankKey: BadBankKey, itemKey: BadItemKey);
            TestRubric = BuildRubric();
            fieldTestUseVar = new FieldTestUse();
            fieldTestUseVar.Code = "ELA";
            fieldTestUseVar.QuestionNumber = 1;

            DuplicateDigest = SampleItem.Create(bankKey: GoodBankKey, itemKey: DuplicateItemKey);
            var duplicateDigest2 = SampleItem.Create(bankKey: GoodBankKey, itemKey: DuplicateItemKey);

            PerformanceCard = ItemCardViewModel.Create(bankKey: GoodBankKey, itemKey: 209);
            PerformanceDigest = SampleItem.Create(bankKey: GoodBankKey, itemKey: 209, isPerformanceItem: true, associatedStimulus: 1, fieldTestUse: fieldTestUseVar);
            PerformanceDigestDuplicate = SampleItem.Create(bankKey: DuplicateBankKey, itemKey: 210, isPerformanceItem: true, associatedStimulus: 1, fieldTestUse: fieldTestUseVar);

            BrailleItem = SampleItem.Create(bankKey: GoodBankKey, itemKey: 211, isPerformanceItem: true, associatedStimulus: 1,
                fieldTestUse: fieldTestUseVar,
                brailleOnlyItem: false,
                brailleItemCodes: ImmutableArray.Create("123"),
                braillePassageCodes: ImmutableArray.Create("123"));

            BrailleItemDuplicate = SampleItem.Create(bankKey: DuplicateBankKey, itemKey: 212, isPerformanceItem: true, associatedStimulus: 1,
                fieldTestUse: fieldTestUseVar,
                brailleOnlyItem: false,
                brailleItemCodes: ImmutableArray.Create("123"),
                braillePassageCodes: ImmutableArray.Create("123"));

            BrailleItemReplace = SampleItem.Create(bankKey: DuplicateBankKey, itemKey: 213, isPerformanceItem: true, associatedStimulus: 2,
                fieldTestUse: fieldTestUseVar,
                brailleOnlyItem: true,
                brailleItemCodes: ImmutableArray.Create("123"),
                braillePassageCodes: ImmutableArray.Create("123"),
                copiedFromItem: 211);

            SampleItems = ImmutableArray.Create(MathDigest, ElaDigest, DuplicateDigest, DuplicateDigest, DuplicateDigest, PerformanceDigest, PerformanceDigestDuplicate, BrailleItem, BrailleItemDuplicate, BrailleItemReplace);
            var itemCards = ImmutableArray.Create(MathCard, ElaCard, DuplicateCard, DuplicateCard, DuplicateCard);

            Math = new Subject("Math", "", "", new ImmutableArray<Claim>() { }, new ImmutableArray<string>() { });
            Ela = new Subject("ELA", "", "", new ImmutableArray<Claim>() { }, new ImmutableArray<string>() { });
            NotASubject = new Subject("NotASubject", "", "", new ImmutableArray<Claim>() { }, new ImmutableArray<string>() { });
            Claim1 = new Claim("1", "", "", ImmutableArray.Create<Target>());
            Claim2 = new Claim("2", "", "", ImmutableArray.Create<Target>());

            //generated item cards for more like this tests
            itemCards = itemCards.AddRange(MoreItemCards());
            var settings = new AppSettings() { SettingsConfig = new SettingsConfig() { NumMoreLikeThisItems = 3 } };

            var aboutGoodItem = AboutThisItemViewModel.Create(
                rubrics: ImmutableArray.Create(TestRubric),
                itemCard: MathCard,
                depthOfKnowledge: "TestDepth");
            var aboutItems = ImmutableArray.Create(aboutGoodItem);

            Context = SampleItemsContext.Create(sampleItems: SampleItems, itemCards: itemCards, appSettings: settings, aboutAllItems: aboutItems);

            var loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger>();
            loggerFactory.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
            ItemViewRepo = new ItemViewRepo(Context, loggerFactory.Object);
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

        private ImmutableArray<ItemCardViewModel> MoreItemCards()
        {
            var subjectCodes = new string[] { "Math", "ELA", "Science" };
            var claimCodes = new string[] { "1", "2", "3" };
            var gradeValues = GradeLevelsUtils.singleGrades.ToList();
            var moreCards = new List<ItemCardViewModel>();
            for (int i = 10; i < 60; i++)
            {
                moreCards.Add(ItemCardViewModel.Create(
                    bankKey: 10,
                    itemKey: i,
                    grade: gradeValues[i % gradeValues.Count],
                    subjectCode: subjectCodes[i % subjectCodes.Length],
                    claimCode: claimCodes[((i + 60) / 7) % claimCodes.Length]));
            }
            return moreCards.ToImmutableArray();
        }

        #region GetItemDigest/Card

        [Fact]
        public void TestGetItemDigest()
        {
            var result = ItemViewRepo.GetSampleItem(GoodBankKey, GoodItemKey);
            var resultCheck = Context.SampleItems.FirstOrDefault(i => i.ItemKey == GoodItemKey && i.BankKey == GoodBankKey);

            Assert.NotNull(result);
            Assert.Equal(result, resultCheck);
        }

        [Fact]
        public void TestGetItemDigestDuplicate()
        {
            var result = ItemViewRepo.GetSampleItem(DuplicateBankKey, DuplicateItemKey);
            Assert.Null(result);
        }

        [Fact]
        public void TestGetItemCard()
        {
            var result = ItemViewRepo.GetItemCardViewModel(BadBankKey, BadItemKey);
            var resultCheck = Context.ItemCards.FirstOrDefault(i => i.ItemKey == BadItemKey && i.BankKey == BadBankKey);

            Assert.NotNull(result);
            Assert.Equal(result, resultCheck);
        }

        [Fact]
        public void TestGetItemUrl()
        {
            var result = ItemViewRepo.GetSampleItem(GoodBankKey, GoodItemKey);
            var url = ItemViewRepo.GetItemNames(result);

            Assert.NotNull(url);
            Assert.Equal("1-4", url);
        }

        [Fact]
        public void TestGetItemUrlNull()
        {
            var url = ItemViewRepo.GetItemNames(null);
            Assert.Equal(string.Empty, url);
        }

        [Fact]
        public void TestGetItemCardDuplicate()
        {
            Assert.Throws<InvalidOperationException>(() => ItemViewRepo.GetItemCardViewModel(DuplicateBankKey, DuplicateItemKey));
        }

        #endregion

        #region MoreLikeThis

        [Fact]
        public void TestMoreLikeThisHappyCase()
        {
            var itemDigest = SampleItem.Create(subject: Math, claim: Claim1, grade: GradeLevels.Grade6);
            var more = ItemViewRepo.GetMoreLikeThis(itemDigest);

            Assert.Equal(3, more.GradeAboveItems.ItemCards.Count());
            Assert.Equal(3, more.GradeBelowItems.ItemCards.Count());
            Assert.Equal(3, more.SameGradeItems.ItemCards.Count());

            foreach (var card in more.GradeAboveItems.ItemCards)
            {
                Assert.Equal(GradeLevels.Grade7, card.Grade);
            }
            foreach (var card in more.SameGradeItems.ItemCards)
            {
                Assert.Equal(GradeLevels.Grade6, card.Grade);
            }
            foreach (var card in more.GradeBelowItems.ItemCards)
            {
                Assert.Equal(GradeLevels.Grade5, card.Grade);
            }

        }

        [Fact]
        public void TestMoreNAGrade()
        {
            var itemDigest = SampleItem.Create(claim: Claim1, subject: Ela);
            var more = ItemViewRepo.GetMoreLikeThis(itemDigest);

            Assert.Equal(3, more.GradeAboveItems.ItemCards.Count());
            Assert.Equal(3, more.GradeBelowItems.ItemCards.Count());
            Assert.Equal(3, more.SameGradeItems.ItemCards.Count());

            foreach (var card in more.GradeAboveItems.ItemCards)
            {
                Assert.Equal(GradeLevels.NA, card.Grade);
            }
            foreach (var card in more.SameGradeItems.ItemCards)
            {
                Assert.Equal(GradeLevels.NA, card.Grade);
            }
            foreach (var card in more.GradeBelowItems.ItemCards)
            {
                Assert.Equal(GradeLevels.NA, card.Grade);
            }
        }

        [Fact]
        public void TestMoreUnknownSubject()
        {
            var itemDigest = SampleItem.Create(claim: Claim1, subject: NotASubject, grade: GradeLevels.Grade4);
            var more = ItemViewRepo.GetMoreLikeThis(itemDigest);

            Assert.Equal(3, more.GradeAboveItems.ItemCards.Count());
            Assert.Equal(3, more.GradeBelowItems.ItemCards.Count());
            Assert.Equal(3, more.SameGradeItems.ItemCards.Count());

            var countAbove = more.GradeAboveItems.ItemCards.Count(c => c.ClaimCode == Claim1.Code);
            var expectedAbove = Context.ItemCards.Count(c => c.ClaimCode == Claim1.Code && c.Grade == GradeLevels.Grade5);

            Assert.Equal(System.Math.Min(expectedAbove, 3), countAbove);

            var countBelow = more.GradeBelowItems.ItemCards.Count(c => c.ClaimCode == Claim1.Code);
            var expectedBelow = Context.ItemCards.Count(c => c.ClaimCode == Claim1.Code && c.Grade == GradeLevels.Grade3);

            Assert.Equal(System.Math.Min(expectedBelow, 3), countBelow);

            var countSame = more.SameGradeItems.ItemCards.Count(c => c.ClaimCode == Claim1.Code);
            var expectedSame = Context.ItemCards.Count(c => c.ClaimCode == Claim1.Code && c.Grade == GradeLevels.Grade4);

            Assert.Equal(System.Math.Min(expectedSame, 3), countSame);

        }

        [Fact]
        public void TestGetAboutThisItemViewModelGoodItem()
        {
            var aboutThisItemViewModel = ItemViewRepo.GetAboutThisItemViewModel(MathDigest);

            Assert.NotNull(aboutThisItemViewModel);
            Assert.Equal(aboutThisItemViewModel.Rubrics.Length, 1);
            Assert.Equal(aboutThisItemViewModel.Rubrics[0], TestRubric);
            Assert.Equal(aboutThisItemViewModel.DepthOfKnowledge, "TestDepth");
        }

        [Fact]
        public void TestGetAboutThisItemViewModelBadItem()
        {
            var aboutThisItemViewModel = ItemViewRepo.GetAboutThisItemViewModel(null);

            Assert.Null(aboutThisItemViewModel);
        }

        #endregion

        #region BrailleItems

        [Fact]
        public void TestBadGetItemNames()
        {
            var item = ItemViewRepo.GetItemNames(null);

            Assert.Equal(item, string.Empty);
        }

        [Fact]
        public void TestGetBrailleItemNames()
        {
            var item = ItemViewRepo.GetBrailleItemNames(BrailleItem);
            var associatedItems = ItemViewRepo.GetAssociatedBrailleItems(BrailleItem).Select(i => i.ToString());

            Assert.Equal(associatedItems.Count(), 4);
            Assert.True(item.Contains(associatedItems.ElementAt(0)));
            Assert.True(item.Contains(associatedItems.ElementAt(1)));
            Assert.True(item.Contains(associatedItems.ElementAt(2)));
            Assert.True(item.Contains(associatedItems.ElementAt(3)));
            Assert.False(item.Contains("1-211"));
        }
        #endregion
    }
}
