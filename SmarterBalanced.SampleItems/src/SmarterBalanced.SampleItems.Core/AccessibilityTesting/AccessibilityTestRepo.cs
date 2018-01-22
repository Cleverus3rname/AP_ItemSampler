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

            // Need a list of brief sample items sorted by grade.
            var gradeSortedItems = context.SampleItems
                .OrderBy(item => item.Grade)
                .Select(item => BriefSampleItem.FromSampleItem(item, baseUrl)).ToImmutableArray();
            
            // List of all items with numAcceptableDisables resources disabled. This keeps the number of items needed to a minimum.
            var testableItems = gradeSortedItems.Where(item => 
                item.AccessibilityResources.Where(res => res.Disabled == true).Count()
                <= numAcceptableDisables).ToImmutableArray();

            // Get a single item to use as the base for the low level coverage. Other items will be selected to fill in the 
            // resources not covered by this Key Test item.
            // Selects a random item that is within the Elementary grade levels. 
            var lowTestKey = testableItems
                .ElementAt(rand
                .Next(testableItems.Count(item => item.Grade <= GradeLevels.Elementary)));

            // Get a Key test item to use as the base for high level coverage. Other items will be selected to fill in the 
            // resources not covered by this key test item.
            // Selects a random item that is within the High School grade levels. 
            var highTestKey = testableItems
                .ElementAt(rand
                .Next(testableItems.Count(item => item.Grade <= GradeLevels.Middle), 
                    testableItems.Count()));

            List<BriefSampleItem> lowTestItems = new List<BriefSampleItem>();
            List<BriefSampleItem> highTestItems = new List<BriefSampleItem>();

            lowTestItems.Add(lowTestKey);
            highTestItems.Add(highTestKey);

            // Iterate over the disabled resources in the selected Low Level Test Key and find sample items that have that resource
            // enabled. 
            foreach(AccessibilityResource lowResource in lowTestKey.AccessibilityResources.Where(res => res.Disabled == true))
            {
                // Check whether the currently selected resource is already covered by an item selected for testing.
                var checkForResource = lowTestItems
                    .Any(item => item.AccessibilityResources
                    .Any(res => res.Label == lowResource.Label && res.Disabled == false));
                
                if (!checkForResource)
                {
                    // Construct a list of items that have the currently selected resource enabled
                    // This list can potentially be empty if no accessable items use the selected resource,
                    // for example, no Elementary level items use the Calculator resource.
                    var itemsUsingDisabledResource = gradeSortedItems
                        .Where(item => item.AccessibilityResources
                        .Any(res => res.Label == lowResource.Label && res.Disabled == false)).ToList();
                    try // Needed in case the above list is empty
                    {
                        // If there are any items in the elementary school range, select from those
                        if (itemsUsingDisabledResource.Any(item => item.Grade <= GradeLevels.Elementary))
                        {
                            lowTestItems.Add(itemsUsingDisabledResource
                                .ElementAt(rand
                                // Select a random item with an index lower than the total number of elementary level
                                // items in the list
                                .Next(itemsUsingDisabledResource.Count(item => item.Grade <= GradeLevels.Elementary))));
                        }
                        // If there aren't any elementary level items, select from middle school items.
                        else
                        {
                            lowTestItems.Add(itemsUsingDisabledResource
                                .ElementAt(rand
                                // Select a random item with an index lower than the total number of Middle school level
                                // items in the list
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

            // Iterate over the disabled resources in the selected high level Test Key and find sample items that have that resource
            // enabled.
            foreach(AccessibilityResource highResource in highTestKey.AccessibilityResources.Where(res => res.Disabled == true))
            {
                // Check whether the currently selected resource is already covered by an item selected for testing.
                var checkForResource = highTestItems
                    .Any(item => item.AccessibilityResources
                    .Any(res => res.Label == highResource.Label && res.Disabled == false));
                
                if (!checkForResource)
                {
                    // Construct a list of items that have the currently selected resource enabled
                    // This list can potentially be empty if no accessable items use the selected resource,
                    // for example, no Elementary level items use the Calculator resource.
                    var itemsUsingDisabledResource = gradeSortedItems
                        .Where(item => item.AccessibilityResources
                        .Any(res => res.Label == highResource.Label && res.Disabled == false)).ToList();
                    try // Needed in case the above list is empty
                    {
                        // If there are any items in the high school range, select from those
                        if (itemsUsingDisabledResource.Any(item => item.Grade > GradeLevels.Middle))
                        {
                            highTestItems.Add(itemsUsingDisabledResource
                                .ElementAt(rand
                                // Select a random item with an index after the Middle school items and before
                                // the end of the list
                                .Next(itemsUsingDisabledResource.Count(item => item.Grade <= GradeLevels.Middle)
                                    ,itemsUsingDisabledResource.Count())));
                        }
                        // If there aren't any High level items, select from middle school items.
                        else
                        {
                            lowTestItems.Add(itemsUsingDisabledResource
                                .ElementAt(rand
                                    // Select a random item with an index after the Elementary school items and before
                                    // the end of the list
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

            // Grab a sample item, whichever is at the top of the list. All sample items contain a list 
            // of all Accessibility resources. We'll use this item to get a list of all accessibility resources.
            SampleItem resourceListItem = context.SampleItems.First();

            // Create an array of resources
            ImmutableArray<AccessibilityResource> resourceList = resourceListItem.AccessibilityResourceGroups
                .SelectMany(group => group.AccessibilityResources.Select(r => r)).ToImmutableArray();
            
            // Create a list of the names of all resources.
            List<string> accessibilityResourceTitles = resourceList.Select(r => r.Label).ToList();
           
            // Create a place to store all formatted test items.
            List<AccessibilityTestItem> itemsUnderTest = new List<AccessibilityTestItem>();

            // Iterate over the resources in the list to associate test items with a resource to test
            foreach(string selectedResource in accessibilityResourceTitles)
            {
                // Select a random item from the list of items selected for testing.
                var selectedItem = testItems.ElementAt(rand.Next(testItems.Count()));

                // Keep selecting a new item until one that has the currently selected resource enabled is found.
                while (selectedItem.AccessibilityResources.Any(res => res.Label == selectedResource && res.Disabled == true))
                    selectedItem = testItems.ElementAt(rand.Next(testItems.Count()));
                
                // Add the association to the list of formatted items.
                itemsUnderTest.Add(AccessibilityTestItem.FromBriefSampleItem(selectedItem, selectedResource, context));
            }
            return itemsUnderTest;
        }
    }
}
