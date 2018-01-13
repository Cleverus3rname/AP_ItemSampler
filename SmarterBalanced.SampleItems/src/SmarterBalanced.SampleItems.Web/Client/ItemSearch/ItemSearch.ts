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
  BasicFilterCategoryModel
} from "@osu-cass/sb-components";

export const ItemsSearchClient = (params: SearchAPIParamsModel) =>
  getRequest<ItemCardModel[]>("/BrowseItems/search", params);

export const ItemsViewModelClient = () =>
  getRequest<ItemsSearchFilterModel>("/BrowseItems/FilterSearchModel");

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
    const grades = {
        ...ItemSearch.filterSearchToCategory(itemSearchFilter.grades, searchAPI),
        type: OptionTypeModel.DropDown
    };
    const subjects = {
        ...ItemSearch.filterSearchToCategory(itemSearchFilter.subjects, searchAPI),
        type: OptionTypeModel.DropDown
    };

    let basicFilters: BasicFilterCategoryModel[] = [
        grades, subjects
    ];
    return basicFilters;
}

export function getAdvancedFilterCategories(
  itemSearchFilter: ItemsSearchFilterModel,
  searchAPI: SearchAPIParamsModel
): AdvancedFilterCategoryModel[] {
    itemSearchFilter.grades.filterOptions = [
        GradeLevels.Elementary,
        GradeLevels.Middle,
        GradeLevels.High
  ];;

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

  const grades = {
    ...ItemSearch.filterSearchToCategory(itemSearchFilter.grades, searchAPI),
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
    grades,
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
