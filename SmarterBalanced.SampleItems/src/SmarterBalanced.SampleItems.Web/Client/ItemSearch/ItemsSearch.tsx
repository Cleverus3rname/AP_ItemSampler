import '../Styles/search.less';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { RouteComponentProps } from 'react-router';
import { FilterISPComponent } from './filterISPComponent';
import { updateUrl, readUrl } from '../UrlHelper';

import {
    Resource,
    get,
    ItemCard,
    ItemCardModel,
    GradeLevels,
    SearchAPIParamsModel,
    ItemsSearchModel,
    itemPageLink,
    FilterOptionModel,
    AdvancedFilterContainer,
    AdvancedFilterCategoryModel,
    Filter,
    ItemSearch,
    ItemsSearchFilterModel,
    getResourceContent
} from '@osu-cass/sb-components';



export const ItemsSearchClient = (params: SearchAPIParamsModel) =>
    get<ItemCardModel[]>("/BrowseItems/search", params);

export const ItemsViewModelClient = () =>
    get<ItemsSearchFilterModel>("/BrowseItems/FilterSearchModel");



export interface Props extends RouteComponentProps<{}> {
    itemsSearchClient: (params: SearchAPIParamsModel) => Promise<ItemCardModel[]>;
    itemsViewModelClient: () => Promise<ItemsSearchFilterModel>;
}

export interface State {
    searchResults: Resource<ItemCardModel[]>;
    itemSearch: Resource<ItemsSearchModel>;
    currentFilter?: AdvancedFilterCategoryModel[];

}

export class ItemsSearchComponent extends React.Component<Props, State> {
    private searchModel: ItemsSearchModel | undefined = undefined;
    constructor(props: Props) {
        super(props);
        this.state = {
            searchResults: { kind: "loading" },
            itemSearch: { kind: "loading" },
        };

        this.props.itemsViewModelClient()
            .then(data => this.onFetchFilterModel(data))
            .catch(err => this.onError(err));


        this.props.itemsSearchClient({})
            .then((data) => this.onSearch(data))
            .catch((err) => this.onError(err));

    }
    onSearch(results: ItemCardModel[]) {
        this.setState({ searchResults: { kind: "success", content: results } });
    }

    onError(err: any) {
        this.setState({ searchResults: { kind: "failure" } });
    }

    getFilterCategories(itemSearchFilter: ItemsSearchFilterModel): AdvancedFilterCategoryModel[] {
        const claims = { ...ItemSearch.filterSearchToCategory(itemSearchFilter.claims), isMultiSelect: true, disabled: false, displayAllButton: true };
        const subjects = { ...ItemSearch.filterSearchToCategory(itemSearchFilter.subjects), isMultiSelect: true, disabled: false, displayAllButton: true };
        const interactions = { ...ItemSearch.filterSearchToCategory(itemSearchFilter.interactionTypes), isMultiSelect: true, disabled: false, displayAllButton: true };
        //TODO: add rest
        let advancedFilters: AdvancedFilterCategoryModel[] = [
            subjects, claims, interactions
        ];

        return advancedFilters;


    }
    //TODO:translate the itemsearchfiltermodel to a flat itemsearchmodel
    getItemSearchModel(itemSearchFilter: ItemsSearchFilterModel): ItemsSearchModel {
        const itemSearch: ItemsSearchModel = {
            claims: itemSearchFilter.claims.filterOptions,
            subjects: itemSearchFilter.subjects.filterOptions,
            interactionTypes: itemSearchFilter.interactionTypes.filterOptions
        };
        return itemSearch;

    }
    onFetchFilterModel(itemSearchFilter: ItemsSearchFilterModel) {
        //TODO: Filter grades
        const filters = this.getFilterCategories(itemSearchFilter);
        const searchModel = this.getItemSearchModel(itemSearchFilter);
        this.setState({
            itemSearch: { kind: "success", content: searchModel },
            currentFilter: filters
        }, );

    }

    selectSingleResult() {
        const searchResults = this.state.searchResults;
        if (searchResults.kind === "success" && searchResults.content && searchResults.content.length === 1) {
            const searchResult = searchResults.content[0];
            itemPageLink(searchResult.bankKey, searchResult.itemKey);
        }
    }

    isLoading() {
        return this.state.searchResults.kind === "loading" || this.state.searchResults.kind === "reloading";
    }

    getFilteredItemCards(cards: ItemCardModel[]): ItemCardModel[] {
        const filters = this.state.currentFilter;
        if (filters) {
            const searchAPI = ItemSearch.filterToSearchApiModel(filters);
            cards = ItemSearch.filterItemCards(cards, searchAPI);
        }

        return cards;
    }


    renderItemCards(): JSX.Element[] | JSX.Element | undefined {
        const cardState = this.state.searchResults;
        const cards = getResourceContent(cardState);
        let resultsElement: JSX.Element[] | JSX.Element | undefined;

        if (cards) {
            const filteredCards = this.getFilteredItemCards(cards);

            if (filteredCards.length === 0) {
                resultsElement = <span className="placeholder-text" role="alert">No results found for the given search terms.</span>
            }

            resultsElement = filteredCards.map(digest =>
                <ItemCard {...digest} key={digest.bankKey.toString() + "-" + digest.itemKey.toString()} />);

        } else if (cardState.kind === "failure") {
            resultsElement = <div className="placeholder-text" role="alert">An error occurred. Please try again later.</div>;
        }

        return resultsElement;

    }
    renderResultElement(): JSX.Element {
        return <div className="search-results" >
            {this.renderItemCards()}
        </div>


    }
    beginSearchFilter = (categories: AdvancedFilterCategoryModel[]) => {
        const searchModel = getResourceContent(this.state.itemSearch);

        if (searchModel) {
            const searchAPI = ItemSearch.filterToSearchApiModel(categories);
            categories = Filter.getUpdatedSearchFilters(searchModel, categories, searchAPI)
        }

        this.setState({
            currentFilter: categories
        });
    }


    renderfilters() {
        const isLoading = this.isLoading();


        if (this.state.currentFilter) {
            return (
                <FilterISPComponent defaultFilter={this.state.currentFilter} searchFilters={this.beginSearchFilter} />
            );
        }
        else {
            return <p><em>Loading...</em></p>
        }
    }

    render() {
        return (
            <div className="search-container" style={{ "marginTop": "50px" }}>
                {this.renderfilters()}
                {this.renderResultElement()}
            </div>
        );
    }
}
