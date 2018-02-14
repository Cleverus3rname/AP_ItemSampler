using SmarterBalanced.SampleItems.Core.Repos.Models;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace SmarterBalanced.SampleItems.Core.Repos
{
    public interface ISampleItemsSearchRepo
    {
        IList<ItemCardViewModel> GetItemCards();
        IList<ItemCardViewModel> GetItemCards(ItemsSearchParams parms);
        ItemsSearchViewModel GetItemsSearchViewModel();
        IList<SampleItemViewModel> GetSampleItemViewModels(string baseUrl);
        FilterSearch GetFilterSearch();
    }
}
