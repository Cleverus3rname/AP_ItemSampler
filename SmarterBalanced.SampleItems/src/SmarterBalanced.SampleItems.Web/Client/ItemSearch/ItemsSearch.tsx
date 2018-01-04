import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { RouteComponentProps } from 'react-router';

import {
    Resource,
    ItemCard,
    ItemCardModel,
    SearchAPIParamsModel,
    ItemsSearchModel,
    AdvancedFilterContainer,
    AdvancedFilterCategoryModel,
    Filter,
    ItemSearch,
    ItemsSearchFilterModel,
    getResourceContent,
    SearchUrl,
    FilterLink,
    SearchResultContainer,
    SearchResultType,
    ItemModel
} from '@osu-cass/sb-components';
import { getFilterCategories, getItemSearchModel } from './SiwSearch';


export interface Props extends RouteComponentProps<{}> {
    itemsSearchClient: ( params: SearchAPIParamsModel ) => Promise<ItemCardModel[]>;
    itemsViewModelClient: () => Promise<ItemsSearchFilterModel>;
}

export interface State {
    searchResults: Resource<ItemCardModel[]>;
    itemSearch: Resource<ItemsSearchModel>;
    currentFilter?: AdvancedFilterCategoryModel[];
    searchAPIParams: SearchAPIParamsModel;

}

export class ItemsSearchComponent extends React.Component<Props, State> {

    constructor ( props: Props ) {
        super( props );

        const searchAPIParams = this.getLocationSearch( this.props.location.search );
        this.state = {
            searchAPIParams,
            searchResults: { kind: "loading" },
            itemSearch: { kind: "loading" }
        };
    }

    componentDidMount () {
        this.props.itemsViewModelClient()
            .then( data => this.onFetchFilterModel( data ) )
            .catch( err => this.onError( err ) );

        this.props.itemsSearchClient( {} )
            .then( ( data ) => this.onSearch( data ) )
            .catch( ( err ) => this.onError( err ) );
    }

    componentWillReceiveProps ( nextProps: Props ) {
        if ( nextProps.history.action === "PUSH" ) {
            const searchAPIParams = this.getLocationSearch( nextProps.location.search );
            const itemSearch = getResourceContent( this.state.itemSearch );
            this.setState( {
                searchAPIParams,
                itemSearch: { kind: "reloading", content: itemSearch }
            } );
            this.props.itemsViewModelClient()
                .then( data => this.onFetchFilterModel( data ) )
                .catch( err => this.onError( err ) );
        }
    }


    onSearch ( results: ItemCardModel[] ) {
        this.setState( { searchResults: { kind: "success", content: results } } );
    }

    onError ( err: any ) {
        this.setState( { searchResults: { kind: "failure" } } );
    }


    onFetchFilterModel ( itemSearchFilter: ItemsSearchFilterModel ) {
        //TODO: Filter grades
        let filters = getFilterCategories( itemSearchFilter, this.state.searchAPIParams );
        const searchModel = getItemSearchModel( itemSearchFilter );
        filters = Filter.getUpdatedSearchFilters( searchModel, filters, this.state.searchAPIParams );
        this.setState( {
            itemSearch: { kind: "success", content: searchModel },
            currentFilter: filters
        }, );

    }

    getFilteredItemCards ( cards: ItemCardModel[] ): ItemCardModel[] {
        let newCards;
        try {
            newCards = ItemSearch.filterItemCards( cards, this.state.searchAPIParams );
        } catch ( exception ) {
            console.error( "unable to filter item cards", exception );
        }

        return newCards ? newCards : cards;
    }


    renderItemCards (): JSX.Element[] | JSX.Element | undefined {
        const cardState = this.state.searchResults;
        const cards = getResourceContent( cardState );
        let resultsElement: JSX.Element[] | JSX.Element | undefined;

        if ( cards ) {
            const filteredCards = this.getFilteredItemCards( cards );

            if ( filteredCards.length === 0 ) {
                resultsElement = <span className="placeholder-text" role="alert">No results found for the given search terms.</span>;
            }

            resultsElement = filteredCards.map( digest =>
                <ItemCard
                    {...digest}
                    key={`${ digest.bankKey }-${ digest.itemKey }`} /> );

        } else if ( cardState.kind === "failure" ) {
            resultsElement = <div className="placeholder-text" role="alert">An error occurred. Please try again later.</div>;
        }

        return resultsElement;

    }

    renderResultElement (): JSX.Element {
        const cardState = this.state.searchResults;
        const cards = getResourceContent( cardState );
        let filteredItemCards;
        if ( cards ) {
            filteredItemCards = this.getFilteredItemCards( cards );
        }

        return <div className="search-results" >
            <SearchResultContainer
                onRowSelection={(item: ItemModel, reset: boolean) => {}}
                onItemSelection={(item: ItemCardModel) => {}}
                itemCards={filteredItemCards}
                defaultRenderType={SearchResultType.ItemCard}
            />
        </div>;

    }

    updateLocationSearch ( searchAPI: SearchAPIParamsModel ) {
        const search = SearchUrl.encodeQuery( searchAPI );
        const location = {
            search, ...this.props.history.location
        };
        this.props.history.replace( location );
    }

    getLocationSearch ( search: string ): SearchAPIParamsModel {
        let searchAPI: SearchAPIParamsModel = {};
        try {
            searchAPI = SearchUrl.decodeSearch( search );
        } catch { }

        return searchAPI;
    }

    onFilterUpdate = ( categories: AdvancedFilterCategoryModel[] | undefined ) => {
        if ( !categories ) {
            return;
        }
        const searchModel = getResourceContent( this.state.itemSearch );
        let searchAPI = { ...this.state.searchAPIParams };
        const grad = Filter.getSelectedGrade( categories );

        searchAPI = ItemSearch.filterToSearchApiModel( categories );
        this.updateLocationSearch( searchAPI );

        if ( searchModel ) {
            categories = Filter.getUpdatedSearchFilters( searchModel, categories, searchAPI );
        }

        this.setState( {
            currentFilter: categories,
            searchAPIParams: searchAPI
        } );
    }

    renderFilters () {
        let content = <p><em>Loading...</em></p>;
        if ( this.state.currentFilter ) {
            content = (
                <div>
                    <AdvancedFilterContainer
                        filterId="siw-advanced-filter"
                        filterCategories={...this.state.currentFilter}
                        onUpdateFilter={this.onFilterUpdate}
                        pageTitle="Browse Items"
                    />
                </div>
            );
        }

        return content;
    }

    render () {
        return (
            <div className="container search-container">
                {this.renderFilters()}
                {this.renderResultElement()}
                <FilterLink filterId="siw-advanced-filter" />
            </div>
        );
    }
}
