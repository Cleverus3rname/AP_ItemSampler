using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmarterBalanced.SampleItems.Core.Reporting;
using Microsoft.Extensions.Logging;

namespace SmarterBalanced.SampleItems.Web.Controllers
{
    [Route("api/Reporting")]
    public class ReportingController : Controller
    {
        private readonly IReportingRepo reportingRepo;
        private readonly ILogger logger;

        public ReportingController(IReportingRepo repo, ILoggerFactory loggerFactory)
        {
            reportingRepo = repo;
            logger = loggerFactory.CreateLogger<ReportingController>();
        }

        [HttpGet("AccessibilityWalk")]
        public IActionResult AccessbiilityWalk()
        {
            var baseUrl = Request.Host.ToString();

            var fileStream = reportingRepo.AccessibilityWalk(baseUrl);
            fileStream.FileDownloadName = "Accessibility.csv";
            return fileStream;
        }
    }
}