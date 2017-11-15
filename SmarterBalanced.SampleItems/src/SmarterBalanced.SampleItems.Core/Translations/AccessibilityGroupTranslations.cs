using Microsoft.AspNetCore.Mvc.Rendering;
using SmarterBalanced.SampleItems.Core.Repos.Models;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SmarterBalanced.SampleItems.Core.Translations
{
    public static class AccessibilityGroupTranslations
    {

        public static ImmutableArray<AccessibilityResourceGroup> ApplyIsaapPreferences(
            this ImmutableArray<AccessibilityResourceGroup> groups,
            string[] isaap)
        {
            if (groups == null) throw new ArgumentNullException(nameof(groups));
            if (isaap == null) throw new ArgumentNullException(nameof(isaap));

            if (isaap.Length != 0)
            {
                var isaapGroups = groups
                    .Select(g => g.WithResources(g.AccessibilityResources
                        .Select(r => r.ApplyIsaapToResource(isaap))
                        .ToImmutableArray()))
                    .ToImmutableArray();

                return isaapGroups;
            }
            else
            {
                return groups;
            }
        }

        public static ImmutableArray<AccessibilityResourceGroup> ApplyCookiePreferences(
            this ImmutableArray<AccessibilityResourceGroup> groups,
            Dictionary<string, string> cookie)
        {
            if (groups == null) throw new ArgumentNullException(nameof(groups));
            if (cookie == null) throw new ArgumentNullException(nameof(cookie));

            if (cookie.Count != 0)
            {
                var cookieGroups = groups
                    .Select(g => g.WithResources(g.AccessibilityResources
                        .Select(r => r.ApplyCookie(cookie))
                        .ToImmutableArray()))
                    .ToImmutableArray();

                return cookieGroups;
            }
            else
            {
                return groups;
            }
        }

        public static ImmutableArray<AccessibilityResourceGroup> ApplyPreferences(
            this ImmutableArray<AccessibilityResourceGroup> groups,
            string[] isaap = default(string[]),
            Dictionary<string, string> cookie = default(Dictionary<string, string>))
        {
            if (groups == null) throw new ArgumentNullException(nameof(groups));

            if (isaap.Length != 0)
            {
                return ApplyIsaapPreferences(groups, isaap);
            }
            else if (cookie.Count != 0)
            {
                return ApplyCookiePreferences(groups, cookie);
            }
            else
            {
                return groups;
            }
        }

        private static AccessibilityResource ApplyIsaapToResource(this AccessibilityResource resource, string[] isaap)
        {
            var issapSelection = resource.Selections.FirstOrDefault(sel => isaap.Contains(sel.SelectionCode));
            if (issapSelection == null)
            {
                return resource;
            }

            return resource.ApplySelectedCode(issapSelection.SelectionCode);
        }

        private static AccessibilityResource ApplySelectedCode(this AccessibilityResource resource, string code)
        {
            var newSelection = resource.Selections.FirstOrDefault(sel => sel.SelectionCode.Equals(code));
            if (newSelection == null || newSelection.Disabled)
            {
                return resource;
            }

            var newResource = resource.WithCurrentSelection(newSelection.SelectionCode);
            return newResource;
        }

        private static AccessibilityResource ApplyCookie(this AccessibilityResource resource, Dictionary<string, string> cookie)
        {
            string newSelectedCode;
            if (cookie.TryGetValue(resource.ResourceCode, out newSelectedCode))
            {
                return resource.ApplySelectedCode(newSelectedCode);
            }

            return resource;
        }
    }

}
