﻿interface AccessibilityResource {
    resourceCode: string; // ID for this resource
    defaultSelection: string;
    description: string;
    disabled: boolean;
    label: string;
    currentSelectionCode: string; // ID of the current selection
    order: number;
    selections: Dropdown.Selection[];
}

namespace ItemPage {

    function toiSAAP(accResourceGroups: AccResourceGroup[]): string {
        let str = "";
        for (let group of accResourceGroups) {
            for (let res of group.accessibilityResources) {
                if (res.currentSelectionCode && !res.disabled) {
                    str += res.currentSelectionCode + ";";
                }
            }
        }
        return encodeURIComponent(str);
    }

    function resetResource(model: AccessibilityResource): AccessibilityResource {
        const newModel = Object.assign({}, model);
        newModel.currentSelectionCode = model.defaultSelection;
        return newModel;
    }

    function trimAccResource(resource: AccessibilityResource): { label: string, selectedCode: string } {
        return {
            label: resource.label,
            selectedCode: resource.currentSelectionCode,
        };
    }

    function toCookie(accGroups: AccResourceGroup[]): string {
        let prefs: AccessibilityModal.ResourceSelections = {};
        for (const group of accGroups) {
            for (const resource of group.accessibilityResources) {
                prefs[resource.resourceCode] = resource.currentSelectionCode;
            }
        }

        const json = JSON.stringify(prefs);
        const cookie = btoa(json);
        return cookie;
    }

    function addDisabledPlaceholder(resource: AccessibilityResource): AccessibilityResource {
        if (resource.disabled) {
            let newSelection = Object.assign(resource, resource);
            let disabledOption: Dropdown.Selection = {
                label: "Disabled for item",
                selectionCode: "",
                disabled: true,
                order: 0,
            };
            newSelection.selections.push(disabledOption);
            newSelection.currentSelectionCode = "";
            return newSelection;
        }
        return resource;
    }

    export function getResourceTypes(resourceGroups: AccResourceGroup[]): string[] {
        let resourceTypes: string[] = [];
        for (const group of resourceGroups) {
            if (resourceTypes.indexOf(group.label) === -1) {
                resourceTypes.push(group.label);
            }
        }
        return resourceTypes;
    }

    export interface AccResourceGroup {
        label: string;
        order: number;
        accessibilityResources: AccessibilityResource[];
    }



    export interface ViewModel {
        itemViewerServiceUrl: string;
        accessibilityCookieName: string;
        accResourceGroups: AccResourceGroup[];
        moreLikeThisVM: MoreLikeThis.Props;
        aboutThisItemVM: AboutThisItem.Props;
    }

    export interface Props extends ViewModel {
        onSave: (selections: AccessibilityModal.ResourceSelections) => void;
        onReset: () => void;
    }

    interface State { }

    export class Page extends React.Component<Props, State> {
        constructor(props: Props) {
            super(props);
        }

        saveOptions = (resourceSelections: AccessibilityModal.ResourceSelections): void => {
            this.props.onSave(resourceSelections);
        }

        openAboutItemModal(e: React.KeyboardEvent<HTMLAnchorElement>) {
            if (e.keyCode === 13 || e.keyCode === 23) {
                const modal: any = ($("#about-item-modal-container"));
                modal.modal();
            }
        }

        openMoreLikeThisModal(e: React.KeyboardEvent<HTMLAnchorElement>) {
            if (e.keyCode === 13 || e.keyCode === 23) {
                const modal: any = ($("#more-like-this-modal-container"));
                modal.modal();
            }
        }

        openShareModal(e: React.KeyboardEvent<HTMLAnchorElement>) {
            if (e.keyCode === 13 || e.keyCode === 23) {
                const modal: any = ($("#share-modal-container"));
                modal.modal();
            }
        }

        openAccessibilityModal(e: React.KeyboardEvent<HTMLAnchorElement>) {
            if (e.keyCode === 13 || e.keyCode === 23) {
                const modal: any = ($("#accessibility-modal-container"));
                modal.modal();
            }
        }

