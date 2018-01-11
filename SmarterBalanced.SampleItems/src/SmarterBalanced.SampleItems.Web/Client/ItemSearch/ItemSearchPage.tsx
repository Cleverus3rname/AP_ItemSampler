import * as React from "react";
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
import { getFilterCategories, getItemSearchModel } from "./ItemSearch";

export interface Props extends RouteComponentProps<{}> {
  itemsSearchClient: (params: SearchAPIParamsModel) => Promise<ItemCardModel[]>;
  itemsViewModelClient: () => Promise<ItemsSearchFilterModel>;
}

export interface State {
  searchResults: Resource<ItemCardModel[]>;
  itemSearch: Resource<ItemsSearchModel>;
  advFilters?: AdvancedFilterCategoryModel[];
  basicFilters?: BasicFilterCategoryModel[];
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
      redirect: false
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
    const { searchAPIParams } = this.state;

    let filters = getFilterCategories(itemSearchFilter, searchAPIParams);
    const searchModel = getItemSearchModel(itemSearchFilter);

    filters = Filter.getUpdatedSearchFilters(
      searchModel,
      filters,
      searchAPIParams
    );

    this.setState({
      itemSearch: { kind: "success", content: searchModel },
      advFilters: filters,
      basicFilters: []
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

  onFilterApplied(
    basicCategories: BasicFilterCategoryModel[],
    advCategories: AdvancedFilterCategoryModel[]
  ) {
    const { itemSearch } = this.state;
    let bothFilters: FilterCategoryModel[] = basicCategories;
    bothFilters = bothFilters.concat(advCategories);

    let newAdvFilters = advCategories;

    const searchModel = getResourceContent(itemSearch);

    // TODO: if we have duplicate categories maybe we will need to parse two lists in the url.location
    let searchAPI = ItemSearch.filterToSearchApiModel(advCategories);

    if (searchModel) {
      newAdvFilters = Filter.getUpdatedSearchFilters(
        searchModel,
        advCategories,
        searchAPI
      );

      searchAPI = ItemSearch.filterToSearchApiModel(newAdvFilters);
    }

    this.updateLocationSearch(searchAPI);

    this.setState({
      advFilters: advCategories,
      searchAPIParams: searchAPI,
      basicFilters: basicCategories
    });
  }

  handleBasicFilterApplied = (basicFilters: BasicFilterCategoryModel[]) => {
    this.onFilterApplied(basicFilters, this.state.advFilters || []);
  };

  handleAdvancedFilterApplied = (
    advCategories: AdvancedFilterCategoryModel[]
  ) => {
    this.onFilterApplied(this.state.basicFilters || [], advCategories);
  };

  renderFilters() {
    let content;
    const { advFilters, basicFilters } = this.state;

    if (advFilters && basicFilters) {
      content = (
        <div>
          <FilterContainer
            filterId="sb-filter-id"
            advancedFilterCategories={advFilters}
            onUpdateAdvancedFilter={this.handleAdvancedFilterApplied}
            basicFilterCategories={basicFilters}
            onUpdateBasicFilter={this.handleBasicFilterApplied}
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
