﻿import {
    SearchAPIParamsModel,
    AdvancedFilterCategoryModel,
    ItemSearch,
    ItemsSearchFilterModel,
    ItemsSearchModel,
    ItemCardModel,
    get,

    GradeLevels
} from "@osu-cass/sb-components";

export const ItemsSearchClient = (params: SearchAPIParamsModel) =>
    get<ItemCardModel[]>("/BrowseItems/search", params);

export const ItemsViewModelClient = () =>
    get<ItemsSearchFilterModel>("/BrowseItems/FilterSearchModel");


export function getFilterCategories(itemSearchFilter: ItemsSearchFilterModel, searchAPI: SearchAPIParamsModel): AdvancedFilterCategoryModel[] {
    const gradeOptions = [GradeLevels.Elementary, GradeLevels.Middle, GradeLevels.High];
    itemSearchFilter.grades.filterOptions = gradeOptions;
    const claims = { ...ItemSearch.filterSearchToCategory(itemSearchFilter.claims, searchAPI), isMultiSelect: true, disabled: false, displayAllButton: true };
    const subjects = { ...ItemSearch.filterSearchToCategory(itemSearchFilter.subjects, searchAPI), isMultiSelect: true, disabled: false, displayAllButton: true };
    const interactions = { ...ItemSearch.filterSearchToCategory(itemSearchFilter.interactionTypes, searchAPI), isMultiSelect: true, disabled: false, displayAllButton: true };
    const grades = { ...ItemSearch.filterSearchToCategory(itemSearchFilter.grades, searchAPI), isMultiSelect: true, disabled: false, displayAllButton: true };
    const techTypes = { ...ItemSearch.filterSearchToCategory(itemSearchFilter.technologyTypes, searchAPI), isMultiSelect: false, disabled: false, displayAllButton: true };


    //TODO: add rest
    let advancedFilters: AdvancedFilterCategoryModel[] = [
        grades, subjects, claims, interactions, techTypes
    ];

    return advancedFilters;


}
//TODO:translate the itemsearchfiltermodel to a flat itemsearchmodel
export function getItemSearchModel(itemSearchFilter: ItemsSearchFilterModel): ItemsSearchModel {
    const itemSearch: ItemsSearchModel = {
        claims: itemSearchFilter.claims.filterOptions,
        subjects: itemSearchFilter.subjects.filterOptions,
        interactionTypes: itemSearchFilter.interactionTypes.filterOptions
    };
    return itemSearch;

}
