using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace SmarterBalanced.SampleItems.Dal.Providers.Models
{
    public class FilterCategory<T>
    {
        public string Code { get; set; }
        public string Label { get; set; }
        public string HelpText { get; set; }
        public ImmutableArray<T> FilterOptions {get;set;}
    }
}
