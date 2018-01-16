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
                .Any(ar => ar.AccessibilityResources.Any(ac => (ac.Label == "Calculator" && ac.Disabled == false)))).Take(10);
            
            var cards = items.Select(i => i.ToItemCardViewModel()).ToList();
            
            return cards;
        }

        public IList<int> GetAccessibilityItemsWithResource(AccessibilityTestSearch parms)
        {
            var items = context.SampleItems
                .Where(t => t.AccessibilityResourceGroups
                .Any(ar => ar.AccessibilityResources
                .Any(ac => (parms.Resource.Contains(ac.Label) && ac.Disabled == parms.State))));
            var itemKeys = items.Select(i => i.ToItemCardViewModel().ItemKey).ToList();
            return itemKeys;
        }

        public IList<BriefSampleItem> GetItemsWithClaim(string claim)
        {
            var items = context.SampleItems.Where(sitem => sitem.Claim.Label == claim).ToList();
            var briefItems = items.Select(i => BriefSampleItem.FromSampleItem(i)).ToList();
            return briefItems;
        }

    }
}
