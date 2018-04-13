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
    label: "Subject"
  };

  const claims: BasicFilterCategoryModel = {
    ...ItemSearch.filterSearchToCategory(itemSearchFilter.claims, searchAPI),
    optionType: OptionTypeModel.AdvFilter,
    label: "Claims",
    emptyOptionsText:"Select a Subject first."
  };

  const targets: BasicFilterCategoryModel = {
    ...ItemSearch.filterSearchToCategory(itemSearchFilter.targets, searchAPI),
    optionType: OptionTypeModel.AdvFilter,
    label: "Targets",
    emptyOptionsText: "Select a Subject and Claim first."
  };

  const searchItem: BasicFilterCategoryModel = {
    disabled: false,
    label: "Enter Item ID",
    filterOptions: [],
    helpText: "Item id helptext",
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

  const claims = {
    ...ItemSearch.filterSearchToCategory(itemSearchFilter.claims, searchAPI),
    isMultiSelect: true,
    disabled: false,
    displayAllButton: true
  };

  const subjects = {
    ...ItemSearch.filterSearchToCategory(itemSearchFilter.subjects, searchAPI),
    isMultiSelect: true,
    disabled: false,
    displayAllButton: true
  };

  const interactions = {
    ...ItemSearch.filterSearchToCategory(
      itemSearchFilter.interactionTypes,
      searchAPI
    ),
    isMultiSelect: true,
    disabled: false,
    displayAllButton: true
  };

  const techTypes = {
    ...ItemSearch.filterSearchToCategory(
      itemSearchFilter.technologyTypes,
      searchAPI
    ),
    isMultiSelect: false,
    disabled: false,
    displayAllButton: true
  };

  const targets = {
    ...ItemSearch.filterSearchToCategory(itemSearchFilter.targets, searchAPI),
    isMultiSelect: true,
    disabled: false,
    displayAllButton: true
  };

  const calculator = {
    ...ItemSearch.filterSearchToCategory(
      itemSearchFilter.calculator,
      searchAPI
    ),
    isMultiSelect: false,
    disabled: false,
    displayAllButton: true
  };

  return [
    subjects,
    claims,
    interactions,
    techTypes,
    targets,
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
