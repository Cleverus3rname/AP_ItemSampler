using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using SmarterBalanced.SampleItems.Dal.Providers.Models;

namespace SmarterBalanced.SampleItems.Core.AccessibilityTesting
{
    public class BriefAccessibilityResource
    {
        public string ResourceCode { get; }
        public string Label { get; }
        public bool Disabled { get; }

        public BriefAccessibilityResource(
            string resourceCode,
            string label,
            bool disabled)
        {
            ResourceCode = resourceCode;
            Label = label;
            Disabled = disabled;
        }

        public static BriefAccessibilityResource Create(
            string resourceCode = "",
            string label = "",
            bool disabled = false)
        {
            return new BriefAccessibilityResource(
                resourceCode: resourceCode,
                label: label,
                disabled: disabled);
        }
    }
}