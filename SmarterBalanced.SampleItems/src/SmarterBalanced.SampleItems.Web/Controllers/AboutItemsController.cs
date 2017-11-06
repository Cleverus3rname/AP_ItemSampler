using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmarterBalanced.SampleItems.Core.Repos.Models;
using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using Microsoft.Extensions.Logging;

namespace SmarterBalanced.SampleItems.Web.Controllers
{
    [Route("AboutItems")]
    public class AboutItemsController : Controller
    {
        private readonly IAboutItemsRepo repo;
        private readonly AppSettings appSettings;
        private readonly ILogger logger;

        public AboutItemsController(IAboutItemsRepo aboutItemRepo, AppSettings settings, ILoggerFactory loggerFactory)
        {
            repo = aboutItemRepo;
            appSettings = settings;
            logger = loggerFactory.CreateLogger<AboutItemsController>();
        }

        [HttpGet("GetItemUrl")]
        public IActionResult GetItemUrl(string interactionTypeCode)
        {
            var viewModel = repo.GetAboutItemsViewModel(interactionTypeCode);
            return Json(viewModel);
        }

    }

}
