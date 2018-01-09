using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    }

}
