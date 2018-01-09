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

        public IList<SampleItem> GetAccessibilityItems()
        {
           var items = context.SampleItems
                .Where(t => t.AccessibilityResourceGroups
                .Any(
                    ar =>
                        ar.AccessibilityResources
                        .Any(ac => ac.Label == "Calculator")));
            
            return items.Take(10).ToList();
        }

    }
}
