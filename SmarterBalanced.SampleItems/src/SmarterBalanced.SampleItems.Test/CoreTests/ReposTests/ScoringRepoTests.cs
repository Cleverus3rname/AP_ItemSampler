using Microsoft.Extensions.Logging;
using Moq;
using SmarterBalanced.SampleItems.Core.Repos;
using SmarterBalanced.SampleItems.Core.ScoreGuide;
using SmarterBalanced.SampleItems.Core.ScoreGuide.Models;
using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using SmarterBalanced.SampleItems.Dal.Providers;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Xunit;

namespace SmarterBalanced.SampleItems.Test.CoreTests.ReposTests
{
    public class ScoringRepoTests
    {
        ScoringRepo ScoringRepo;
        SampleItemsContext Context;
        ScoreSearchParams scoringSearchParam;
        int GoodItemKey;
        int BadItemKey;
        int GoodBankKey;
        int BadBankKey;
        int DuplicateItemKey, DuplicateBankKey;
        ItemCardViewModel MathCard, ElaCard, DuplicateCard;
        Subject Math;
        SampleItem MathDigest, ElaDigest;
        ImmutableArray<SampleItem> SampleItems;
        ImmutableArray<ItemCardViewModel> ItemCards;

        public ScoringRepoTests()
        {
            GoodBankKey = 1;
            BadBankKey = 3;
            BadItemKey = 9;
            GoodItemKey = 4;
            DuplicateBankKey = 5;
            DuplicateItemKey = 6;
            MathCard = ItemCardViewModel.Create(bankKey: GoodBankKey, itemKey: GoodItemKey, grade:GradeLevels.Grade10);
            ElaCard = ItemCardViewModel.Create(bankKey: BadBankKey, itemKey: BadItemKey, grade: GradeLevels.Elementary);
            DuplicateCard = ItemCardViewModel.Create(bankKey: DuplicateBankKey, itemKey: DuplicateItemKey, grade: GradeLevels.Grade5, isPerformanceItem: true);
            MathDigest = SampleItem.Create(bankKey: GoodBankKey, itemKey: GoodItemKey);
            ElaDigest = SampleItem.Create(bankKey: BadBankKey, itemKey: BadItemKey);

            SampleItems = ImmutableArray.Create(MathDigest, ElaDigest);
            ItemCards = ImmutableArray.Create(MathCard, ElaCard, DuplicateCard, DuplicateCard, DuplicateCard);
            var settings = new AppSettings();
            scoringSearchParam = new ScoreSearchParams(GradeLevels.Elementary, new string[] { "" }, new string[] { "" }, false);
            var loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger>();
            loggerFactory.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
            var interaction = InteractionType.Create();
            var interactionTypes = ImmutableArray.Create(interaction);
            var gradeLevels = ImmutableArray.Create(GradeLevels.All);
            Math = Subject.Create(code: "Math");
            var subjects = ImmutableArray.Create(Math);

            Context = SampleItemsContext.Create(sampleItems: SampleItems, itemCards: ItemCards, appSettings: settings, interactionTypes: interactionTypes, subjects: subjects);
            ScoringRepo = new ScoringRepo(Context, loggerFactory.Object);
        }

        [Fact]
        public void TestGetScoringGuideViewModel()
        {
            var scoringVm = ScoringRepo.GetScoringGuideViewModel();

            Assert.NotNull(scoringVm);
            Assert.Equal(scoringVm.InteractionTypes, Context.InteractionTypes);
            Assert.Equal(scoringVm.Subjects, Context.Subjects);
        }

        [Fact]
        public void TestGetItemCards()
        {
            var scoreSearchResult = ScoringRepo.GetItemCards(GradeLevels.Elementary, new string[] { "" }, new string[] { "" }, false);
            var itemCardsResult = ScoringRepo.GetItemCards(scoringSearchParam);

            Assert.NotNull(itemCardsResult);
            Assert.Equal(itemCardsResult, scoreSearchResult);
        }

        [Fact]
        public void TestGetItemCardsNull()
        {
            var itemCardsResult = ScoringRepo.GetItemCards(null);

            Assert.NotNull(itemCardsResult);
            Assert.Equal(itemCardsResult.Count, ItemCards.Length);
        }

        [Fact]
        public void TestGetItemCardsGrades()
        {
            var gradeParams = new ScoreSearchParams(GradeLevels.Grade10, new string[] { "" }, new string[] { "" }, false);
            var itemCardsResult = ScoringRepo.GetItemCards(gradeParams);

            Assert.NotNull(itemCardsResult);
            Assert.Equal(itemCardsResult.Count, 1);
        }

        [Fact]
        public void TestGetItemCardsTechType()
        {
            var techTypeParams = new ScoreSearchParams(GradeLevels.Elementary, new string[] { "" }, new string[] { "pt" }, false);
            var itemCardsResult = ScoringRepo.GetItemCards(techTypeParams);

            Assert.NotNull(itemCardsResult);
            Assert.Equal(itemCardsResult.Count, 3);
        }
    }
}
