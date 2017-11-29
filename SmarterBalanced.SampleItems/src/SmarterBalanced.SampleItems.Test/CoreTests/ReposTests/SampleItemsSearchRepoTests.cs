using Microsoft.Extensions.Logging;
using Moq;
using SmarterBalanced.SampleItems.Core.Repos;
using SmarterBalanced.SampleItems.Core.Repos.Models;
using SmarterBalanced.SampleItems.Dal.Providers;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace SmarterBalanced.SampleItems.Test.CoreTests.ReposTests
{
    public class SampleItemsSearchRepoTests
    {
        SampleItemsSearchRepo SampleItemsSearchRepo;
        int GoodItemKey;
        int GoodBankKey;
        int BadItemKey;
        int BadBankKey;
        SampleItemsContext Context;
        Claim Claim1, Claim2;
        Subject Math, Ela;
        InteractionType ItEla, ItMath;
        public SampleItemsSearchRepoTests()
        {
            GoodItemKey = 2343;
            GoodBankKey = 8398;
            BadItemKey = 9234;
            BadBankKey = 1123;
            Claim1 = Claim.Create(code: "claim1", claimNumber: "1");
            Claim2 = Claim.Create(code: "claim2", claimNumber: "1");

            ItMath = new InteractionType("2", "Math Itype", "", 2);
            ItEla = new InteractionType("1", "Ela Itype", "", 1);
            Math = Subject.Create("Math", "Mathematics", "Math", ImmutableArray.Create(Claim2), ImmutableArray.Create(ItMath.Code));
            Ela = Subject.Create("ELA", "English", "ELA", ImmutableArray.Create(Claim1), ImmutableArray.Create(ItEla.Code));
            var interactionTypes = ImmutableArray.Create(ItEla, ItMath);
            var subjects = ImmutableArray.Create(Ela, Math);
            var itemCards = ImmutableArray.Create(
                        ItemCardViewModel.Create(bankKey: GoodBankKey, itemKey: GoodItemKey, grade: GradeLevels.Grade6,
                                                 subjectCode: Math.Code, interactionTypeCode: ItMath.Code, claimCode: Claim1.Code),
                        ItemCardViewModel.Create(bankKey: GoodBankKey, itemKey: BadItemKey, grade: GradeLevels.High,
                                                 subjectCode: Math.Code, interactionTypeCode: ItMath.Code, claimCode: Claim2.Code),
                        ItemCardViewModel.Create(bankKey: BadBankKey, itemKey: GoodItemKey, grade: GradeLevels.Grade9,
                                                 subjectCode: Ela.Code, interactionTypeCode: ItEla.Code, claimCode: Claim1.Code),
                        ItemCardViewModel.Create(bankKey: BadBankKey, itemKey: BadItemKey, grade: GradeLevels.Grade4,
                                                 subjectCode: Ela.Code, interactionTypeCode: ItEla.Code, claimCode: Claim2.Code),
                        ItemCardViewModel.Create(bankKey: 1, itemKey: 2, grade: GradeLevels.Grade12)
                );
            Context = SampleItemsContext.Create(itemCards: itemCards, subjects: subjects, interactionTypes: interactionTypes);

            var loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger>();
            loggerFactory.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            SampleItemsSearchRepo = new SampleItemsSearchRepo(Context, loggerFactory.Object);
        }

        /// <summary>
        /// Asserts whether or not the card lists have exactly the same cards
        /// </summary>
        /// <param name="cards1">First list to compare</param>
        /// <param name="cards2">Second list to compare</param>
        /// <returns></returns>
        private void AssertItemCardListsEqual(IList<ItemCardViewModel> cards1, IList<ItemCardViewModel> cards2)
        {
            foreach (var card in cards1)
            {
                try { cards2.Remove(card); }
                catch { }
            }
            if (cards2.Any())
            {
                throw new System.Exception("Lists not equal");
            }
        }

        /// <summary>
        /// Orders cards by bank key, then by item key
        /// </summary>
        /// <param name="cardsList">List of cards to order</param>
        /// <returns></returns>
        private List<ItemCardViewModel> Sort(IList<ItemCardViewModel> cardsList)
        {
            return cardsList.OrderBy(c => c.BankKey).ThenBy(c => c.ItemKey).ToList();
        }

        #region GetItemCards
        [Fact]
        public void HappyCase()
        {
            Assert.Equal(Context.ItemCards.Count(), SampleItemsSearchRepo.GetItemCards().Count);
        }

        [Fact]
        public void TestSearchGradeLevel()
        {
            var parameters = new ItemsSearchParams(null, GradeLevels.High, null, new string[] { }, new string[] { }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            var cardsCheck = Context.ItemCards.Where(c => GradeLevelsUtils.Contains(GradeLevels.High, c.Grade)).ToList();

            Assert.NotNull(cards);
            Assert.Equal(cards.Count, cardsCheck.Count);
            Assert.Equal(Sort(cards), Sort(cardsCheck));
        }

        [Fact]
        public void TestSearchSubject()
        {
            var parameters = new ItemsSearchParams(null, GradeLevels.All, new List<string>() { Math.Code }, new string[] { }, new string[] { }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            var cardsCheck = Context.ItemCards.Where(c => c.SubjectCode == Math.Code).ToList();

            Assert.NotNull(cards);
            Assert.Equal(cards.Count, cardsCheck.Count);
            Assert.Equal(Sort(cards), Sort(cardsCheck));
        }

        [Fact]
        public void TestSearchClaim()
        {
            var parameters = new ItemsSearchParams(null, GradeLevels.All, new List<string>() { Math.Code, Ela.Code }, new string[] { }, new string[] { Claim1.Code }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            var cardsCheck = Context.ItemCards.Where(c => c.ClaimCode == Claim1.Code).ToList();

            Assert.NotNull(cards);
            Assert.Equal(cards.Count, cardsCheck.Count);
            Assert.Equal(Sort(cards), Sort(cardsCheck));
        }

        [Fact]
        public void TestSearchItemType()
        {
            var parameters = new ItemsSearchParams(null, GradeLevels.All, new List<string>() { Math.Code, Ela.Code }, new string[] { ItEla.Code }, new string[] { }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            var cardsCheck = Context.ItemCards.Where(c => c.InteractionTypeCode == ItEla.Code).ToList();

            Assert.NotNull(cards);
            Assert.Equal(cards.Count, cardsCheck.Count);
            Assert.Equal(Sort(cards), Sort(cardsCheck));
        }

        [Fact]
        public void TestNullItemId()
        {
            var parameters = new ItemsSearchParams(null, GradeLevels.All, null, new string[] { }, new string[] { }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            var cardsCheck = Context.ItemCards;

            Assert.NotNull(cards);
            Assert.Equal(cards.Count, cardsCheck.Count());
            Assert.Equal(Sort(cards), Sort(cardsCheck));
        }

        [Fact]
        public void TestNonIntegerItemId()
        {
            var parameters = new ItemsSearchParams("NOT A NUMBER!", GradeLevels.All, null, new string[] { }, new string[] { }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            var cardsCheck = Context.ItemCards;

            Assert.NotNull(cards);
            Assert.Equal(cards.Count, cardsCheck.Count());
            Assert.Equal(Sort(cards), Sort(cardsCheck));
        }

        [Fact]
        public void TestNullSearchParamObj()
        {
            var cards = SampleItemsSearchRepo.GetItemCards(null);

            Assert.NotNull(cards);
            Assert.Equal(cards.Count, Context.ItemCards.Count());
            Assert.Equal(Sort(cards), Sort(Context.ItemCards));
        }

        [Fact]
        public void TestGetItemCardsNull()
        {
            var parameters = new ItemsSearchParams(null, GradeLevels.All, null, null, null, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            var cardsCheck = Context.ItemCards;

            Assert.NotNull(cards);
            Assert.Equal(cards.Count, cardsCheck.Count());
            Assert.Equal(Sort(cards), Sort(cardsCheck));
        }

        [Fact]
        public void TestSearchTwoClaims()
        {
            var parameters = new ItemsSearchParams(null, GradeLevels.All, new List<string>() { Math.Code }, new string[] { }, new string[] { Claim1.Code, Claim2.Code }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            var cardsCheck = Context.ItemCards.Where(c => c.SubjectCode == Math.Code && (c.ClaimCode == Claim1.Code
                                                     || c.ClaimCode == Claim2.Code)).ToList();
            Assert.NotNull(cards);
            Assert.Equal(cards.Count, cardsCheck.Count);
            Assert.Equal(Sort(cards), Sort(cardsCheck));
        }

        [Fact]
        public void TestSearchTwoITypes()
        {
            var parameters = new ItemsSearchParams(null, GradeLevels.All, new List<string>() { Math.Code }, new string[] { ItEla.Code, ItMath.Code }, new string[] { }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            var cardsCheck = Context.ItemCards.Where(c => c.SubjectCode == Math.Code && (c.ClaimCode == Claim1.Code
                                                     || c.ClaimCode == Claim2.Code)).ToList();
            Assert.NotNull(cards);
            Assert.Equal(cards.Count, cardsCheck.Count);
            Assert.Equal(Sort(cards), Sort(cardsCheck));
        }

        [Fact]
        public void TestSearchTwoSubjects()
        {
            var parameters = new ItemsSearchParams(null, GradeLevels.All, new List<string>() { Math.Code, Ela.Code }, new string[] { }, new string[] { }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            var cardsCheck = Context.ItemCards.Where(c => c.SubjectCode == Math.Code || c.SubjectCode == Ela.Code).ToList();

            Assert.NotNull(cards);
            Assert.Equal(cards.Count, cardsCheck.Count);
            Assert.Equal(Sort(cards), Sort(cardsCheck));
        }

        [Fact]
        public void TestSearchAllCategories()
        {
            var parameters = new ItemsSearchParams(GoodItemKey.ToString(), GradeLevels.Grade6, new List<string>() { Math.Code }, new string[] { ItMath.Code }, new string[] { Claim1.Code }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            var cardsCheck = Context.ItemCards.Where(c => c.SubjectCode == Math.Code && c.ItemKey == GoodItemKey && c.Grade == GradeLevels.Grade6
                                                     && c.InteractionTypeCode == ItMath.Code && c.ClaimCode == Claim1.Code).ToList();

            Assert.NotNull(cards);
            Assert.Equal(cards.Count, cardsCheck.Count);
            Assert.Equal(Sort(cards), Sort(cardsCheck));
        }

        [Fact]
        public void TestGetItemCardsAll()
        {
            var cards = SampleItemsSearchRepo.GetItemCards();
            Assert.NotNull(cards);
            Assert.Equal(cards.Count, Context.ItemCards.Count());
        }

        [Fact]
        public void TestGetItemCardsMultipleGrade()
        {
            var parameters = new ItemsSearchParams(null, GradeLevels.Grade9 | GradeLevels.Grade4, null, new string[] { }, new string[] { }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            var cardsCheck = Context.ItemCards.Where(c => c.Grade == GradeLevels.Grade9 || c.Grade == GradeLevels.Grade4 
                                                     || c.Grade == GradeLevels.Elementary || c.Grade == GradeLevels.High).ToList();

            Assert.NotNull(cards);
            Assert.Equal(cards.Count, 3);
            Assert.Equal(Sort(cards), Sort(cardsCheck));
        }

        [Fact]
        public void TestGetItemCardsEmptyList()
        {
            var parameters = new ItemsSearchParams("-1", GradeLevels.High, null, new string[] { }, new string[] { }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            Assert.NotNull(cards);
            Assert.Empty(cards);
        }

        [Fact]
        public void TestGetItemCardsItemKey()
        {
            var parameters = new ItemsSearchParams(BadItemKey.ToString(), GradeLevels.All, null, new string[] { }, new string[] { }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            var cardsCheck = Context.ItemCards.Where(c => c.ItemKey.ToString().StartsWith(BadItemKey.ToString())).ToList();

            Assert.NotNull(cards);
            Assert.Equal(cardsCheck.Count, cards.Count);
            Assert.Equal(Sort(cardsCheck), Sort(cards));
        }

        [Fact]
        public void TestGetItemCardsInteractionNoSubject()
        {
            var parameters = new ItemsSearchParams(null, GradeLevels.All, null, new string[] { ItMath.Code }, new string[] { }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            Assert.NotNull(cards);
            Assert.Equal(Context.ItemCards.Count(), cards.Count);

            //Shouldn't filter cards because no subject
            Assert.Equal(Sort(Context.ItemCards), Sort(cards));
        }

        [Fact]
        public void TestGetItemCardsClaimNoSubject()
        {
            var parameters = new ItemsSearchParams(null, GradeLevels.All, null, new string[] { }, new string[] { Claim2.Code }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            Assert.NotNull(cards);
            Assert.Equal(Context.ItemCards.Count(), cards.Count);

            //Shouldn't filter cards because no subject
            Assert.Equal(Sort(Context.ItemCards), Sort(cards));
        }

        [Fact]
        public void TestGetItemCardsSubjectAndInteraction()
        {
            var parameters = new ItemsSearchParams(null, GradeLevels.All, new List<string>() { Math.Code }, new string[] { ItMath.Code }, new string[] { }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            var cardsCheck = Context.ItemCards.Where(c => c.SubjectCode == Math.Code && c.InteractionTypeCode == ItMath.Code).ToList();

            Assert.NotNull(cards);
            Assert.Equal(cardsCheck.Count, cards.Count);
            Assert.Equal(Sort(cardsCheck), Sort(cards));
        }

        [Fact]
        public void TestGetItemCardsSubjectAndClaim()
        {
            var parameters = new ItemsSearchParams(null, GradeLevels.All, new List<string>() { Math.Code }, new string[] { }, new string[] { Claim2.Code }, false, null);
            var cards = SampleItemsSearchRepo.GetItemCards(parameters);
            var cardsCheck = Context.ItemCards.Where(c => c.SubjectCode == Math.Code && c.ClaimCode == Claim2.Code).ToList();

            Assert.NotNull(cards);
            Assert.Equal(cardsCheck.Count, cards.Count);
            Assert.Equal(Sort(cardsCheck), Sort(cards));
        }

        #endregion

        #region GetItemsSearchViewModel

        [Fact]
        public void TestGetItemSearchViewModel()
        {
            var model = SampleItemsSearchRepo.GetItemsSearchViewModel();

            Assert.NotNull(model);
            Assert.Equal(model.InteractionTypes, Context.InteractionTypes);
            Assert.Equal(model.Subjects, Context.Subjects);
        }

        #endregion

    }
}
