using Xunit;
using Moq;
using SmarterBalanced.SampleItems.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using SmarterBalanced.SampleItems.Core.Repos;
using SmarterBalanced.SampleItems.Core.Repos.Models;
using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Immutable;
using SmarterBalanced.SampleItems.Dal.Translations;
using System.Threading.Tasks;

namespace SmarterBalanced.SampleItems.Test.WebTests.ControllerTests
{
    public class ItemControllerTests
    {
        ItemController controller;
        ItemViewModel itemViewModel;
        ItemViewModel itemViewModelCookie;
        AboutThisItemViewModel aboutThisItemViewModel;
        AboutThisItemViewModel aboutItemCookie;
        int bankKey;
        int itemKey;
        string iSAAP;

        public ItemControllerTests()
        {
            bankKey = 234343;
            itemKey = 485954;

            SampleItem digest = SampleItem.Create
            (
                bankKey : bankKey,
                itemKey : itemKey,
                grade : GradeLevels.NA
            );
            ItemCardViewModel card = digest.ToItemCardViewModel();
            var aboutThisItemVM = AboutThisItemViewModel.Create(
                scoring: SampleItemScoring.Create(),
                itemCard: card,
                depthOfKnowledge: "",
                targetDescription: "",
                commonCoreStandardsDescription: "");


            SampleItem digestCookie = SampleItem.Create
            (
                bankKey : bankKey,
                itemKey : 0,
                grade : GradeLevels.NA
            );
            ItemCardViewModel cardCookie = digest.ToItemCardViewModel();


            aboutItemCookie = AboutThisItemViewModel.Create(
                scoring: SampleItemScoring.Create(),
                itemCard: cardCookie,
                depthOfKnowledge: "",
                targetDescription: "",
                commonCoreStandardsDescription: "");

            aboutThisItemViewModel = new AboutThisItemViewModel(
                itemCard: card,
                scoring: SampleItemScoring.Create(),
                targetDescription: "",
                depthOfKnowledge: "",
                commonCoreStandardsDescription: "",
                educationalDifficulty: "",
                evidenceStatement: "",
                associatedItems: "");


            iSAAP = "TDS_test;TDS_test2;";

            string accCookieName = "accessibilitycookie";

            var accessibilityResourceGroups = new List<AccessibilityResourceGroup>();

            var appSettings = new AppSettings()
            {
                SettingsConfig = new SettingsConfig()
                {
                    AccessibilityCookie = accCookieName
                }
            };

            itemViewModel = new ItemViewModel(
                itemViewerServiceUrl: $"http://itemviewerservice.cass.oregonstate.edu/item/{bankKey}-{itemKey}",
                accessibilityCookieName: accCookieName,
                isPerformanceItem: false,
                subject: "MATH",
                moreLikeThisVM: default(MoreLikeThisViewModel),
                brailleItemCodes: new ImmutableArray<string>(),
                braillePassageCodes: new ImmutableArray<string>(),
                brailleItem: null,
                nonBrailleItem: null);

            itemViewModelCookie = new ItemViewModel(
                itemViewerServiceUrl: string.Empty,
                accessibilityCookieName: string.Empty,
                isPerformanceItem: false,
                subject: "MATH",
                moreLikeThisVM: default(MoreLikeThisViewModel),
                brailleItemCodes: new ImmutableArray<string>(),
                braillePassageCodes: new ImmutableArray<string>(),
                brailleItem: null,
                nonBrailleItem: null);

            var itemViewRepoMock = new Mock<IItemViewRepo>();

            itemViewRepoMock
                .Setup(repo =>
                    repo.GetItemViewModel(bankKey, itemKey))
                .Returns(itemViewModel);

            itemViewRepoMock
                .Setup(repo =>
                    repo.GetAboutThisItemViewModel(bankKey, itemKey))
                .Returns(aboutThisItemViewModel);

            var loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger>();
            loggerFactory.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            controller = new ItemController(itemViewRepoMock.Object, appSettings, loggerFactory.Object);
        }

        /// <summary>
        /// Tests that an ItemViewModel is returned given a valid id.
        /// </summary>
        [Fact]
        public void TestDetailsSuccess()
        {
            var result = controller.Details(bankKey, itemKey);

            JsonResult resJson = Assert.IsType<JsonResult>(result);
            var model = Assert.IsType<ItemViewModel>(resJson.Value);

            Assert.Equal(itemViewModel, model);

        }

        /// <summary>
        /// Tests that a BadRequestResult is returned given a null key
        /// </summary>
        [Fact]
        public void TestDetailsNullParam()
        {
            var result = controller.Details(null, itemKey);

            Assert.IsType<BadRequestResult>(result);
        }

        /// <summary>
        /// Tests that a BadRequestResult is returned given a nonexistent key
        /// </summary>
        [Fact]
        public void TestDetailsBadId()
        {
            var result = controller.Details(bankKey + 1, itemKey + 1);

            Assert.IsType<BadRequestResult>(result);
        }

        /// <summary>
        /// Tests that a BadRequestResult is returned given a null key
        /// </summary>
        [Fact]
        public void TestAboutThisItemViewModelNullParam()
        {
            var result = controller.AboutThisItemViewModel(null, itemKey);

            Assert.IsType<BadRequestResult>(result);
        }

        /// <summary>
        /// Tests that the correct result is returned given invalid keys
        /// </summary>
        [Fact]
        public void TestAboutThisItemBadId()
        {
            var result = controller.AboutThisItemViewModel(bankKey + 1, itemKey + 1);

            Assert.IsType<BadRequestResult>(result);
        }

        /// <summary>
        /// Tests that the correct result is returned given good keys
        /// </summary>
        [Fact]
        public void TestAboutThisItemViewModelGood()
        {
            var result = controller.AboutThisItemViewModel(bankKey, itemKey);

            JsonResult resJson = Assert.IsType<JsonResult>(result);

            var model = Assert.IsType<AboutThisItemViewModel>(resJson.Value);

            Assert.Equal(aboutThisItemViewModel, model);
        }

        /// <summary>
        /// Test that the correct result is returned when given a null parameter
        /// </summary>
        [Fact]
        public void TestAccessibilityResourceGroupIsaapNullParam()
        {
            var result = controller.AccessibilityResourceGroupIsaap(null, null, null, false);

            Assert.IsType<BadRequestResult>(result);
        }

        /// <summary>
        /// Test that the correct result is returned when given an invalid braille code
        /// </summary>
        [Fact]
        public async void TestBrailleBadBrailleCode()
        {
            var result = await controller.Braille(bankKey, itemKey, null);
            
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void TestBrailleNullParam()
        {
            var result = await controller.Braille(null, null, null);

            Assert.IsType<BadRequestResult>(result);
        }

        /// <summary>
        /// Test that the correct result is returned when given bad bank and item keys
        /// </summary>
        [Fact]
        public async void TestBrailleBadId()
        {
            var result = await controller.Braille(bankKey + 1, itemKey + 1, "");

            Assert.IsType<BadRequestResult>(result);
        }
    }

}
