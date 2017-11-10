import '../Styles/search.less';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { RouteComponentProps } from 'react-router';
import { AdvancedFilterCategory, AdvancedFilterContainer } from '@osu-cass/react-advanced-filter';
import { mockAdvancedFilterCategories } from './filterModels';
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
    AdvancedFilterCategoryModel
} from '@osu-cass/sb-components';



export const ItemsSearchClient = (params: SearchAPIParamsModel) =>
    get<ItemCardModel[]>("/BrowseItems/search", params);

export const ItemsViewModelClient = () =>
    get<ItemsSearchModel>("/BrowseItems/ItemsSearchViewModel");


export interface Props extends RouteComponentProps<{}> {
    itemsSearchClient: (params: SearchAPIParamsModel) => Promise<ItemCardModel[]>;
    itemsViewModelClient: () => Promise<ItemsSearchModel>;
}

export interface State {
    searchResults: Resource<ItemCardModels.ItemCardViewModel[]>;
    itemSearch: Resource<ItemSearchModels.ItemsSearchViewModel>;
    currentFilter: AdvancedFilterCategory[];
}

export class ItemsSearchComponent extends React.Component<Props, State> {
    constructor(props: Props) {
        super(props);
        this.state = {
            searchResults: { kind: "loading" },
            itemSearch: { kind: "loading" },
            currentFilter: mockAdvancedFilterCategories
        };

        this.props.itemsViewModelClient()
            .then(data => this.onFetchedItemSearch(data))
            .catch(err => this.onError(err));
        
    }

    beginSearch(params: SearchAPIParamsModel) {
        const searchResults = this.state.searchResults;
        if (searchResults.kind === "success") {
            this.setState({
                searchResults: {
                    kind: "reloading",
                    content: searchResults.content
                }
            });
        } else if (searchResults.kind === "failure") {
            this.setState({
                searchResults: { kind: "loading" }
            });
        }

        this.props.itemsSearchClient(params)
            .then((data) => this.onSearch(data))
            .catch((err) => this.onError(err));
    }

    onSearch(results: ItemCardModel[]) {
        this.setState({ searchResults: { kind: "success", content: results } });
    }

    onError(err: any) {
        this.setState({ searchResults: { kind: "failure" } });
    }

    onFetchedItemSearch(itemsSearch: ItemSearchModels.ItemsSearchViewModel) {
        const newFilters = [...this.state.currentFilter];

        const newGrade = this.state.currentFilter.find(f => f.label.toLocaleLowerCase() === "grades")
        const newSubject = this.state.currentFilter.find(f => f.label.toLocaleLowerCase() === "subjects");
        if (newGrade && newSubject) {
            // TODO: get buisness logic for displable filter options. 
            //if math is selected change displayed claims/targets/etc...

            //else if english is selected change displayed  claims/targets/etc...
        }

        this.setState({
            itemSearch: { kind: "success", content: itemsSearch },
            currentFilter: newFilters
        }, );

    }

    updateCurrentFilterOnLoad(itemsSearch: ItemSearchModels.ItemsSearchViewModel) {
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
            resultsElement = searchResults.content && searchResults.content.length === 0
                ? <span className="placeholder-text" role="alert">No results found for the given search terms.</span>
                : searchResults.content.map(digest =>
                    <ItemCard {...digest} key={digest.bankKey.toString() + "-" + digest.itemKey.toString()} />);
        } else if (searchResults.kind === "failure") {
            resultsElement = <div className="placeholder-text" role="alert">An error occurred. Please try again later.</div>;
        } else {
            resultsElement = undefined;
        }
        return resultsElement;
    }

    // TODO: Optimize this 
    translateAdvancedFilterCate(categorys: AdvancedFilterCategoryModel[]): SearchAPIParamsModel {
        let model: SearchAPIParamsModel = {
            itemId: "",
            gradeLevels: GradeLevels.All,
            subjects: [],
            claims: [],
            interactionTypes: [],
            performanceOnly: false,
            targets:[]
        };

        const gradeCategory = categorys.find(c => c.label.toLowerCase() === 'grades');
        if (gradeCategory && !gradeCategory.disabled) {
            model.gradeLevels = GradeLevels.All;
            const selectedGrade = gradeCategory.filterOptions.find(fo => fo.isSelected);
            if (selectedGrade) {
                model.gradeLevels = Number(selectedGrade.key);
            }
        }

        const subjectsCategory = categorys.find(c => c.label.toLowerCase() === 'subjects');
        if (subjectsCategory && !subjectsCategory.disabled) {
            subjectsCategory.filterOptions.forEach(fo => {
                if (fo.isSelected) {
                    model.subjects.push(fo.key);
                }
            });
        }

        return model;
    }

    beginSearchFilter = (categories: AdvancedFilterCategory[]) => {
        this.setState({
            currentFilter: categories
        });
        const searchResults = this.state.searchResults;
        if (searchResults.kind === "success") {
            this.setState({
                searchResults: {
                    kind: "reloading",
                    content: searchResults.content
                }
            });
        } else if (searchResults.kind === "failure") {
            this.setState({
                searchResults: { kind: "loading" }
            });
        }

        //filterItems(categories, this.state.searchResults.content);

        const params = this.translateAdvancedFilterCate(categories);

        this.props.itemsSearchClient(params)
            .then((data) => this.onSearch(data))
            .catch((err) => this.onError(err));


    }


    renderfilters() {
        const isLoading = this.isLoading();
        const searchVm = this.state.itemSearch;

        const param = this.translateAdvancedFilterCate(this.state.currentFilter);

        //pull item cards
        this.props.itemsSearchClient(param)
            .then((data) => this.onSearch(data))
            .catch((err) => this.onError(err));

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