        render() {
            let isaap = toiSAAP(this.props.accResourceGroups);
            let ivsUrl: string = this.props.itemViewerServiceUrl.concat("&isaap=", isaap);
            var windowWidth = 600;
            const accText = (window.innerWidth < 350) ? "" : "Accessibility";
            const abtText = (window.innerWidth < windowWidth) ? "About" : "About This Item";
            const moreText = (window.innerWidth < windowWidth) ? "More" : "More Like This";
            return (
                <div>
                    <div className="btn-toolbar item-nav-group" role="toolbar" aria-label="Toolbar with button groups">
                        <div className="btn-group mr-2 item-nav-bottom" role="group" aria-label="First group">

                            <a className="btn item-nav-btn" data-toggle="modal" data-target="#about-item-modal-container"
                                onKeyUp={e => this.openAboutItemModal(e)} tabIndex={0}>
                                <span className="glyphicon glyphicon-info-sign glyphicon-pad" aria-hidden="true"></span>
                                {abtText}
                            </a>

                            <a className="btn item-nav-btn" data-toggle="modal" data-target="#more-like-this-modal-container"
                                onKeyUp={e => this.openMoreLikeThisModal(e)} tabIndex={0}>
                                <span className="glyphicon glyphicon-th-large glyphicon-pad" aria-hidden="true"></span>
                                {moreText}                               
                            </a>

                            <a className="btn item-nav-btn" data-toggle="modal" data-target="#share-modal-container"
                                onKeyUp={e => this.openShareModal(e)} tabIndex={0}>
                                <span className="glyphicon glyphicon-share-alt glyphicon-pad" aria-hidden="true"></span>
                                Share
                            </a>
                        </div>

                        <div className="btn-group mr-2 pull-right" role="group" aria-label="Second group">
                            <a type="button" className="accessibility-btn btn btn-primary" data-toggle="modal"
                                data-target="#accessibility-modal-container"
                                onKeyUp={e => this.openAccessibilityModal(e)} tabIndex={0}>
                                <span className="glyphicon glyphicon-collapse-down" aria-hidden="true"></span>
                                {accText}
                            </a>
                        </div>
                    </div>
                    <ItemFrame url={ivsUrl} />
                    <AboutThisItem.ATIComponent {...this.props.aboutThisItemVM} />
                    <AccessibilityModal.ItemAccessibilityModal
                        accResourceGroups={this.props.accResourceGroups}
                        onSave={this.props.onSave}
                        onReset={this.props.onReset} />
                        <MoreLikeThis.Modal {...this.props.moreLikeThisVM} />
                    <Share.ShareModal iSAAP={isaap}/>
                </div>
            );
        }
    }

    export class Controller {
        constructor(private itemProps: ViewModel, private rootDiv: HTMLDivElement) { }

        onSave = (selections: AccessibilityModal.ResourceSelections) => {

            const newGroups: AccResourceGroup[] = [];
            for (let group of this.itemProps.accResourceGroups) {
                const newGroup = Object.assign({}, group);
                const newResources: AccessibilityResource[] = [];
                for (let res of newGroup.accessibilityResources) {
                    const newRes = Object.assign({}, res);
                    newRes.currentSelectionCode = selections[newRes.label] || newRes.currentSelectionCode;
                    newResources.push(newRes);
                }
                newGroup.accessibilityResources = newResources;
                newGroups.push(newGroup);
            }
            this.itemProps = Object.assign({}, this.itemProps);
            this.itemProps.accResourceGroups = newGroups;

            let cookieValue = toCookie(this.itemProps.accResourceGroups);
            document.cookie = this.itemProps.accessibilityCookieName.concat("=", cookieValue, "; path=/");

            this.render();
        }

        onReset = () => {
            document.cookie = this.itemProps.accessibilityCookieName.concat("=", "", "; path=/");
            
            const newAccResourceGroups = this.itemProps.accResourceGroups.map(g => {
                const newGroup = Object.assign({}, g);
                newGroup.accessibilityResources = newGroup.accessibilityResources.map(resetResource);
                return newGroup;
            });

            this.itemProps = Object.assign({}, this.itemProps);
            this.itemProps.accResourceGroups = newAccResourceGroups;
            
            this.render();
        }

        render() {
            ReactDOM.render(
                <Page {...this.itemProps} onSave={this.onSave} onReset={this.onReset} />,
                this.rootDiv);
        }
    }
}

function initializeItemPage(itemProps: ItemPage.ViewModel) {
    const controller = new ItemPage.Controller(
        itemProps,
        document.getElementById("item-container") as HTMLDivElement);
    controller.render();
}
