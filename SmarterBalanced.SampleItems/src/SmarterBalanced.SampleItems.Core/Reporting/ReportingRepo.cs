using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmarterBalanced.SampleItems.Core.Reporting.Models;
using SmarterBalanced.SampleItems.Dal.Providers;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System.Collections.Generic;
using System.Linq;

namespace SmarterBalanced.SampleItems.Core.Reporting
{
    public class ReportingRepo : IReportingRepo
    {
        private readonly SampleItemsContext context;
        private readonly ILogger logger;

        public ReportingRepo(SampleItemsContext context, ILoggerFactory loggerFactory)
        {
            this.context = context;
            logger = loggerFactory.CreateLogger<ReportingRepo>();
        }

        public FileStreamResult AccessibilityWalk(string baseUrl)
        {
            var csvAccessibility = GetItemsAccessibilityWalk(baseUrl);

            return csvAccessibility.CsvFileStream();
        }

        public CsvReport GetItemsAccessibilityWalk(string baseUrl)
        {
            var records = new List<Dictionary<string, string>>();
            var items = context.SampleItems.Select(si => new
            {
                item = si,
                resources = si.AccessibilityResourceGroups
                    .SelectMany(arg =>
                        arg.AccessibilityResources
                            .ToDictionary(ar => ar.Label, ar => !ar.Disabled))
            }).OrderBy(i => i.item.Subject.Code)
                .ThenBy(i => i.item.Grade.IndividualGradeToNumString())
                .ThenBy(i => i.item.Claim.ClaimNumber).ToList();

            foreach (var item in items)
            {
                var rowItem = new Dictionary<string, string>();
                rowItem.Add("Item", item.item.ToString());
                rowItem.Add("Grade", item.item.Grade.ToDisplayString());
                rowItem.Add("Subject", item.item.Subject.ShortLabel);

                foreach (var key in item.resources)
                {
                    rowItem.Add(key.Key, key.Value.ToString());
                };

                rowItem.Add("URL", $"=HYPERLINK(\"{baseUrl}/Item/{item.item.ToString()}\")");
                records.Add(rowItem);
            }

            var headerKeys = records.ElementAtOrDefault(0).Select(r => r.Key).ToList();

            return new CsvReport { TableRows = records, HeaderColumns = headerKeys };
        }
    }
}