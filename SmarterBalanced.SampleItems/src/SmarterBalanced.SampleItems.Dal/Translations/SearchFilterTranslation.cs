using SmarterBalanced.SampleItems.Dal.Configurations.Models;
using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace SmarterBalanced.SampleItems.Dal.Translations
{
    public static class SearchFilterTranslation
    {
        public static FilterSearch ToSearchFilter(
            List<FilterCategorySettings> settings,
            ImmutableArray<Subject> subjects,
            ImmutableArray<Claim> claims,
            ImmutableArray<InteractionType> interactionTypes)
        {
            var subjectFilter = ToFilterCategory(subjects, settings
                .FirstOrDefault(s => s.Code.ToLower() == "subject"));
            var claimFilter = ToFilterCategory(claims, settings
                .FirstOrDefault(s => s.Code.ToLower() == "claim"));
            var interactionFilter = ToFilterCategory(interactionTypes, settings
                .FirstOrDefault(s => s.Code.ToLower() == "interactiontype"));
            var gradesFilter = ToFilterCategory(GradeLevelsUtils.allGrades, settings
                .FirstOrDefault(s => s.Code.ToLower() == "grade"));
            var technologyType = TechnologyTypeToFilterCategory(
                settings.FirstOrDefault(s => s.Code.ToLower() == "performancecat"));

            var filterSearch = new FilterSearch
            {
                Subjects = subjectFilter,
                Claims = claimFilter,
                InteractionTypes = interactionFilter,
                Grades = gradesFilter,
                TechnologyTypes = technologyType
            };

            return filterSearch;
        }

        public static FilterCategory<T> ToFilterCategory<T>(this ImmutableArray<T> options, FilterCategorySettings settings)
        {
            return new FilterCategory<T>
            {
                Label = settings.Label,
                Code = settings.Code,
                FilterOptions = options,
                HelpText = settings.HelpText
            };
        }

        public static FilterCategory<TechnologyType> TechnologyTypeToFilterCategory(FilterCategorySettings settings)
        {
            var performance = new TechnologyType { Code = "Performance", Label = "Performance Only" };
            var cat = new TechnologyType { Code = "CAT", Label = "CAT Only" };
            var options = ImmutableArray.Create(performance, cat);

            return ToFilterCategory(options, settings);
        }

    }

}
