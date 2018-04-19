﻿import * as React from "react";
import * as ReactDOM from "react-dom";
import { RouteComponentProps, Redirect } from "react-router";

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
  ItemModel,
  BasicFilterCategoryModel,
  FilterCategoryModel,
  FilterContainer,
  CombinedFilter,
  SIWFilter,
  FilterType,
  OptionTypeModel
} from "@osu-cass/sb-components";
import { getAdvancedFilterCategories, getBasicFilterCategories, getItemSearchModel } from "./ItemSearch";

export interface Props extends RouteComponentProps<{}> {
  itemsSearchClient: () => Promise<ItemCardModel[]>;
  itemsViewModelClient: () => Promise<ItemsSearchFilterModel>;
  appName: string;
}

export interface State {
  searchResults: Resource<ItemCardModel[]>;
  itemSearch: Resource<ItemsSearchModel>;
  advancedFilter: AdvancedFilterCategoryModel[];
  basicFilter: BasicFilterCategoryModel[];
  searchAPIParams: SearchAPIParamsModel;
  visibleItemCards: ItemCardModel[];
  item: ItemModel | undefined;
  redirect: boolean;
}

export class ItemsSearchComponent extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);

    const searchAPIParams = this.getLocationSearch(this.props.location.search);
    this.state = {
      searchAPIParams,
      searchResults: { kind: "loading" },
      itemSearch: { kind: "loading" },
      item: undefined,
      redirect: false,
      advancedFilter: [],
      basicFilter: [],
      visibleItemCards: []
    };
  }

  componentDidMount() {
      document.title = `Browse Items - Smarter Balanced ${this.props.appName}`;
    this.props
      .itemsViewModelClient()
      .then(data => this.onFetchFilterModel(data))
      .catch(err => this.onError(err));

    this.props
      .itemsSearchClient()
      .then(data => this.onSearch(data))
      .catch(err => this.onError(err));
  }

  componentWillReceiveProps(nextProps: Props) {
    if (nextProps.history.action === "PUSH") {
      const searchAPIParams = this.getLocationSearch(nextProps.location.search);
      const itemSearch = getResourceContent(this.state.itemSearch);
      this.setState({
        searchAPIParams,
        itemSearch: { kind: "reloading", content: itemSearch }
      });
      this.props
        .itemsViewModelClient()
        .then(data => this.onFetchFilterModel(data))
        .catch(err => this.onError(err));
    }
  }

  onSearch(results: ItemCardModel[]) {
      this.setState({ searchResults: { kind: "success", content: results } }, () =>
          this.onFilterUpdated(this.state.searchAPIParams, this.state.basicFilter, this.state.advancedFilter)
      );
  }

    onError(err: {}) {
        console.error(err);
    this.setState({ searchResults: { kind: "failure" } });
  }

  onFetchFilterModel(itemSearchFilter: ItemsSearchFilterModel) {
      let advancedFilters = getAdvancedFilterCategories(itemSearchFilter, this.state.searchAPIParams);
      let basicFilters = getBasicFilterCategories(itemSearchFilter, this.state.searchAPIParams);
      const searchModel = getItemSearchModel(itemSearchFilter);
      advancedFilters = Filter.getUpdatedSearchFilters(searchModel, advancedFilters, this.state.searchAPIParams);
      advancedFilters = Filter.hideFiltersBasedOnSearchParams(advancedFilters, this.state.searchAPIParams);
      basicFilters = Filter.getUpdatedSearchFilters(searchModel, basicFilters, this.state.searchAPIParams);
      basicFilters = Filter.hideFiltersBasedOnSearchParams(basicFilters, this.state.searchAPIParams);
      this.setState({
          itemSearch: { kind: "success", content: searchModel },
          advancedFilter: advancedFilters,
          basicFilter: basicFilters
      });
  }

  getFilteredItemCards(cards: ItemCardModel[]): ItemCardModel[] {
    let newCards;
    try {
      newCards = ItemSearch.filterItemCards(cards, this.state.searchAPIParams);
    } catch (exception) {
      console.error("unable to filter item cards", exception);
    }

    return newCards ? newCards : cards;
  }

  updateLocationSearch(searchAPI: SearchAPIParamsModel) {
    const search = SearchUrl.encodeQuery(searchAPI);
    const location = { ...this.props.history.location, search };
    this.props.history.replace(location);
  }

  getLocationSearch(search: string): SearchAPIParamsModel {
    let searchAPI: SearchAPIParamsModel = {};
    try {
        searchAPI = SearchUrl.decodeSearch(search);
    } catch {}

    return searchAPI;
  }

  onFilterUpdated = (
      searchParams: SearchAPIParamsModel,
      basic: BasicFilterCategoryModel[],
      advanced: AdvancedFilterCategoryModel[]
    ) => {
      this.updateLocationSearch(searchParams);
      // filter cards by everything but target
      const cards = getResourceContent(this.state.searchResults);
      const searchParamsNoTarget = { ...searchParams, targets: undefined }
      let newCards = ItemSearch.filterItemCards(cards || [], searchParamsNoTarget);
      // hide irrelevant target options
      const newBasicFilter = Filter.hideTargetOptions(basic, newCards);
      // filter by target also
      if (searchParams.targets && searchParams.targets.length > 0) {
          newCards = newCards.filter(c => searchParams.targets!.indexOf(c.targetId) !== -1);
      }
      
      this.setState({
          basicFilter: basic,
          advancedFilter: advanced,
          searchAPIParams: searchParams,
          visibleItemCards: newCards
      });
  }

  renderItemCards(): JSX.Element[] | JSX.Element | undefined {
    const cardState = this.state.searchResults;
    const cards = getResourceContent(cardState);
    let resultsElement: JSX.Element[] | JSX.Element | undefined;

    if (cards) {
      const filteredCards = this.getFilteredItemCards(cards);

      if (this.state.visibleItemCards.length === 0) {
        resultsElement = (
          <span className="placeholder-text" role="alert">
            No results found for the given search terms.
          </span>
        );
      }

      resultsElement = this.state.visibleItemCards.map(digest => (
        <ItemCard {...digest} key={`${digest.bankKey}-${digest.itemKey}`} />
      ));
    } else if (cardState.kind === "failure") {
      resultsElement = (
        <div className="placeholder-text" role="alert">
          An error occurred. Please try again later.
        </div>
      );
    }

    return resultsElement;
  }

  rowSelect = (item: ItemModel, reset: boolean) => {
    this.setState({ item, redirect: true });
  };
  itemSelect = (item: ItemCardModel) => { return; };

  renderResultElement(): JSX.Element {
    return (
      <div className="search-results">
        <SearchResultContainer
          onRowSelection={this.rowSelect}
          onItemSelection={this.itemSelect}
          itemCards={this.state.visibleItemCards}
          defaultRenderType={SearchResultType.ItemCard}
          isLinkTable={true}
        />
      </div>
    );
  }

  renderFilters() {
      let content;
      const { basicFilter, advancedFilter, searchAPIParams, itemSearch } = this.state;
      const searchModel = getResourceContent(itemSearch);
    if (advancedFilter && basicFilter) {
      content = (
          <div>
              <SIWFilter
                  basicFilter={basicFilter}
                  advancedFilter={advancedFilter}
                  searchAPI={searchAPIParams}
                  filterId="sb-filter-id"
                  onFilterUpdated={this.onFilterUpdated}
                  searchModel={searchModel}
              />
        </div>
      );
    }

    return content;
  }

  renderMain(): JSX.Element {
    return (
      <div className="container search-container">
        <h2 className="page-title">Browse Items</h2>
        {this.renderFilters()}
        {this.renderResultElement()}
        <FilterLink filterId="sb-filter-id" />
      </div>
    );
  }

  render() {
    const { redirect, item } = this.state;
    let content;

    if (item) {
      content = <Redirect push to={`/item/${item.bankKey}-${item.itemKey}`} />;
    } else {
      content = this.renderMain();
    }

    return content;
  }
}
