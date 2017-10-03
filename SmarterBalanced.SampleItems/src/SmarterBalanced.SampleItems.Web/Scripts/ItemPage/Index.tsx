import * as ReactDOM from 'react-dom';
import * as React from 'react';
import * as ItemPageContainer from './ItemPageContainer';
import * as ItemPageModels from './ItemPageModels';

export function initializeItemPage(itemProps: ItemPageModels.ItemPageViewModel) {
    ReactDOM.render(<ItemPageContainer.ItemPageContainer itemPage={itemProps} />, document.getElementById("item-container"));
}