﻿using System.Collections.Generic;

namespace SmarterBalanced.SampleItems.Dal.Providers.Models
{
    public sealed class AccessibilityResource
    {
        public string Code { get; set; }

        public int Order { get; set; }

        public string DefaultSelection { get; set; }

        public string SelectedCode { get; set; }

        public List<AccessibilitySelection> Selections { get; set; }

        public string Label { get; set; }

        public string Description { get; set; }

        public bool Disabled { get; set; }

        public string ResourceType { get; set; }
        public string ResourceTypeLabel { get; set; }
        public AccessibilityResource DeepClone()
        {
            List<AccessibilitySelection> selections = new List<AccessibilitySelection>();
            foreach(AccessibilitySelection selection in Selections)
            {
                selections.Add(selection.Clone());
            }
            return new AccessibilityResource
            {
                Code = Code,
                Order = Order,
                DefaultSelection = DefaultSelection,
                SelectedCode = SelectedCode,
                Selections = selections,
                Label = Label,
                Description = Description,
                Disabled = Disabled,
                ResourceType = ResourceType,
                ResourceTypeLabel = ResourceTypeLabel
            };
        }
    }
}
