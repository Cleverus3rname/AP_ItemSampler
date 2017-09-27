﻿import * as React from 'react';
import * as Collapsible from './Collapsible';
import * as AboutItemModels from './AboutItemModels';
import * as Rubric from './Rubric';
import * as AboutItemDetail from './AboutItemDetail';

export interface Props extends AboutItemModels.AboutThisItemViewModel {
 
}

export class AboutThisItemComponent extends React.Component<Props, {}> {
    render() {
        if (!this.props.rubrics) {
            return null;
        }
        const rubrics = this.props.rubrics.map((ru, i) => <Rubric.RubricComponent {...ru} key={String(i)} />);
        return (
            <div className="modal fade"
                id="about-item-modal-container"
                tabIndex={-1} role="dialog"
                aria-labelledby="About Item Modal"
                aria-hidden="true">

                <div className="modal-dialog about-item-modal" role="document">
                    <div className="modal-content">
                        <div className="modal-header">
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                            <h4 className="modal-title">About This Item</h4>
                        </div>
                        <div className="modal-body">
                            <AboutItemDetail.AboutThisItemDetailComponent {...this.props} />
                        </div>
                        <div className="modal-footer">
                            <button className="btn btn-primary" form="accessibility-form" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}