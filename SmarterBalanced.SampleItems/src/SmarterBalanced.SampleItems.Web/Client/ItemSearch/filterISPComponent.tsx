import * as Models from './ItemsSearchModels';
import * as React from 'react';
import * as GradeLevels from '../GradeLevels/GradeLevels';
import { parseQueryString } from '../ApiModel';
import { mockAdvancedFilterCategories } from './filterModels';
import { AdvancedFilterCategory, AdvancedFilterContainer, AdvancedFilterOption } from '@osu-cass/react-advanced-filter';


export interface Props {
    defaultFilter: AdvancedFilterCategory[];
    searchFilters: (categories: AdvancedFilterCategory[]) => void;
}

export interface State {
    currentFilter: AdvancedFilterCategory[];

    itemId: string;
    gradeLevels: GradeLevels.GradeLevels;
    subjects: string[];
    claims: string[];
    interactionTypes: string[];
    targets: number[];
    performanceOnly: boolean;
}

export class FilterISPComponent extends React.Component<Props, State> {
    constructor(props: Props) {
        super(props);

        //grab current url
        const queryObject = parseQueryString(location.search);
        const itemId = (queryObject["itemID"] || [])[0] || "";

        const gradeString = (queryObject["gradeLevels"] || [])[0];
        const gradeLevels: GradeLevels.GradeLevels = parseInt(gradeString, 10) || GradeLevels.GradeLevels.NA;

        const subjects = queryObject["subjects"] || [];
        const claims = queryObject["claims"] || [];
        const interactionTypes = queryObject["interactionTypes"] || [];
        const performanceOnly = (queryObject["performanceOnly"] || [])[0] === "true";
        const targets = queryObject["targets"] || [];

        this.state = {
            currentFilter:this.props.defaultFilter,

            itemId: itemId,
            gradeLevels: gradeLevels,
            subjects: subjects,
            claims: claims,
            interactionTypes: interactionTypes,
            targets: targets.map(t => Number(t)),
            performanceOnly: performanceOnly
        }
    }

    encodeQuery(): string {
        let pairs: string[] = [];
        if (this.state.claims && this.state.claims.length !== 0) {
            pairs.push("claims=" + this.state.claims.join(","));
        }
        if (this.state.gradeLevels !== GradeLevels.GradeLevels.NA) {
            pairs.push("gradeLevels=" + this.state.gradeLevels);
        }
        if (this.state.interactionTypes && this.state.interactionTypes.length !== 0) {
            pairs.push("interactionTypes=" + this.state.interactionTypes.join(","));
        }
        if (this.state.itemId) {
            pairs.push("itemID=" + this.state.itemId);
        }
        if (this.state.subjects && this.state.subjects.length !== 0) {
            pairs.push("subjects=" + this.state.subjects.join(","));
        }
        if (this.state.performanceOnly) {
            pairs.push("performanceOnly=true");
        }
        if (this.state.targets && this.state.targets.length !== 0) {
            pairs.push("targets=" + this.state.targets.join(","));
        }

        if (pairs.length === 0) {
            return "/BrowseItems";
        }

        const query = "?" + pairs.join("&");
        return query;
    }

    render() {
        return (
            <AdvancedFilterContainer filterOptions={...this.state.currentFilter} onClick={this.props.searchFilters} />
        );
    }
}