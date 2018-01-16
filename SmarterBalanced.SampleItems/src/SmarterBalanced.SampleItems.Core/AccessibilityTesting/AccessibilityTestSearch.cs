using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmarterBalanced.SampleItems.Core.AccessibilityTesting
{
    public class AccessibilityTestSearch
    {
        public string Resource { get; }
        public bool State { get; }

        public AccessibilityTestSearch(
            string accessibilityResource,
            bool enabledState)
        {
            Resource = accessibilityResource;
            State = enabledState;
        }
    }
}