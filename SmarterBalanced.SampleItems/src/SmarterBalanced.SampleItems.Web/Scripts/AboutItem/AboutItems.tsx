import * as React from 'react';
import * as ReactDOM from 'react-dom';
import * as $ from 'jquery';
import * as AboutThisItem from './AboutThisItem';
import { ItemFrame } from './ItemViewerFrame';

interface InteractionType {
    code: string;
    label: string;
    description: string;
    order?: number;
}

interface AboutItemsViewModel {
    interactionTypes: InteractionType[];
    itemUrl: string;
    selectedInteractionTypeCode: string;
    aboutThisItemViewModel: AboutThisItem.Props;
}

namespace AboutItems {

    interface State {
        selectedCode: string;
        itemUrl: string;
        aboutThisItemViewModel: AboutThisItem.Props;
    }

    export class AIComponent extends React.Component<AboutItemsViewModel, State>{
        constructor(props: AboutItemsViewModel) {
            super(props);

            this.state = {
                selectedCode: this.props.selectedInteractionTypeCode,
                itemUrl: this.props.itemUrl,
                aboutThisItemViewModel: this.props.aboutThisItemViewModel
            };
        }

        handleChange = (e: React.FormEvent<HTMLSelectElement>) => {
            const newCode = e.currentTarget.value
            if (newCode === this.state.selectedCode) {
                return;
            }

            this.fetchUpdatedViewModel(newCode);
        }

        fetchUpdatedViewModel(newCode: string) {
            const params = {
                interactionTypeCode: newCode
            };

            $.ajax({
                dataType: "JSON",
                type: "GET",
                url: "/AboutItems/GetItemUrl",
                data: params,
                success: this.onFetchedUpdatedViewModel
            });
        }

        onFetchedUpdatedViewModel = (viewModel: AboutItemsViewModel) => {
            if (!viewModel) {
                console.log("An error occurred updating the item.");
                return;
            }

            this.setState({
                itemUrl: viewModel.itemUrl,
                selectedCode: viewModel.selectedInteractionTypeCode,
                aboutThisItemViewModel: viewModel.aboutThisItemViewModel
            });
        }

        renderDescription() {
            let desc = "";
            for (let it of this.props.interactionTypes) {
                if (it.code === this.state.selectedCode) {
                    desc = it.description;
                }
            }

            return (
                <div aria-live="polite" aria-relevant="text"
                    dangerouslySetInnerHTML={{ __html: desc }} className="aboutitems-desc" />
            );
        }

        renderInteractionTypesSelect() {
            let items: JSX.Element[] = [];
            for (let i of this.props.interactionTypes) {
                items.push(
                    <option key={i.code} value={i.code}> {i.label} </option>
                );
            }

            return (
                <select className="form-control" onChange={this.handleChange}>
                    {items}
                </select>
            );
        }

        openAboutItemModal(e: React.KeyboardEvent<HTMLAnchorElement>) {
            if (e.keyCode === 13 || e.keyCode === 23) {
                const modal: any = ($("#about-item-modal-container"));
                modal.modal();
            }
        }

         renderNoItem() {
                return (
                    <div className="no-item">
                        <p>No items of the selected type found.</p>
                    </div>
                );
         }

        renderItemFrame() {
            return (
                <div className="aboutitem-iframe" aria-live="polite" aria-relevant="additions removals" >
                     <div className="item-nav" role="toolbar" aria-label="Toolbar with button groups">
                        <div className="item-nav-left-group" role="group" aria-label="First group">
                        <a className="item-nav-btn" data-toggle="modal" data-target="#about-item-modal-container"
                            onKeyUp={e => this.openAboutItemModal(e)} role="button" tabIndex={0}>
                            <span className="glyphicon glyphicon-info-sign glyphicon-pad" aria-hidden="true" />
                            About This Item
                        </a>
                      </div>
                    </div>
                    <ItemFrame url={this.state.itemUrl} />
                    <AboutThisItem.ATIComponent {...this.state.aboutThisItemViewModel} />
                </div>
            );
        }
        render() {
            const itemFrame = this.state.itemUrl ? this.renderItemFrame() : this.renderNoItem();
            return (
                <div className="aboutitems-parents">
                    <div className="aboutitems-info">
                        <h1>About Test Items</h1>
                        <div className="aboutitems-text">
                            Smarter Balanced assessments use a variety of item
                             types to accurately measure what students know and can do.
                             To learn more and see an example item, select an item type below.
                        </div>
                        <div className="aboutitems-dropdown form-group">
                            {this.renderInteractionTypesSelect()}
                        </div>
                        {this.renderDescription()}
                    </div>
                    {itemFrame}
                </div>
            );
        }
    }
}


export function initializeAboutItems(viewModel: AboutItemsViewModel) {
    ReactDOM.render(
        <AboutItems.AIComponent {...viewModel} />,
        document.getElementById("about-items") as HTMLElement
    );
}