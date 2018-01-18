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
        public string SelectionCode { get; }
        public string Label { get; }
        public bool Disabled { get; }

        public BriefAccessibilityResource(
            string resourceCode,
            string selectionCode,
            string label,
            bool disabled)
        {
            ResourceCode = resourceCode;
            SelectionCode = selectionCode;
            Label = label;
            Disabled = disabled;
        }

        public static BriefAccessibilityResource Create(
            string resourceCode = "",
            string selectionCode = "",
            string label = "",
            bool disabled = false)
        {
            return new BriefAccessibilityResource(
                resourceCode: resourceCode,
                selectionCode: selectionCode,
                label: label,
                disabled: disabled);
        }
    }
}