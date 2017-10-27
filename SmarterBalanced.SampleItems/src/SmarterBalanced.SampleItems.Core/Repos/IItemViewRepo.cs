using System;
using System.Collections.Generic;
using SmarterBalanced.SampleItems.Core.Repos.Models;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Immutable;

namespace SmarterBalanced.SampleItems.Core.Repos
{
    public interface IItemViewRepo
    {
        ItemViewModel GetItemViewModel(
            int bankKey,
            int itemKey,
            string[] iSAAPCodes,
            Dictionary<string, string> cookieValue);

        MoreLikeThisViewModel GetMoreLikeThis(SampleItem sampleItem);

        AboutThisItemViewModel GetAboutThisItemViewModel(SampleItem sampleItem);

        ImmutableArray<AccessibilityResourceGroup> GetAccessibilityResourceGroup(int bankKey, int itemKey);

        ImmutableArray<AccessibilityResourceGroup> GetAccessibilityResourceGroup(int bankKey, int itemKey, string[] iSAAPCodes);

        Task<Stream> GetItemBrailleZip(int itemBank, int itemKey, string brailleCode);

        string GenerateBrailleZipName(int itemId, string brailleCode);

        AboutThisItemViewModel GetAboutThisItemViewModel(int itemBank, int itemKey);

    }
}
