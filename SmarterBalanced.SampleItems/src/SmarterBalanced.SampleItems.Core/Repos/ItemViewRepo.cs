﻿using SmarterBalanced.SampleItems.Core.Repos.Models;
using SmarterBalanced.SampleItems.Dal.Providers;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using SmarterBalanced.SampleItems.Core.Translations;
using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace SmarterBalanced.SampleItems.Core.Repos
{
    public class ItemViewRepo : IItemViewRepo
    {
        private readonly SampleItemsContext context;
        private readonly ILogger logger;

        public ItemViewRepo(SampleItemsContext context, ILoggerFactory loggerFactory)
        {
            this.context = context;
            logger = loggerFactory.CreateLogger<ItemViewRepo>();
        }

        public AppSettings AppSettings
        {
            get
            {
                return context.AppSettings;
            }
        }

        public ItemDigest GetItemDigest(int bankKey, int itemKey)
        {
            return context.ItemDigests.SingleOrDefault(item => item.BankKey == bankKey && item.ItemKey == itemKey);
        }

        public ItemCardViewModel GetItemCardViewModel(int bankKey, int itemKey)
        {
            return context.ItemCards.SingleOrDefault(item => item.BankKey == bankKey && item.ItemKey == itemKey);
        }

        /// <summary>
        /// Constructs an itemviewerservice URL to access the 
        /// item corresponding to the given ItemDigest.
        /// </summary>
        private string GetItemViewerUrl(ItemDigest digest, string iSAAPcode)
        {
            if (digest == null)
            {
                return string.Empty;
            }

            string baseUrl = context.AppSettings.SettingsConfig.ItemViewerServiceURL;
            return $"{baseUrl}/item/{digest.BankKey}-{digest.ItemKey}?isaap={iSAAPcode}";
        }

        /// <summary>
        /// Constructs an itemviewerservice URL to access the 
        /// item corresponding to the given ItemDigest.
        /// </summary>
        private string GetItemViewerUrl(ItemDigest digest)
        {
            if (digest == null)
            {
                return string.Empty;
            }

            string baseUrl = context.AppSettings.SettingsConfig.ItemViewerServiceURL;
            return $"{baseUrl}/item/{digest.BankKey}-{digest.ItemKey}";
        }

        private List<AccessibilityResource> SetResourceValuesFromCookie(ImmutableArray<AccessibilityResource> cookiePreferences, ImmutableArray<AccessibilityResource> defaultPreferences)
        {
            List<AccessibilityResource> computedResources = new List<AccessibilityResource>();

            //Use the defaults for any disabled accessibility resources
            computedResources = defaultPreferences.Where(r => r.Disabled).ToList();

            var disputedResources = defaultPreferences.Where(r => !r.Disabled);

            //Enabled resources
            foreach (AccessibilityResource res in disputedResources)
            {
                var newRes = res.DeepClone();
                try
                {
                    var userPref = cookiePreferences.Where(p => p.Label == newRes.Label).SingleOrDefault();
                    var defaultSelDisabled = newRes.Selections.Where(s => s.Code == userPref.SelectedCode).SingleOrDefault();
                    var selected = userPref.SelectedCode;
                    if (!defaultSelDisabled.Disabled)
                    {
                        newRes.SelectedCode = userPref.SelectedCode;
                    }
                }
                catch (Exception e) when (
                    e is ArgumentNullException ||
                    e is InvalidOperationException ||
                    e is NullReferenceException)
                {
                    //There was a mismatch between the user's supplied preferences and the allowed values, 
                    //or there was duplidate data
                    //Use the default which is already set
                    logger.LogInformation(e.ToString());
                }

                computedResources.Add(newRes);
            }

            return computedResources;
        }

        private List<AccessibilityResourceGroup> SetAccessibilityFromCookie(AccessibilityResourceGroup[] cookiePreferences, ImmutableArray<AccessibilityResourceGroup> defaultPreferences)
        {
            List<AccessibilityResourceGroup> resourceGroups = new List<AccessibilityResourceGroup>();
            foreach (AccessibilityResourceGroup group in defaultPreferences)
            {
                ImmutableArray<AccessibilityResource> cookieResources;
                ImmutableArray<AccessibilityResource> computedResources;
                try
                {
                    cookieResources = cookiePreferences.Where(g => g.Order == group.Order).First().AccessibilityResources;
                    computedResources = SetResourceValuesFromCookie(cookieResources, group.AccessibilityResources)
                        .OrderBy(r => r.Order)
                        .OrderBy(r => r.Disabled)
                        .ToImmutableArray();
                }
                catch(Exception e)
                {
                    //Fall back to the defaults if there are no user preferences for the group
                    if(e is ArgumentNullException || e is InvalidOperationException)
                    {
                        logger.LogDebug($"Cookie does not contain user accessibility preferences for {group.Label} group");
                    }
                    else
                    {
                        throw;
                    }
                    computedResources = group.AccessibilityResources;
                }

                resourceGroups.Add(new AccessibilityResourceGroup(
                    label: group.Label,
                    order: group.Order,
                    accessibilityResources: computedResources
                    ));
            }
            return resourceGroups;
        }

        /// <summary>
        /// Converts a base64 encoded, serialized JSON string to an array of AccessibilityResourceViewModels
        /// </summary>
        /// <param name="cookieValue"></param>
        /// <returns></returns>
        private AccessibilityResourceGroup[] DecodeCookie(string cookieValue)
        {
            try
            {
                byte[] data = Convert.FromBase64String(cookieValue);
                cookieValue = Encoding.UTF8.GetString(data);
                AccessibilityResourceGroup[] cookiePreferences = JsonConvert.DeserializeObject<AccessibilityResourceGroup[]>(cookieValue);
                return cookiePreferences;
            }
            catch (Exception e)
            {
                logger.LogInformation("Unable to deserialize user accessibility options from cookie. Reason: "
                    + e.Message);
                return null;
            }
        }

        /// <returns>
        /// An ItemViewModel instance, or null if no item exists with
        /// the given combination of bankKey and itemKey.
        /// </returns>
        public ItemViewModel GetItemViewModel(int bankKey, int itemKey, string[] iSAAPCodes,
            string cookieValue)
        {
            AccessibilityResourceGroup[] cookiePreferences = null;
            var itemDigest = GetItemDigest(bankKey, itemKey);
            var itemCardViewModel = GetItemCardViewModel(bankKey, itemKey);
            if (itemDigest == null || itemCardViewModel == null)
            {
                return null;
            }

            var aboutItem = new AboutItemViewModel(itemDigest.Rubrics, itemCardViewModel);

            if (iSAAPCodes.Length == 0)
            {
                cookiePreferences = DecodeCookie(cookieValue);
            }

            var accResources = itemDigest.AccessibilityResourceGroups.SetIsaap(iSAAPCodes);
            if ((cookiePreferences != null) && (iSAAPCodes.Length == 0))
            {
                accResources = SetAccessibilityFromCookie(cookiePreferences, accResources).ToImmutableArray();
            }

            return new ItemViewModel(
                            itemViewerServiceUrl: GetItemViewerUrl(itemDigest),
                            accessibilityCookieName: AppSettings.SettingsConfig.AccessibilityCookie,
                            aboutItemVM: aboutItem,
                            accResourceGroups: accResources,
                            moreLikeThisVM: GetMoreLikeThis(itemDigest)
                        );
        }

        /// <summary>
        /// Gets up to 3 items same grade, grade above, and grade below. All items 
        /// </summary>
        /// <param name="grade"></param>
        /// <param name="subject"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public MoreLikeThisViewModel GetMoreLikeThis(ItemDigest itemDigest)
        {
            var subjectCode = itemDigest.Subject.Code;
            var claimCode = itemDigest.Claim?.Code;
            var grade = itemDigest.Grade;
            var itemKey = itemDigest.ItemKey;
            var bankKey = itemDigest.BankKey;

            var matchingSubjectClaim = context.ItemCards.Where(i => i.SubjectCode == subjectCode && i.ClaimCode == claimCode);
            // TODO: get Take() value from appsettings
            int numExpected = 3;

            var comparer = new MoreLikeThisComparer(subjectCode, claimCode);
            GradeLevels gradeBelow = grade.GradeBelow(), gradeAbove = grade.GradeAbove();

            var cardsGradeBelow = context.ItemCards
                .Where(i => i.Grade == gradeBelow)
                .OrderBy(i => i, comparer)
                .Take(numExpected);

            var cardsSameGrade = context.ItemCards
                .Where(i => i.Grade == grade && i.ItemKey != itemKey)
                .OrderBy(i => i, comparer)
                .Take(numExpected);

            var cardsGradeAbove = context.ItemCards
                .Where(i => i.Grade == gradeAbove)
                .OrderBy(i => i, comparer)
                .Take(numExpected);

            var moreLikeThisVM = new MoreLikeThisViewModel(
                cardsGradeBelow.ToImmutableArray(),
                cardsSameGrade.ToImmutableArray(),
                cardsGradeAbove.ToImmutableArray()
                );

            return moreLikeThisVM;
        }

    }

}
