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

        ///<summary>
        /// Gets a list of all items matching a set of conditions
        ///</summary>
        ///<param name="parms">required, an object containing a resource label and bool indicating disabled or enabled</param>
        ///<returns name="items">a list of all items containing the specified resource and state</returns>
        public IList<SampleItem> GetAccessibilityItemsWithResource(AccessibilityTestSearch parms)
        {
            var items = context.SampleItems
                .Where(t => t.AccessibilityResourceGroups
                .Any(ar => ar.AccessibilityResources
                .Any(ac => (parms.Resource.Contains(ac.Label) && ac.Disabled == parms.State)))).Take(10).ToList();
            
            var itemKeys = items.Select(i => i.ToItemCardViewModel().ItemKey).ToList();
            return items;
        }

        ///<summary>
        /// Gets a list of all items associated with the specified claim
        ///</summary>
        ///<param name="claim">reqired, the claim label to match against</param>
        ///<returns name="briefItems">a list of all items with specified claim</returns>
        public IList<BriefSampleItem> GetItemsWithClaim(string claim)
        {
            var items = context.SampleItems.Where(sitem => sitem.Claim.Label == claim).ToList();
            var briefItems = items.Select(i => BriefSampleItem.FromSampleItem(i)).ToList();
            return briefItems;
        }

        ///<summary>
        /// Return a list of brief sample items that give full testing coverage of the Accessibility Resources
        /// for the lower level grades and the higher level grades.
        ///</summary>
        ///<param name="domainUrl">optional, the base domain of the current context</param>
        ///<returns name="testSet">a list of the brief sample items selected for testing</returns>
        public IList<BriefSampleItem> GetAccessibilityTestItems(string domainUrl = "") 
        {
            int numAcceptableDisables = 4;
            var baseUrl = domainUrl;
            var rand = new Random();

            var gradeSortedItems = context.SampleItems
                .OrderBy(item => item.Grade)
                .Select(item => BriefSampleItem.FromSampleItem(item, baseUrl)).ToImmutableArray();
            var testableItems = gradeSortedItems.Where(item => 
                item.AccessibilityResources.Where(res => res.Disabled == true).Count()
                <= numAcceptableDisables).ToImmutableArray();

            var lowTestKey = testableItems
                .ElementAt(rand
                .Next(testableItems.Count(item => item.Grade <= GradeLevels.Elementary)));

            var highTestKey = testableItems
                .ElementAt(rand
                .Next(testableItems.Count(item => item.Grade <= GradeLevels.Middle), 
                    testableItems.Count()));

            List<BriefSampleItem> lowTestItems = new List<BriefSampleItem>();
            List<BriefSampleItem> highTestItems = new List<BriefSampleItem>();

            lowTestItems.Add(lowTestKey);
            highTestItems.Add(highTestKey);

            foreach(AccessibilityResource lowResource in lowTestKey.AccessibilityResources.Where(res => res.Disabled == true))
            {
                var checkForResource = lowTestItems
                    .Any(item => item.AccessibilityResources
                    .Any(res => res.Label == lowResource.Label && res.Disabled == false));
                
                if (!checkForResource)
                {
                    var itemsUsingDisabledResource = gradeSortedItems
                        .Where(item => item.AccessibilityResources
                        .Any(res => res.Label == lowResource.Label && res.Disabled == false)).ToList();
                    try
                    {
                        if (itemsUsingDisabledResource.Any(item => item.Grade <= GradeLevels.Elementary))
                        {
                            lowTestItems.Add(itemsUsingDisabledResource
                                .ElementAt(rand
                                .Next(itemsUsingDisabledResource.Count(item => item.Grade <= GradeLevels.Elementary))));
                        }
                        else
                        {
                            lowTestItems.Add(itemsUsingDisabledResource
                                .ElementAt(rand
                                .Next(itemsUsingDisabledResource.Count(item => item.Grade <= GradeLevels.Middle))));
                        }

                    }
                    catch (System.ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine("No Elementary Level Items found with {0} Resource Enabled", lowResource.Label);
                        Console.WriteLine("Exception source: {0}", e.Source);
                    }
                }
            }

            foreach(AccessibilityResource highResource in highTestKey.AccessibilityResources.Where(res => res.Disabled == true))
            {
                var checkForResource = highTestItems
                    .Any(item => item.AccessibilityResources
                    .Any(res => res.Label == highResource.Label && res.Disabled == false));
                
                if (!checkForResource)
                {
                    var itemsUsingDisabledResource = gradeSortedItems
                        .Where(item => item.AccessibilityResources
                        .Any(res => res.Label == highResource.Label && res.Disabled == false)).ToList();
                    try
                    {
                        if (itemsUsingDisabledResource.Any(item => item.Grade > GradeLevels.Middle))
                        {
                            highTestItems.Add(itemsUsingDisabledResource
                                .ElementAt(rand
                                .Next(itemsUsingDisabledResource.Count(item => item.Grade <= GradeLevels.Middle)
                                    ,itemsUsingDisabledResource.Count())));
                        }
                        else
                        {
                            lowTestItems.Add(itemsUsingDisabledResource
                                .ElementAt(rand
                                    .Next(itemsUsingDisabledResource.Count(item => item.Grade <= GradeLevels.Elementary),
                                        itemsUsingDisabledResource.Count())));
                        }
                    }
                    catch (System.ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine("No High School Level Items found with {0} Resource Enabled", highResource.Label);
                        Console.WriteLine("Exception Source: {0}", e.Source);
                    }
                }
            }

            var testSet = lowTestItems.Concat(highTestItems).ToList();
            return testSet;

        }

        ///<summary>
        /// Takes a list of brief sample items that give full testing coverage of the Accessibility Resources
        /// for lower and higher level grades (see GetAccessibilityTestItems) and returns a list of 
        /// AccessibilityTestItems which contain information about the Sample item and the Resource it has been
        /// selected for testing.
        ///</summary>
        ///<param name="testItems">required, a list of brief items selected for testing</param>
        ///<returns name="itemsUnderTest">a list of objects containing an item/resource association and item info</returns>
        public IList<AccessibilityTestItem> FormatTestItems(IList<BriefSampleItem> testItems)
        {
            var rand = new Random();
            SampleItem resourceListItem = context.SampleItems.First();
            ImmutableArray<AccessibilityResource> resourceList = resourceListItem.AccessibilityResourceGroups
                .SelectMany(group => group.AccessibilityResources.Select(r => r)).ToImmutableArray();
            List<string> accessibilityResourceTitles = resourceList.Select(r => r.Label).ToList();
            List<AccessibilityTestItem> itemsUnderTest = new List<AccessibilityTestItem>();
            foreach(string selectedResource in accessibilityResourceTitles)
            {
                var selectedItem = testItems.ElementAt(rand.Next(testItems.Count()));
                while (selectedItem.AccessibilityResources.Any(res => res.Label == selectedResource && res.Disabled == true))
                    selectedItem = testItems.ElementAt(rand.Next(testItems.Count()));t));
            }
            return itemsUnderTest;
        }
    }
}
