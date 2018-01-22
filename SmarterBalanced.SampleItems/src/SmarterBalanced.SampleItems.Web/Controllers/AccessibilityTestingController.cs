using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using SmarterBalanced.SampleItems.Core.Repos.Models;
using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using Microsoft.Extensions.Logging;
using SmarterBalanced.SampleItems.Core.AccessibilityTesting;

namespace SmarterBalanced.SampleItems.Web.Controllers
{
    [Route("AccessibilityTesting")]
    public class AccessibilityTestController : Controller
    {
        private readonly AccessibilityTestRepo repo;
        private readonly AppSettings appSettings;
        private readonly ILogger logger;

        public AccessibilityTestController(AccessibilityTestRepo accessibilityTestRepo, AppSettings settings, ILoggerFactory loggerFactory)
        {
            repo = accessibilityTestRepo;
            appSettings = settings;
            logger = loggerFactory.CreateLogger<AccessibilityTestController>();
        }

        [HttpGet("GetTestItems")]
        public IActionResult GetItemUrl()
        {
            var viewModel = repo.GetAccessibilityItems();
            return Json(viewModel);
        }

        [HttpGet("GetAccessibilityFamiliesContainingResource")]
        public IActionResult GetAccessibilityFamiliesContainingResource(string accessibilityResource)
        {
            var families = repo.GetAccessibilityFamilies(accessibilityResource);
            return Json(families);
        }

        [HttpGet("GetItems")]
        public IActionResult GetItemAccessibility(string accessibilityResource, bool enabledState)
        {
            var parms = new AccessibilityTestSearch(accessibilityResource, enabledState);
            var items = repo.GetAccessibilityItemsWithResource(parms);
            return Json(items);
        }

        /// <summary>
        /// Gets a list of all items with a specified claim.
        /// </summary>
        /// <param name="claim">required, desired claim label</param>
        [HttpGet("GetItemsWithClaim")]
        public IActionResult GetItemsWithClaim(string claim)
        {
            var items = repo.GetItemsWithClaim(claim);
            return Json(items);
        }

        /// <summary>
        /// Gets a list of the smallest number of sample items giving full testing coverage
        /// of all Accessibility Resources and returns that list.
        /// </summary>
        [HttpGet("GetELATestItems")]
        public IActionResult GetELATestItems()
        {
            var items = repo.GetAccessibilityTestItems();
            return Json(items);
        }

        ///<summary>
        /// Gets a list of sample items that give full testing coverage of all Accessibility
        /// Resources, associates each resource with an item that it can be tested with,
        /// and returns a CSV file with information on each resource/item pair.
        ///</summary>
        [HttpGet("GetTestCase")]
        public IActionResult GetTestCase()
        {
            var baseUrl = Request.Host.ToString();
            var items = repo.GetAccessibilityTestItems(baseUrl);
            var printableItems = repo.FormatAccessibilityTestItems(items);
            var orderedItems = printableItems.OrderBy(item => item.ItemKey);
            var csvStream = new MemoryStream();
            var writer = new StreamWriter(csvStream, System.Text.Encoding.UTF8);
            var csv = new CsvWriter(writer);

            csv.WriteRecords(orderedItems);
            writer.Flush();
            csvStream.Seek(0, SeekOrigin.Begin);

            return File(csvStream, "text/csv", "AccessibilityTestCaseItems.csv");
        }

        [HttpGet("GetInteractions")]
        public IActionResult GetInteractions()
        {
            return Json(repo.GetInteractionTypes());
        }

        [HttpGet("GetInteractionTestSet")]
        public IActionResult GetInteractionTestSet()
        {
            var baseUrl = Request.Host.ToString();
            var testSet = repo.GetInteractionTestItems(baseUrl);
            var orderedItems = testSet.OrderBy(item => item.InteractionUnderTest);
            var csvStream = new MemoryStream();
            var writer = new StreamWriter(csvStream, System.Text.Encoding.UTF8);
            var csv = new CsvWriter(writer);

            csv.WriteRecords(orderedItems);
            writer.Flush();
            csvStream.Seek(0, SeekOrigin.Begin);

            return File(csvStream, "text/csv", "InteractionTestCaseItems.csv");
        }

    }

}
