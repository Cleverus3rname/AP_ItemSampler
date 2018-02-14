using Microsoft.Extensions.Logging;
using SmarterBalanced.SampleItems.Core.Repos.Models;
using SmarterBalanced.SampleItems.Core.Translations;
using SmarterBalanced.SampleItems.Dal.Providers;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Gen = SmarterBalanced.SampleItems.Dal.Xml.Models;


namespace SmarterBalanced.SampleItems.Core.Repos
{
    public class SampleItemsSearchRepo : ISampleItemsSearchRepo
    {
        private readonly SampleItemsContext context;
        private readonly ILogger logger;

        public SampleItemsSearchRepo(SampleItemsContext context, ILoggerFactory loggerFactory)
        {
            this.context = context;
            logger = loggerFactory.CreateLogger<SampleItemsSearchRepo>();
        }

        public FilterSearch GetFilterSearch()
        {
            return context.FilterSearch;
        }

        public IList<ItemCardViewModel> GetItemCards()
        {
            return context.ItemCards.ToList();
        }

        public IList<ItemCardViewModel> GetItemCards(ItemsSearchParams parms)
        {
            var query = context.ItemCards.Where(i => i.Grade != GradeLevels.NA && !i.BrailleOnlyItem);

            if (parms == null)
            {
                return query.ToList();
            }

            int itemId;
            if (int.TryParse(parms.ItemId, out itemId))
            {
                query = query.Where(i => i.ItemKey.ToString().StartsWith(itemId.ToString()));
            }

            if (parms.Grades != GradeLevels.All && parms.Grades != GradeLevels.NA)
                query = query.Where(i => GradeLevelsUtils.Contains(parms.Grades, i.Grade));

            if (parms.Subjects != null && parms.Subjects.Any())
            {
                query = query.Where(i => parms.Subjects.Contains(i.SubjectCode));
                if (parms.InteractionTypes.Any())
                    query = query.Where(i => parms.InteractionTypes.Contains(i.InteractionTypeCode));
                if (parms.ClaimIds.Any())
                    query = query.Where(i => parms.ClaimIds.Contains(i.ClaimCode));
            }

            if (parms.PerformanceOnly)
            {
                query = query.Where(i => i.IsPerformanceItem);
            }
            if (parms.Targets != null && parms.Targets.Any())
            {
                query = query.Where(i => parms.Targets.Contains(i.TargetHash));
            }

            return query.OrderBy(i => i.SubjectCode).ThenBy(i => i.Grade).ThenBy(i => i.ClaimCode).ToList();
        }

        public ItemsSearchViewModel GetItemsSearchViewModel()
        {
            return new ItemsSearchViewModel
            {
                InteractionTypes = context.InteractionTypes,
                Subjects = context.Subjects,
                Targets = context.Targets,
                Claims = context.Claims
            };
        }

        public IList<SampleItemViewModel> GetSampleItemViewModels(string baseUrl)
        {
            var items = context.SampleItems.Select(s => s.ToSampleItemViewModel(baseUrl)).ToList();

            return items;
        }


        public List<ExpandoObject> GetItemsAccessibilityWalk(string baseUrl)
        {
            var items = context.SampleItems.Select(si => new
            {
                item = si,
                resources = si.AccessibilityResourceGroups
                    .SelectMany(arg =>
                        arg.AccessibilityResources
                            .ToDictionary(ar => ar.Label, ar => !ar.Disabled))
            }).OrderBy(i => i.item.Subject.Code).ThenBy(i => i.item.Grade.IndividualGradeToNumString()).ThenBy(i => i.item.Claim.ClaimNumber).ToList();
            var records = new List<ExpandoObject>();
            foreach (var item in items)
            {

                var rowItem = new ExpandoObject() as IDictionary<string, object>;
                rowItem.Add("Item", item.item.ToString());
                rowItem.Add("Grade", item.item.Grade.ToDisplayString());
                rowItem.Add("Subject", item.item.Subject.ShortLabel);

                foreach (var key in item.resources)
                {
                    rowItem.Add(key.Key, key.Value);
                };

                rowItem.Add("URL", $"=HYPERLINK(\"http://siw-dev.cass.oregonstate.edu/Item/{item.item.ToString()}\")");
                records.Add(rowItem as ExpandoObject);

            }


            return records;
        }
    }
}
