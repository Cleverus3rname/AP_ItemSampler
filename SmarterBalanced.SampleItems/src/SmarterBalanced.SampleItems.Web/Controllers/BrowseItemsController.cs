using CsvHelper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmarterBalanced.SampleItems.Core.Repos;
using SmarterBalanced.SampleItems.Core.Repos.Models;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SmarterBalanced.SampleItems.Web.Controllers
{
    [Route("BrowseItems")]
    public class BrowseItemsController : Controller
    {
        private readonly ISampleItemsSearchRepo sampleItemsSearchRepo;
        private readonly ILogger logger;

        public BrowseItemsController(ISampleItemsSearchRepo repo, ILoggerFactory loggerFactory )
        {
            sampleItemsSearchRepo = repo;
            logger = loggerFactory.CreateLogger<BrowseItemsController>();
        }

        /// <summary>
        /// Instantiates and returns a JSON serialized ItemsSearchViewModel.
        /// </summary>
        [HttpGet("ItemsSearchViewModel")]
        public IActionResult ItemsSearchViewModel()
        {
            var model = sampleItemsSearchRepo.GetItemsSearchViewModel();
            if(model == null)
            {
                return BadRequest();
            }

            return Json(model);
        }

        /// <summary>
        /// Instantiates and returns a JSON serialized FilterSearchModel.
        /// </summary>
        [HttpGet("FilterSearchModel")]
        public IActionResult FilterSearchModel()
        {
            var model = sampleItemsSearchRepo.GetFilterSearch();
            if (model == null)
            {
                return BadRequest();
            }

            return Json(model);
        }

        /// <summary>
        /// Returns a list of ItemCardViewModels based on the function arguments.
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="gradeLevels"></param>
        /// <param name="subjects"></param>
        /// <param name="interactionTypes"></param>
        /// <param name="claims"></param>
        /// <param name="performanceOnly"></param>
        /// <param name="targets"></param>
        [HttpGet("Search")]
        [EnableCors("AllowAllOrigins")]
        public IActionResult Search(string itemID, GradeLevels gradeLevels, string[] subjects, string[] interactionTypes, string[] claims, bool performanceOnly, int[] targets)
        {
            var parms = new ItemsSearchParams(itemID, gradeLevels, subjects, interactionTypes, claims, performanceOnly, targets);
            var items = sampleItemsSearchRepo.GetItemCards(parms);
            return Json(items);
        }

        /// <summary>
        /// Takes the current URL, converts it to a string, and returns a list 
        /// of JSON serialized SampleItemViewModels.
        /// </summary>
        [HttpGet("ExportItems")]
        [EnableCors("AllowAllOrigins")]
        public IActionResult ExportItems()
        {
            var baseUrl = Request.Host.ToString();
            var items = sampleItemsSearchRepo.GetSampleItemViewModels(baseUrl);
            return Json(items);
        }

        /// <summary>
        /// Creates and returns a .csv file from a list of SampleItemViewModels based on the current url. 
        /// </summary>
        [HttpGet("Export")]
        public IActionResult Export()
        {
            var baseUrl = Request.Host.ToString();
            var items = sampleItemsSearchRepo.GetSampleItemViewModels(baseUrl);
            var csvStream = new MemoryStream();
            var writer = new StreamWriter(csvStream, System.Text.Encoding.UTF8);
            var csv = new CsvWriter(writer);

            csv.WriteRecords(items);
            writer.Flush();
            csvStream.Seek(0, SeekOrigin.Begin);

            return File(csvStream, "text/csv", "SIWItems.csv");
        }
    }
}
