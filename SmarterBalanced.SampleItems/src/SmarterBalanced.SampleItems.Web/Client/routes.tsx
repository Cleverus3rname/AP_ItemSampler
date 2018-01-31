import * as React from "react";
import { Route, Redirect } from "react-router-dom";
import {
  Layout,
  SbNavlinkProps,
  aboutTestItemsClient,
  aboutThisItemViewModelClient,
  itemPageClient,
  itemAccessibilityClient,
  ItemViewPage,
  AboutTestItemsPage
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

const fetchItemCards = () => itemSearchClient;
const fetchItemViewModel = () => itemsSearchFilterClient;

export const routes = (
  <Layout siteName="Sample Items" links={siteLinks}>
    <Route exact path="/" component={Home} />

    <Route
      path="/AboutItems/:itemType?"
      render={props => (
        <AboutTestItemsPage
          {...props}
          aboutClient={aboutTestItemsClient}
          showRubrics={false}
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
        />
      )}
    />
  </Layout>
);
