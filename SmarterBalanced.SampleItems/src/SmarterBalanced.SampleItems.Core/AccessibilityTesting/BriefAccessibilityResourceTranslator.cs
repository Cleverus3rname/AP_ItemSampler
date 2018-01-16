using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using SmarterBalanced.SampleItems.Dal.Providers.Models;

namespace SmarterBalanced.SampleItems.Core.AccessibilityTesting
{
    public static class BriefAccessibilityResourceTranslator
    {
        public static BriefAccessibilityResource ToBriefAccessibilityResource(this AccessibilityResource accessibilityResource)
        {
            var resource = BriefAccessibilityResource.Create(
                resourceCode: accessibilityResource.ResourceCode,
                label: accessibilityResource.Label,
                disabled: accessibilityResource.Disabled);
            return resource;
        }
    }
}