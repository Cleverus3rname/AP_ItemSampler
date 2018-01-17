using System;
using System.Collections.Generic;
using System.Text;

namespace SmarterBalanced.SampleItems.Dal.Providers.Models
{
    public class FilterSearch
    {
        public FilterCategory<InteractionType> InteractionTypes { get; set; }
        public FilterCategory<Claim> Claims { get; set; }
        public FilterCategory<Subject> Subjects { get; set; }
        public FilterCategory<Target> Targets { get; set; }
        public FilterCategory<GradeLevels> Grades { get; set; }
        public FilterCategory<TechnologyType> TechnologyTypes { get; set; }
        public FilterCategory<TechnologyType> Calculator { get; set; }

    }
}
