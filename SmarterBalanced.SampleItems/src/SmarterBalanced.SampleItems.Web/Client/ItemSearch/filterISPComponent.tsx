import * as React from 'react';
import { mockAdvancedFilterCategories } from './filterModels';
import { AdvancedFilterCategoryModel, AdvancedFilterContainer } from '@osu-cass/sb-components';
import { updateUrl, readUrl } from '../UrlHelper';


export interface Props {
    defaultFilter: AdvancedFilterCategoryModel[];
    searchFilters: (categories: AdvancedFilterCategoryModel[]) => void;
}

export interface State {
    currentFilter: AdvancedFilterCategoryModel[];
}

export class FilterISPComponent extends React.Component<Props, State> {
    constructor(props: Props) {
        super(props);
        
        this.state = {
            currentFilter: readUrl(this.props.defaultFilter)
        } 
        this.props.searchFilters(this.state.currentFilter);
    }

    searchHandler = (filters: AdvancedFilterCategoryModel[]) => {
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