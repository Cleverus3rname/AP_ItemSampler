import '../Styles/search.less';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { RouteComponentProps } from 'react-router';
import { FilterISPComponent } from './filterISPComponent';
import { updateUrl, readUrl } from '../UrlHelper';
import { filterItems }  from './ItemSearchFilter'

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
    ItemSearchFilterModel
} from '@osu-cass/sb-components';



export const ItemsSearchClient = (params: SearchAPIParamsModel) =>
    get<ItemCardModel[]>("/BrowseItems/search", params);

export const ItemsViewModelClient = () => 
    get<ItemSearchFilterModel>("/BrowseItems/FilterSearchModel");



export interface Props extends RouteComponentProps<{}> {
    itemsSearchClient: (params: SearchAPIParamsModel) => Promise<ItemCardModel[]>;
    itemsViewModelClient: () => Promise<ItemSearchFilterModel>;
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

    getFilterCategories(itemSearchFilter: ItemSearchFilterModel) {

    }
    //TODO:translate the itemsearchfiltermodel to a flat itemsearchmodel
    getItemSearchModel(itemSearchFilter: ItemSearchFilterModel) {

    }
    onFetchFilterModel(itemSearchFilter: ItemSearchFilterModel) {
        //TODO: Filter grades
        const filters = this.getFilterCategories();
        const searchModel = this.getItemSearchModel();
        this.setState({
            itemSearch: { kind: "success", content: searchModel },
            currentFilter: filter
        }, );

    }

    updateCurrentFilterOnLoad(itemsSearch: ItemsSearchModel) {
        this.setState({})
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

    renderResultElement(): JSX.Element | JSX.Element[] | undefined {
        const searchResults = this.state.searchResults;

        let resultsElement: JSX.Element[] | JSX.Element | undefined;
        if ((searchResults.kind === "success" || searchResults.kind === "reloading") && searchResults.content) {

            resultsElement = filterItems(this.state.currentFilter, searchResults.content).map(digest =>
                <ItemCard {...digest} key={digest.bankKey.toString() + "-" + digest.itemKey.toString()} />);

            if (resultsElement.length === 0) {
                resultsElement = <span className="placeholder-text" role="alert">No results found for the given search terms.</span>
            }

        } else if (searchResults.kind === "failure") {
            resultsElement = <div className="placeholder-text" role="alert">An error occurred. Please try again later.</div>;
        } else {
            resultsElement = undefined;
        }
        return resultsElement;
    }
    beginSearchFilter = (categories: AdvancedFilterCategoryModel[]) => {
        this.setState({
            currentFilter: categories
        });
        const searchResults = this.state.itemSearch;
        if (searchResults.kind !== "success") {

            const params = this.translateAdvancedFilterCate(categories);
            this.props.itemsSearchClient(params)
                .then((data) => this.onSearch(data))
                .catch((err) => this.onError(err));
        }
    }


    renderfilters() {
        const isLoading = this.isLoading();
        const searchVm = this.state.itemSearch;

        const param = this.translateAdvancedFilterCate(this.state.currentFilter);

        if (searchVm.kind == "success" || searchVm.kind == "reloading") {
            if (searchVm.content) {
                return (
                    <FilterISPComponent defaultFilter={this.state.currentFilter} searchFilters={this.beginSearchFilter} />
                );
            }
            else {
                return <p><em>Loading...</em></p>
            }
        }
        else {
            return <p><em>Loading...</em></p>
        }
    }

    render() {
        return (
            <div className="search-container" style={{ "marginTop":"50px"}}>
                {this.renderfilters()}
                <div className="search-results" >
                    {this.renderResultElement()}
                </div>
            </div>
        );
    }
}
