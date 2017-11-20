import { OptionTypeModel, AdvancedFilterCategoryModel } from '@osu-cass/sb-components'

//todo remove all of this
export const mockAdvancedFilterCategories: AdvancedFilterCategoryModel[] = [
    {
        "disabled": false,
        "isMultiSelect": true,
        "helpText": "Grade HelpText here.",
        "label": "Grades",
        "code": "Grade",
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
        "displayAllButton": true
    },
    {//Subjects filter
        "disabled": false,
        "isMultiSelect": true,
        "label": "Subjects",
        "helpText": "Subjects HelpText here.",
        "code": "Subjects",

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
    }


];
