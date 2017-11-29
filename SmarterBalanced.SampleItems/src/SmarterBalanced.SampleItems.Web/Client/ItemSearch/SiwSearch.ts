import {
    SearchAPIParamsModel,
    AdvancedFilterCategoryModel,
    ItemSearch,
    ItemsSearchFilterModel,
    ItemsSearchModel,
    ItemCardModel,
    get
} from "@osu-cass/sb-components";

export const ItemsSearchClient = (params: SearchAPIParamsModel) =>
    get<ItemCardModel[]>("/BrowseItems/search", params);

export const ItemsViewModelClient = () =>
    get<ItemsSearchFilterModel>("/BrowseItems/FilterSearchModel");


export function getFilterCategories(itemSearchFilter: ItemsSearchFilterModel, searchAPI: SearchAPIParamsModel): AdvancedFilterCategoryModel[] {
    const claims = { ...ItemSearch.filterSearchToCategory(itemSearchFilter.claims, searchAPI), isMultiSelect: true, disabled: false, displayAllButton: true };
    const subjects = { ...ItemSearch.filterSearchToCategory(itemSearchFilter.subjects, searchAPI), isMultiSelect: true, disabled: false, displayAllButton: true };
    const interactions = { ...ItemSearch.filterSearchToCategory(itemSearchFilter.interactionTypes, searchAPI), isMultiSelect: true, disabled: false, displayAllButton: true };
    //TODO: add rest
    let advancedFilters: AdvancedFilterCategoryModel[] = [
        subjects, claims, interactions
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
