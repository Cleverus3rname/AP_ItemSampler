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
            ImmutableArray<InteractionType> interactionTypes,
            ImmutableArray<Target> targets)
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
                settings.FirstOrDefault(s => s.Code.ToLower() == "technologytype"));
            var targetFilter = ToFilterCategory(targets, settings.FirstOrDefault(s => s.Code.ToLower() == "target"));
            var calculatorFilter = CalculatorToFilterCategory(settings.FirstOrDefault(s => s.Code.ToLower() == "calculator"));

            var filterSearch = new FilterSearch
            {
                Subjects = subjectFilter,
                Claims = claimFilter,
                InteractionTypes = interactionFilter,
                Grades = gradesFilter,
                TechnologyTypes = technologyType,
                Calculator = calculatorFilter,
                Targets = targetFilter
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
            var performance = new TechnologyType { Code = "Performance", Label = "Yes" };
            var cat = new TechnologyType { Code = "CAT", Label = "No" };
            var options = ImmutableArray.Create(performance, cat);

            return ToFilterCategory(options, settings);
        }

        public static FilterCategory<TechnologyType> CalculatorToFilterCategory(FilterCategorySettings settings)
        {
            var calc = new TechnologyType { Code = "true", Label = "Allowed" };
            var calcOff = new TechnologyType { Code = "false", Label = "Not Allowed" };

            return ToFilterCategory(ImmutableArray.Create(calc, calcOff), settings);
        }

    }

}
