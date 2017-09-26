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
                rubrics: ImmutableArray.Create<Rubric>(),
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

            var aboutItemCookie = AboutThisItemViewModel.Create(
                rubrics: ImmutableArray.Create<Rubric>(),
                itemCard: cardCookie,
                depthOfKnowledge: "",
                targetDescription: "",
                commonCoreStandardsDescription: "");



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
                accResourceGroups: default(ImmutableArray<AccessibilityResourceGroup>),
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
                accResourceGroups: accessibilityResourceGroups.ToImmutableArray(),
                moreLikeThisVM: default(MoreLikeThisViewModel),
                brailleItemCodes: new ImmutableArray<string>(),
                braillePassageCodes: new ImmutableArray<string>(),
                brailleItem: null,
                nonBrailleItem: null);

            var itemViewRepoMock = new Mock<IItemViewRepo>();

            itemViewRepoMock
                .Setup(repo =>
                    repo.GetItemViewModel(bankKey, itemKey, It.Is<string[]>(strings => strings.Length == 0), It.IsAny<Dictionary<string, string>>()))
                .Returns(itemViewModel);

            itemViewRepoMock
                .Setup(repo =>
                    repo.GetItemViewModel(
                        bankKey,
                        itemKey,
                        It.IsAny<string[]>(),
                        It.IsAny<Dictionary<string, string>>()))
                .Returns(itemViewModel);

            var loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger>();
            loggerFactory.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            controller = new ItemController(itemViewRepoMock.Object, appSettings, loggerFactory.Object);
        }

        /// <summary>
        /// Tests that an ItemViewModel is returned given a vaid id.
        /// </summary>
        [Fact]
        public void TestDetailsSuccess()
        {
            var result = controller.Details(bankKey, itemKey, iSAAP);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ItemViewModel>(viewResult.ViewData.Model);

            Assert.Equal(itemViewModel, model);

        }

        /// <summary>
        /// Tests that a BadRequestResult is returned given a null key
        /// </summary>
        [Fact]
        public void TestDetailsNullParam()
        {
            var result = controller.Details(null, itemKey, iSAAP);

            Assert.IsType<BadRequestResult>(result);
        }

        /// <summary>
        /// Tests that a BadRequestResult is returned given a nonexistent key
        /// </summary>
        [Fact]
        public void TestDetailsBadId()
        {
            var result = controller.Details(bankKey + 1, itemKey + 1, iSAAP);

            Assert.IsType<BadRequestResult>(result);
        }

        /// <summary>
        /// Tests that a cookie ISSAP is returned instead of param
        /// </summary>
        [Fact]
        public void TestDetailsNoISAAP()
        {
            var result = controller.Details(bankKey, itemKey, string.Empty);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ItemViewModel>(viewResult.ViewData.Model);

            Assert.Equal(itemViewModel, model);
        }

        /// <summary>
        /// Tests that Index returns correct RedirectToAction controller and action name
        /// </summary>
        [Fact]
        public void TestIndex()
        {
            var result = controller.Index();
            var resultRedirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", resultRedirect.ActionName);
            Assert.Equal("itemsSearch", resultRedirect.ControllerName);
        }
    }

}
