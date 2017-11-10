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
        this.props.searchFilters(this.state.currentFilter);
    }

    searchHandler = (filters: AdvancedFilterCategory[]) => {
        //update URL
        updateUrl(filters);
        this.setState({
            currentFilter: filters
        });

        //conduct callback search
        this.props.searchFilters(filters);
    }

    render() {
        return (
            <div style={{ borderRadius: "5px", "backgroundColor": "white"}}>
                <h1 style={{ padding: "10px"}}>Browse Items</h1>
                <div>
                    <AdvancedFilterContainer filterOptions={...this.state.currentFilter} onClick={this.searchHandler} />
                </div>
            </div>
        );
    }
}