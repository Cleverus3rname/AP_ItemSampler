import * as ReactDOM from 'react-dom';
import * as React from 'react';
import * as AboutItemModels from './AboutItemModels';
import * as AboutItems from './AboutItems';

export function initializeAboutItems(viewModel: AboutItemModels.AboutItemsViewModel) {
    ReactDOM.render(
        <AboutItems.AboutItemComponent {...viewModel} />,
        document.getElementById("about-items") as HTMLElement
    );
}