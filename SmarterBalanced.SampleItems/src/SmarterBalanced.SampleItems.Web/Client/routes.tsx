import * as React from 'react';
import { Route, Redirect } from 'react-router-dom';
import {
    Layout, SbNavlinkProps, AboutTestItemsContainer, aboutTestItemsClient, aboutThisItemViewModelClient,
    ItemPageContainer, itemPageClient, itemAccessibilityClient
} from '@osu-cass/sb-components';

import { Home } from './Home/Home';
import { ItemsSearchComponent } from './ItemSearch/ItemsSearch';
import { RouteComponentProps } from 'react-router';
import { ItemsSearchClient, ItemsViewModelClient } from './ItemSearch/SiwSearch';

export const siteLinks: SbNavlinkProps[] = [
    { name: "Home", url: "/" },
    { name: "About Test Items", url: "/AboutItems" },
    { name: "Browse Test Items", url: "/BrowseItems" }

];

export const routes = <Layout siteName="Sample Items" links={siteLinks}>
    <Route exact path='/' component={Home} />

    <Route path='/AboutItems/:itemType?' render={(props) => (
        <AboutTestItemsContainer {...props} aboutClient={aboutTestItemsClient} />
    )} />

    <Route path='/BrowseItems' render={(props) => (
        <ItemsSearchComponent {...props}
            itemsSearchClient={ItemsSearchClient}
            itemsViewModelClient={ItemsViewModelClient} />
    )} />

    <Route path='/Item/:bankKey-:itemKey' render={(props) => (
        <ItemPageContainer {...props}
            aboutThisClient={aboutThisItemViewModelClient}
            itemPageClient={itemPageClient}
            itemAccessibilityClient={itemAccessibilityClient} />
    )} />

</Layout>;