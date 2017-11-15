using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SmarterBalanced.SampleItems.Core.Repos;
using SmarterBalanced.SampleItems.Core.Repos.Models;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using SmarterBalanced.SampleItems.Web.Controllers;
using System.Collections.Generic;
using System.Collections.Immutable;
using Xunit;
using System.Linq;

namespace SmarterBalanced.SampleItems.Test.WebTests.ControllerTests
{
    public class ItemsSearchControllerTests
    {
        BrowseItemsController controller;
        BrowseItemsController controllerBadReq;
        ItemsSearchViewModel itemsSearchViewModel;
        ItemsSearchViewModel itemsSearchViewModelBad;
        int goodItemKey = 89;

        public ItemsSearchControllerTests()
        {
            itemsSearchViewModel = new ItemsSearchViewModel();
            itemsSearchViewModelBad = null;
            var sampleItemsSearchRepoMock = new Mock<ISampleItemsSearchRepo>();
            var sampleItemsSearchRepoBadRequestMock = new Mock<ISampleItemsSearchRepo>();
            sampleItemsSearchRepoMock.Setup(x => x
                .GetItemCards(It.Is<ItemsSearchParams>(p => p.ItemId == null)))
                .Returns(new List<ItemCardViewModel>());

            sampleItemsSearchRepoMock.Setup(x => x
                .GetItemCards(It.Is<ItemsSearchParams>(p => p.ItemId == goodItemKey.ToString())))
                .Returns(new List<ItemCardViewModel> { ItemCardViewModel.Create(itemKey: goodItemKey) });

            sampleItemsSearchRepoMock.Setup(x => x.GetItemsSearchViewModel()).Returns(itemsSearchViewModel);
            sampleItemsSearchRepoBadRequestMock.Setup(x => x.GetItemsSearchViewModel()).Returns(itemsSearchViewModelBad);
            var loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger>();
            loggerFactory.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
            controller = new BrowseItemsController(sampleItemsSearchRepoMock.Object, loggerFactory.Object);
            controllerBadReq = new BrowseItemsController(sampleItemsSearchRepoBadRequestMock.Object, loggerFactory.Object);
        }

        /// <summary>
        /// Tests that an empty ItemCardViewModel List is returned, given no parms.
        /// </summary>
        [Fact]
        public void TestSearchNoResult()
        {
            var result = controller.Search(null, GradeLevels.High, null, null, null, false, null) as JsonResult;
            var resultList = result.Value as List<ItemCardViewModel>;

            Assert.IsType<List<ItemCardViewModel>>(result.Value);
            Assert.IsType<JsonResult>(result);
            Assert.Equal(0, resultList.Count);
        }

        /// <summary>
        /// Tests that an ItemCardViewModel is returned given a correct ItemKey
        /// </summary>
        [Fact]
        public void TestSearchResult()
        {
            var result = controller.Search(goodItemKey.ToString(), GradeLevels.High, null, null, null, false, null) as JsonResult;
            var resultList = result.Value as List<ItemCardViewModel>;

            Assert.Equal(resultList.Count, 1);
            Assert.Equal(resultList[0].ItemKey, goodItemKey);
        }
    }
}
