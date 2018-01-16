using System;
using System.Collections.Generic;
using System.Text;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.IO;
using SmarterBalanced.SampleItems.Dal.Providers;
using Microsoft.Extensions.Logging;
using System.Linq;
using SmarterBalanced.SampleItems.Dal.Translations;

namespace SmarterBalanced.SampleItems.Core.AccessibilityTesting
{
    public class AccessibilityTestRepo
    {
        private readonly SampleItemsContext context;
        private readonly ILogger logger;

        public AccessibilityTestRepo(SampleItemsContext context, ILoggerFactory loggerFactory)
        {
            this.context = context;
            logger = loggerFactory.CreateLogger<AccessibilityTestRepo>();
        }

        public IList<AccessibilityFamilySearchResult> GetAccessibilityFamilies(string accessibilityResource)
        {
            ImmutableArray<MergedAccessibilityFamily> families = context.MergedAccessibilityFamilies
                .Where(af => af.Resources
                .Any(ar => ar.Label.Contains(accessibilityResource))).ToImmutableArray();

            var results = families.Select(fam => AccessibilityFamilySearchResult.FromMergedFamily(fam)).ToList();

            return results;
        }

        public IList<ItemCardViewModel> GetAccessibilityItems()
        {
           var items = context.SampleItems
                .Where(t => t.AccessibilityResourceGroups
                .Any(ar => ar.AccessibilityResources.Any(ac => (ac.Label == "English Dictionary" && ac.Disabled == false)))).Take(10);
            
            var cards = items.Select(i => i.ToItemCardViewModel()).ToList();
            
            return cards;
        }

        public IList<SampleItem> GetAccessibilityItemsWithResource(AccessibilityTestSearch parms)
        {
            var items = context.SampleItems
                .Where(t => t.AccessibilityResourceGroups
                .Any(ar => ar.AccessibilityResources
                .Any(ac => (parms.Resource.Contains(ac.Label) && ac.Disabled == parms.State)))).ToList();
            var itemKeys = items.Select(i => i.ToItemCardViewModel().ItemKey).ToList();
            return items;
        }

        public IList<BriefSampleItem> GetItemsWithClaim(string claim)
        {
            var items = context.SampleItems.Where(sitem => sitem.Claim.Label == claim).ToList();
            var briefItems = items.Select(i => BriefSampleItem.FromSampleItem(i)).ToList();
            return briefItems;
        }

        public IList<BriefSampleItem> GetAccessibilityTestItems() // Return a list of the top 10 items suitable for testing
        {
            var rand = new Random();
            var briefItems = context.SampleItems.Select(item => BriefSampleItem.FromSampleItem(item)).ToImmutableArray();
            var elaTestItems = briefItems.Where(item => item.BriefResources.Length <= 5).ToImmutableArray();
            var rootTestItem = elaTestItems.ElementAt(rand.Next(elaTestItems.Count()));
            List<BriefSampleItem> testItems = new List<BriefSampleItem>();
            testItems.Add(rootTestItem);

            foreach(BriefAccessibilityResource resource in rootTestItem.BriefResources)
            {
                var checkForResource = testItems.Any(item => !item.BriefResources.Any(res => res.Label == resource.Label));
                if (!checkForResource)
                {
                    var itemSearch = new AccessibilityTestSearch(resource.Label, false);
                    var itemsUsingDisabledResource = GetAccessibilityItemsWithResource(itemSearch);
                    testItems.Add(itemsUsingDisabledResource
                        .Select(item => BriefSampleItem.FromSampleItem(item))
                        .ElementAt(rand.Next(itemsUsingDisabledResource.Count())));
                }
            }

            return testItems;

        }
    }
}
