using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmarterBalanced.SampleItems.Core.Repos.Models
{
    public class ItemsSearchViewModel
    {
        public IList<InteractionType> InteractionTypes { get; set; }
        public IList<Subject> Subjects { get; set; }
        public IList<Target> Targets { get; set; }
        public IList<Claim> Claims { get; set; }
    }
}
