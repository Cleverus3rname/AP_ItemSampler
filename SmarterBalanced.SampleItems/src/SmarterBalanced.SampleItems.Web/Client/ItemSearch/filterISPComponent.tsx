import * as Models from './ItemsSearchModels';
import * as React from 'react';
import * as GradeLevels from '../GradeLevels/GradeLevels';
import { parseQueryString } from '../ApiModel';
import { mockAdvancedFilterCategories } from './filterModels';
import { AdvancedFilterCategory, AdvancedFilterContainer, AdvancedFilterOption } from '@osu-cass/react-advanced-filter';
import { updateUrl, readUrl } from '../UrlHelper';


export interface Props {
    defaultFilter: AdvancedFilterCategory[];
    searchFilters: (categories: AdvancedFilterCategory[]) => void;
}

export interface State {
    currentFilter: AdvancedFilterCategory[];
}

export class FilterISPComponent extends React.Component<Props, State> {
    constructor(props: Props) {
        super(props);
        
        this.state = {
            currentFilter: readUrl(this.props.defaultFilter)
        } 
        
    }

    searchHandler(filters: AdvancedFilterCategory[]) {
        //update URL
        updateUrl(filters);
        this.state = {
            currentFilter: filters
        }

        //conduct callback search
        this.props.searchFilters(filters);
    }

    render() {
        return (
            <AdvancedFilterContainer filterOptions={...this.state.currentFilter} onClick={this.searchHandler} />
        );
    }
}