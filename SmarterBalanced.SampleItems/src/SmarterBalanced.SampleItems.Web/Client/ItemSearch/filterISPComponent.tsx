import * as React from 'react';
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
        
    }

    searchHandler = (filters: AdvancedFilterCategoryModel[]) => {
        //update URL
        //conduct callback search
        this.props.searchFilters(filters);
    }

    render() {
        return (
            <div style={{ borderRadius: "5px", "backgroundColor": "white"}}>
                <h1 style={{ padding: "10px"}}>Browse Items</h1>
                <div>
                    <AdvancedFilterContainer filterOptions={...this.props.defaultFilter} onClick={this.searchHandler} />
                </div>
            </div>
        );
    }
}