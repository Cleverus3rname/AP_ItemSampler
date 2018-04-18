using System.Linq;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using SmarterBalanced.SampleItems.Dal.Providers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Immutable;

namespace SmarterBalanced.SampleItems.Core.Repos.Models
{
    public class AboutItemsRepo : ItemViewRepo, IAboutItemsRepo
    {
        private readonly SampleItemsContext context;
        private readonly ILogger logger;

        public AboutItemsRepo(SampleItemsContext context, ILoggerFactory loggerFactory): base(context, loggerFactory)
        {
            this.context = context;
            logger = loggerFactory.CreateLogger<AboutItemsRepo>();
        }

        /// <summary>
        /// Constructs and AboutItemsViewModel with an item that matches 
        /// the given interactionType.
        /// </summary>
        public AboutItemsViewModel GetAboutItemsViewModel(string interactionTypeCode = "")
        {
            var interactionTypes = context.AboutInteractionTypes;

            if (string.IsNullOrWhiteSpace(interactionTypeCode))
            {
                interactionTypeCode  = interactionTypes.FirstOrDefault()?.Code;
            }

            var items = context.SampleItems
               .Where(i => i.Grade != GradeLevels.NA && i.InteractionType != null)
               .OrderBy(i => (int)i.Grade);

            SampleItem sampleItem = items.FirstOrDefault(item => item.InteractionTypeSubCat == interactionTypeCode)
                ?? items.FirstOrDefault(item => interactionTypeCode == item.InteractionType.Code)
                ?? items.FirstOrDefault(item => interactionTypeCode.Contains(item.InteractionType.Code));

            string itemURL = GetItemViewerUrlSingleItem(sampleItem);

            AboutThisItemViewModel aboutThisItemViewModel = GetAboutThisItem(sampleItem);
            AboutItemsViewModel model = new AboutItemsViewModel(
                interactionTypes: interactionTypes,
                itemUrl: itemURL,
                selectedCode: interactionTypeCode,
                aboutThisItemViewModel: aboutThisItemViewModel);

            return model;
        }

        public AboutThisItemViewModel GetAboutThisItem(int itemBank, int itemKey)
        {
            var aboutThisItemViewModel = context.AboutAllItems.FirstOrDefault(item =>
                item.ItemCardViewModel?.BankKey == itemBank
                && item.ItemCardViewModel?.ItemKey == itemKey);

            if (aboutThisItemViewModel == null)
            {
                throw new Exception($"invalid request for {itemBank}-{itemKey}");
            }

            return aboutThisItemViewModel;
        }

        private AboutThisItemViewModel GetAboutThisItem(SampleItem sampleItem)
        {
            if (sampleItem == null)
            {
                return null;
            }

            var aboutThisItemViewModel = context.AboutAllItems.FirstOrDefault(item =>
                item.ItemCardViewModel?.BankKey == sampleItem.BankKey
                && item.ItemCardViewModel?.ItemKey == sampleItem.ItemKey);

            return aboutThisItemViewModel;
        }

        /// <summary>
        /// Constructs an ItemViewerService URL for a single item 
        /// </summary>
        private string GetItemViewerUrlSingleItem(SampleItem sampleItem)
        {
            if (sampleItem == null)
            {
                return null;
            }

            string baseUrl = context.AppSettings.SettingsConfig.ItemViewerServiceURL;
            return $"{baseUrl}/items?ids={sampleItem.ToString()}&isaap=TDS_T0";
        }
    }

}
