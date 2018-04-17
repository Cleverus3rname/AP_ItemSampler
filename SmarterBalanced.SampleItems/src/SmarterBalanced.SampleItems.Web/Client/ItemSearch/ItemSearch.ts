import {
    SearchAPIParamsModel,
    AdvancedFilterCategoryModel,
    ItemSearch,
    ItemsSearchFilterModel,
    ItemsSearchModel,
    ItemCardModel,
    getRequest,
    GradeLevels,
    OptionTypeModel,
    BasicFilterCategoryModel,
    FilterType
} from "@osu-cass/sb-components";

export const itemSearchClient = getRequest<ItemCardModel[]>(
    "/BrowseItems/search"
);

export const itemsSearchFilterClient = getRequest<ItemsSearchFilterModel>(
    "/BrowseItems/FilterSearchModel"
);

export function getBasicFilterCategories(
    itemSearchFilter: ItemsSearchFilterModel,
    searchAPI: SearchAPIParamsModel
): BasicFilterCategoryModel[] {
    itemSearchFilter.grades.filterOptions = [
        GradeLevels.Grade3,
        GradeLevels.Grade4,
        GradeLevels.Grade5,
        GradeLevels.Grade6,
        GradeLevels.Grade7,
        GradeLevels.Grade8,
        GradeLevels.High
    ];

    const grades: BasicFilterCategoryModel = {
        ...ItemSearch.filterSearchToCategory(itemSearchFilter.grades, searchAPI),
        optionType: OptionTypeModel.DropDown,
        label: "Grade"
    };

    const subjects: BasicFilterCategoryModel = {
        ...ItemSearch.filterSearchToCategory(itemSearchFilter.subjects, searchAPI),
        optionType: OptionTypeModel.AdvFilter,
        label: "Subject",
        isMultiSelect: true
    };

    const claims: BasicFilterCategoryModel = {
        ...ItemSearch.filterSearchToCategory(itemSearchFilter.claims, searchAPI),
        optionType: OptionTypeModel.AdvFilter,
        label: "Claims",
        emptyOptionsText: "Select a Subject first.",
        isMultiSelect: true
    };

  const targets: BasicFilterCategoryModel = {
    ...ItemSearch.filterSearchToCategory(itemSearchFilter.targets, searchAPI),
    optionType: OptionTypeModel.AdvFilter,
    label: "Targets",
      emptyOptionsText: "Select a Subject and Claim first.",
      isMultiSelect: true
  };

    const searchItem: BasicFilterCategoryModel = {
        disabled: false,
        label: "Enter Item ID",
        filterOptions: [],
        code: FilterType.SearchItemId,
        optionType: OptionTypeModel.inputBox,
        placeholderText: "Item ID \#"
    };

    return [grades, subjects, claims, targets, searchItem];
}

export function getAdvancedFilterCategories(
    itemSearchFilter: ItemsSearchFilterModel,
    searchAPI: SearchAPIParamsModel
): AdvancedFilterCategoryModel[] {


    const interactions = {
        ...ItemSearch.filterSearchToCategory(
            itemSearchFilter.interactionTypes,
            searchAPI
        ),
        isMultiSelect: true,
        disabled: false,
        displayAllButton: true
    };

    const techTypes: AdvancedFilterCategoryModel = {
        ...ItemSearch.filterSearchToCategory(
            itemSearchFilter.technologyTypes,
            searchAPI
        ),
        isMultiSelect: false,
        disabled: false,
        displayAllButton: true

    };


    const calculator: AdvancedFilterCategoryModel = {
        ...ItemSearch.filterSearchToCategory(
            itemSearchFilter.calculator,
            searchAPI
        ),
        isMultiSelect: false,
        disabled: false,
        displayAllButton: true
    };

    return [
        interactions,
        techTypes,
        calculator
    ];
}

export function getItemSearchModel(
    itemSearchFilter: ItemsSearchFilterModel
): ItemsSearchModel {
    return {
        claims: itemSearchFilter.claims.filterOptions,
        subjects: itemSearchFilter.subjects.filterOptions,
        interactionTypes: itemSearchFilter.interactionTypes.filterOptions,
        targets: itemSearchFilter.targets.filterOptions
    };
}
