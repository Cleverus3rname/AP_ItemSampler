import * as React from 'react';
import { Route, Redirect } from 'react-router-dom';
import {
    Layout, SbNavlinkProps, AboutTestItemsContainer, aboutTestItemsClient, aboutThisItemViewModelClient, 
    ItemPageContainer, itemPageClient, itemAccessibilityClient} from '@osu-cass/sb-components';

import { Home } from './Home/Home';
import { ItemsSearchComponent, ItemsSearchClient, ItemsViewModelClient } from './ItemSearch/ItemsSearch'


export const siteLinks: SbNavlinkProps[] = [
    { name: "Home", url: "/" },
    { name: "About Test Items", url: "/AboutItems" },
    { name: "Browse Test Items", url: "/BrowseItems" }


];

export const routes = <Layout siteName="Sample Items" links={siteLinks}>
    <Route exact path='/' component={Home} />

    <Route exact path='/AboutItems' render={(props) => (
        <AboutTestItemsContainer {...props} aboutClient={aboutTestItemsClient} />
    )} />

    <Route exact path='/BrowseItems' render={(props) => (
        <ItemsSearchComponent {...props}
            itemsSearchClient={ItemsSearchClient}
            itemsViewModelClient={ItemsViewModelClient} />
    )} />

    <Route exact path='/Item' render={(props) => (
        <ItemPageContainer {...props}
            aboutThisClient={aboutThisItemViewModelClient}
            itemPageClient={itemPageClient}
            itemAccessibilityClient={itemAccessibilityClient} />
    )} />

</Layout>;