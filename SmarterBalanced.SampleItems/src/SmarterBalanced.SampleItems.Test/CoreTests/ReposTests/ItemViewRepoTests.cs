﻿using Microsoft.Extensions.Logging;
using Moq;
using SmarterBalanced.SampleItems.Core.Repos;
using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using SmarterBalanced.SampleItems.Dal.Providers;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;
namespace SmarterBalanced.SampleItems.Test.CoreTests.ReposTests
{
    public class ItemViewRepoTests
    {
        ItemDigest MathDigest, ElaDigest, DuplicateDigest;
        Subject Math, Ela, NotASubject;
        Claim Claim1, Claim2;
        ImmutableArray<ItemDigest> ItemDigests;
        ItemViewRepo ItemViewRepo;
        SampleItemsContext Context;
        int GoodItemKey;
        int BadItemKey;
        int GoodBankKey;
        int BadBankKey;
        int DuplicateItemKey, DuplicateBankKey;
        ItemCardViewModel MathCard, ElaCard, DuplicateCard;
       

        public ItemViewRepoTests()
        {
            GoodBankKey = 1;
            GoodItemKey = 2;
            BadBankKey = 3;
            BadItemKey = 9;
            GoodItemKey = 4;
            DuplicateBankKey = 5;
            DuplicateItemKey = 6;
            MathCard = ItemCardViewModel.Create(bankKey: GoodBankKey, itemKey: GoodItemKey);
            ElaCard = ItemCardViewModel.Create(bankKey: BadBankKey, itemKey: BadItemKey);
            DuplicateCard = ItemCardViewModel.Create(bankKey: DuplicateBankKey, itemKey: DuplicateItemKey);
            MathDigest = new ItemDigest() { BankKey = GoodBankKey, ItemKey = GoodItemKey };
            ElaDigest = new ItemDigest() { BankKey = BadBankKey, ItemKey = BadItemKey };
            DuplicateDigest = new ItemDigest() { BankKey = DuplicateBankKey, ItemKey = DuplicateItemKey };
            ItemDigests = ImmutableArray.Create(MathDigest, ElaDigest, DuplicateDigest, DuplicateDigest, DuplicateDigest);
            var itemCards = ImmutableArray.Create(MathCard, ElaCard, DuplicateCard, DuplicateCard, DuplicateCard);

            Math = new Subject("Math", "", "", new ImmutableArray<Claim>() { }, new ImmutableArray<string>() { });
            Ela = new Subject("Ela", "", "", new ImmutableArray<Claim>() { }, new ImmutableArray<string>() { });
            NotASubject= new Subject("NotASubject", "", "", new ImmutableArray<Claim>() { }, new ImmutableArray<string>() { });
            Claim1 = new Claim("1", "", "");
            Claim2 = new Claim("2", "", "");

            //generated item cards for more like this tests
            itemCards = itemCards.AddRange(MoreItemCards());
            var settings = new AppSettings() { SettingsConfig = new SettingsConfig() { NumMoreLikeThisItems = 3 } };

            Context = SampleItemsContext.Create(itemDigests: ItemDigests, itemCards: itemCards, appSettings: settings);

            var loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger>();
            loggerFactory.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
            ItemViewRepo = new ItemViewRepo(Context, loggerFactory.Object);
        }

        private ImmutableArray<ItemCardViewModel> MoreItemCards()
        {
            var subjectCodes = new string[] { "Math", "Ela", "Science" };
            var claimCodes = new string[] { "1", "2", "3" };
            var gradeValues = GradeLevelsUtils.singleGrades.ToList();
            var moreCards = new List<ItemCardViewModel>();
            for (int i = 10; i < 60; i++)
            {
                moreCards.Add(ItemCardViewModel.Create(
                    bankKey: 10, 
                    itemKey: i, 
                    grade: gradeValues[i % gradeValues.Count],
                    subjectCode: subjectCodes[i%subjectCodes.Length],
                    claimCode: claimCodes[((i+60)/7)%claimCodes.Length]));
            }
            return moreCards.ToImmutableArray();
        }

        #region GetItemDigest/Card

        [Fact]
        public void TestGetItemDigest()
        {
            var result = ItemViewRepo.GetItemDigest(GoodBankKey, GoodItemKey);
            var resultCheck = Context.ItemDigests.FirstOrDefault(i => i.ItemKey == GoodItemKey && i.BankKey == GoodBankKey);

            Assert.NotNull(result);
            Assert.Equal(result, resultCheck);
        }

        [Fact]
        public void TestGetItemDigestDuplicate()
        {
            Assert.Throws<InvalidOperationException>(()=>ItemViewRepo.GetItemDigest(DuplicateBankKey, DuplicateItemKey));
        }

        [Fact]
        public void TestGetItemCard()
        {
            var result = ItemViewRepo.GetItemCardViewModel(BadBankKey, BadItemKey);
            var resultCheck=Context.ItemCards.FirstOrDefault(i => i.ItemKey == BadItemKey && i.BankKey == BadBankKey);

            Assert.NotNull(result);
            Assert.Equal(result, resultCheck);
        }

        [Fact]
        public void TestGetItemCardDuplicate()
        {
            Assert.Throws<InvalidOperationException>(() => ItemViewRepo.GetItemCardViewModel(DuplicateBankKey, DuplicateItemKey));
        }

        #endregion

        //TODO:
        //- add tests for if itemcards and itemdigests have more than one item matching?

        #region MoreLikeThis

        [Fact]
        public void TestMoreLikeThisHappyCase()
        {
            var itemDigest = new ItemDigest() { Subject = Math, Claim = Claim1, Grade = GradeLevels.Grade6 };
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
            var itemDigest = new ItemDigest() { Claim = Claim1, Subject = Ela };
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
            var itemDigest = new ItemDigest() { Claim = Claim1, Subject = NotASubject, Grade = GradeLevels.Grade4 };
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
        #endregion
    }
}
