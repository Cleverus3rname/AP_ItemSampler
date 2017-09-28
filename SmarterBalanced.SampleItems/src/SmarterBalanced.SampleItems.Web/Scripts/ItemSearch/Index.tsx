import * as ReactDOM from 'react-dom';
import * as React from 'react';
import * as ItemsSearch from './ItemsSearch';
import * as ItemSearchModels from './ItemsSearchModels';
import { get } from "../ApiModel";
import * as ItemCardModels from "../ItemCard/ItemCardModels"


export function initializeItemsSearch(viewModel: ItemSearchModels.ItemsSearchViewModel) {
    const searchClient = () => get<ItemCardModels.ItemCardViewModel[]>("/BrowseItems/search");
    const props = { ...viewModel, itemsSearchClient: searchClient };

    ReactDOM.render(
        <ItemsSearch.ItemsSearch {...props} />,
        document.getElementById("search-container") as HTMLElement);
}
