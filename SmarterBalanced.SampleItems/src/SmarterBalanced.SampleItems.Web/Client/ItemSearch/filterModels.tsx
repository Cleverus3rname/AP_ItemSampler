import { OptionType, AdvancedFilterCategory } from '@osu-cass/react-advanced-filter'

export const mockAdvancedFilterCategories: AdvancedFilterCategory[] = [
    {//grade filter
        "disabled": false,
        "isMultiSelect": true,
        "helpText": "Grade HelpText here.",
        "label": "Grades",
        "filterOptions": [
            {
                "label": "Elementary",
                "key": "7",
                "isSelected": false
            },
            {
                "label": "Middle",
                "key": "56",
                "isSelected": false
            },
            {
                "label": "High",
                "key": "960",
                "isSelected": false
            }
        ],
        "displayAllButton":true
    },
    {//Subjects filter
        "disabled": false,
        "isMultiSelect": true,
        "label": "Subjects",
        "helpText": "Subjects HelpText here.",
        "filterOptions": [
            {
                "label": "Mathematics",
                "key": "MATH",
                "isSelected": false
            },
            {
                "label": "English",
                "key": "ELA",
                "isSelected": false
            }
        ],
        "displayAllButton": true
    },
    {//Claims
        "disabled": false,
        "isMultiSelect": true,
        "label": "Claim",
        "helpText": "Claim HelpText here.",
        "filterOptions": [
        ],
        "displayAllButton": true
    },
    {//Targets
        "disabled": false,
        "isMultiSelect": true,
        "label": "Target",
        "helpText": "Target HelpText here.",
        "filterOptions": [
        ],
        "displayAllButton": true
    },
    {//Calculator on/off
        "disabled": false,
        "isMultiSelect": true,
        "label": "Calculator",
        "helpText": "Calculator HelpText here.",
        "filterOptions": [
        ],
        "displayAllButton": true
    }

];
