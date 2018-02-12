import * as React from "react";
import { Route, Redirect, Switch } from "react-router-dom";
import {
    Layout,
    SbNavlinkProps,
    aboutTestItemsClient,
    aboutThisItemViewModelClient,
    itemPageClient,
    itemAccessibilityClient,
    ItemViewPage,
    AboutTestItemsPage,
    ErrorPageContainer
} from "@osu-cass/sb-components";

import { Home } from "./Home/Home";
import { ItemsSearchComponent } from "./ItemSearch/ItemSearchPage";
import { RouteComponentProps } from "react-router";
import {
    itemSearchClient,
    itemsSearchFilterClient
} from "./ItemSearch/ItemSearch";

export const siteLinks: SbNavlinkProps[] = [
    { name: "Home", url: "/" },
    { name: "About Test Items", url: "/AboutItems" },
    { name: "Browse Test Items", url: "/BrowseItems" }
];

const appName = "Sample Items";
const fetchItemCards = () => itemSearchClient;
const fetchItemViewModel = () => itemsSearchFilterClient;

export const routes = (
    <Layout siteName="Sample Items" links={siteLinks}>
        <Switch>
            <Route exact path="/" render={props => (
                <Home {...props} appName={appName} />
            )} />

            <Route
                path="/AboutItems/:itemType?"
                render={props => (
                    <AboutTestItemsPage
                        {...props}
                        aboutClient={aboutTestItemsClient}
                        showRubrics={false}
                        appName={appName}
                    />
                )}
            />

            <Route
                path="/BrowseItems"
                render={props => (
                    <ItemsSearchComponent
                        {...props}
                        itemsSearchClient={fetchItemCards}
                        itemsViewModelClient={fetchItemViewModel}
                        appName={appName}
                    />
                )}
            />

            <Route
                path="/Item/:bankKey-:itemKey"
                render={props => (
                    <ItemViewPage
                        {...props}
                        aboutThisClient={aboutThisItemViewModelClient}
                        itemPageClient={itemPageClient}
                        itemAccessibilityClient={itemAccessibilityClient}
                        showRubrics={false}
                        appName={appName}
                    />
                )}
            />

            <Route
                render={props => (
                    <ErrorPageContainer errorCode={404} />
                )}
            />
        </Switch>
    </Layout>
);
