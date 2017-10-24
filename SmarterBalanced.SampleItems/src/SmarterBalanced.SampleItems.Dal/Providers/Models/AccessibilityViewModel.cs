using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Immutable;

namespace SmarterBalanced.SampleItems.Dal.Providers.Models
{
    public class AccessibilityViewModel
    {
        public ImmutableArray<AccessibilityFamily> Families { get; }
        public ImmutableArray<AccessibilityResource> Resources { get; }
        public ImmutableArray<AccessibilitySelection> Selections { get; }

        public AccessibilityViewModel(
            ImmutableArray<AccessibilityFamily> families,
            ImmutableArray<AccessibilityResource> resources,
            ImmutableArray<AccessibilitySelection> selections)
        {
            Families = families;
            Resources = resources;
            Selections = selections;
        }

        public static AccessibilityViewModel Create(
           ImmutableArray<AccessibilityFamily> families = new ImmutableArray<AccessibilityFamily>(),
           ImmutableArray<AccessibilityResource> resources = new ImmutableArray<AccessibilityResource>(),
           ImmutableArray<AccessibilitySelection> selections = new ImmutableArray<AccessibilitySelection>())
        {
            return new AccessibilityViewModel(
                families: families,
                resources: resources,
                selections: selections
            );
        }
    }
}
