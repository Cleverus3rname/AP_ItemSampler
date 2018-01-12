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
  FilterContainer
} from "@osu-cass/sb-components";
import { getAdvancedFilterCategories, getBasicFilterCategories, getItemSearchModel } from "./ItemSearch";

export interface Props extends RouteComponentProps<{}> {
  itemsSearchClient: (params: SearchAPIParamsModel) => Promise<ItemCardModel[]>;
  itemsViewModelClient: () => Promise<ItemsSearchFilterModel>;
}

export interface State {
  searchResults: Resource<ItemCardModel[]>;
  itemSearch: Resource<ItemsSearchModel>;
  advancedFilter: AdvancedFilterCategoryModel[];
  basicFilter: BasicFilterCategoryModel[];
  searchAPIParams: SearchAPIParamsModel;
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
      basicFilter: []
    };
  }

  componentDidMount() {
    this.props
      .itemsViewModelClient()
      .then(data => this.onFetchFilterModel(data))
      .catch(err => this.onError(err));

    this.props
      .itemsSearchClient({})
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
    this.setState({ searchResults: { kind: "success", content: results } });
  }

  onError(err: {}) {
    this.setState({ searchResults: { kind: "failure" } });
  }

  onFetchFilterModel(itemSearchFilter: ItemsSearchFilterModel) {
      let advancedFilters = getAdvancedFilterCategories(itemSearchFilter, this.state.searchAPIParams);
      let basicFilters = getBasicFilterCategories(itemSearchFilter, this.state.searchAPIParams);
      const searchModel = getItemSearchModel(itemSearchFilter);
      advancedFilters = Filter.getUpdatedSearchFilters(searchModel, advancedFilters, this.state.searchAPIParams);
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

  renderItemCards(): JSX.Element[] | JSX.Element | undefined {
    const cardState = this.state.searchResults;
    const cards = getResourceContent(cardState);
    let resultsElement: JSX.Element[] | JSX.Element | undefined;

    if (cards) {
      const filteredCards = this.getFilteredItemCards(cards);

      if (filteredCards.length === 0) {
        resultsElement = (
          <span className="placeholder-text" role="alert">
            No results found for the given search terms.
          </span>
        );
      }

      resultsElement = filteredCards.map(digest => (
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
  itemSelect = (item: ItemCardModel) => {};

  renderResultElement(): JSX.Element {
    const cardState = this.state.searchResults;
    const cards = getResourceContent(cardState);
    let filteredItemCards;
    if (cards) {
      filteredItemCards = this.getFilteredItemCards(cards);
    }

    return (
      <div className="search-results">
        <SearchResultContainer
          onRowSelection={this.rowSelect}
          onItemSelection={this.itemSelect}
          itemCards={filteredItemCards}
          defaultRenderType={SearchResultType.ItemCard}
          isLinkTable={true}
        />
      </div>
    );
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

  onAdvancedFilterUpdate = (categories: AdvancedFilterCategoryModel[] | undefined) => {
      if (!categories) {
          return;
      }

      const advancedSearchAPI = ItemSearch.filterToSearchApiModel(categories);
      const basicSearchAPI = ItemSearch.filterToSearchApiModel(this.state.basicFilter);
      const searchAPI = Object.assign({}, basicSearchAPI, advancedSearchAPI);
      this.updateLocationSearch(searchAPI);

      const searchModel = getResourceContent(this.state.itemSearch);
      if (searchModel) {
          categories = Filter.getUpdatedSearchFilters(searchModel, categories, searchAPI);
      }

      this.setState({
          advancedFilter: categories,
          searchAPIParams: searchAPI
      });
  }

  onBasicFilterUpdate = (categories: BasicFilterCategoryModel[] | undefined) => {
      if (!categories) {
          return;
      }

      const advancedSearchAPI = ItemSearch.filterToSearchApiModel(this.state.advancedFilter);
      const basicSearchAPI = ItemSearch.filterToSearchApiModel(categories);
      //later source objects overwrite properties of earlier ones
      const searchAPI = Object.assign({}, advancedSearchAPI, basicSearchAPI);
      this.updateLocationSearch(searchAPI);

      this.setState({
          basicFilter: categories,
          searchAPIParams: searchAPI
      });
  }

  renderFilters() {
    let content;
    const { basicFilter, advancedFilter } = this.state;

    if (advancedFilter && basicFilter) {
      content = (
        <div>
          <FilterContainer
            filterId="sb-filter-id"
            advancedFilterCategories={advancedFilter}
            onUpdateAdvancedFilter={this.onAdvancedFilterUpdate}
            basicFilterCategories={basicFilter}
            onUpdateBasicFilter={this.onBasicFilterUpdate}
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
