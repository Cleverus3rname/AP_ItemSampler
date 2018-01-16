using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;
using SmarterBalanced.SampleItems.Dal.Providers.Models;

namespace SmarterBalanced.SampleItems.Core.AccessibilityTesting
{
    public class AccessibilityFamilySearchResult
    {
        public ImmutableArray<string> Subjects { get; }
        public GradeLevels Grades { get; }
        public ImmutableArray<BriefAccessibilityResource> Resources { get; }
        public string ResourceList { get; }

        public AccessibilityFamilySearchResult(
            ImmutableArray<string> subjects,
            GradeLevels grades,
            ImmutableArray<BriefAccessibilityResource> resources,
            string resourceList)
        {
            Subjects = subjects;
            Grades = grades;
            Resources = resources;
            ResourceList = resourceList;
        }

        public static AccessibilityFamilySearchResult FromMergedFamily(MergedAccessibilityFamily mergedFamily)
        {
            var resources = mergedFamily.Resources.Where(r => r.Disabled == true).Select(r => r.ToBriefAccessibilityResource()).ToImmutableArray();
            var resourceList = String.Join(",", resources.Select(r => r.Label));
            return new AccessibilityFamilySearchResult(
                subjects: mergedFamily.Subjects,
                grades: mergedFamily.Grades,
                resources: resources,
                resourceList: resourceList);
        }
    }
}