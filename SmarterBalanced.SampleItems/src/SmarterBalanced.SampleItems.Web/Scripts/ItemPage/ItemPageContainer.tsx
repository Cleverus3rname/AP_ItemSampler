import * as React from 'react';
import * as ReactDOM from 'react-dom';
import * as Accessibility from '../Accessibility/Accessibility';
import * as AboutThisItem from '../AboutItem/AboutThisItem';
import * as ItemPage from './ItemPage';
import * as ItemPageModels from './ItemPageModels';
import * as Api from '../ApiModel';



const AboutThisItemViewModelClient = (params: { bankKey: number, itemKey: number }) => Api.get<AboutThisItem.Props>("/Item/AboutThisItemViewModel", params);
interface Props {
    itemPage: ItemPageModels.ItemPageViewModel;
}

interface State {
    aboutThisItem: Api.Resource<AboutThisItem.Props>
    itemPageVM: ItemPageModels.ItemPageViewModel;
}

export class ItemPageContainer extends React.Component<Props, State>{
    constructor(props: Props) {
        super(props);
        const itemPageVM = { ...props.itemPage };

        itemPageVM.currentItem = Accessibility.isBrailleEnabled(itemPageVM.accResourceGroups) ? itemPageVM.brailleItem : itemPageVM.nonBrailleItem;

        this.state = {
            aboutThisItem: { kind: "loading" },
            itemPageVM: itemPageVM
        }
        this.fetchUpdatedAboutThisItem();

    }

    onSave = (selections: Accessibility.ResourceSelections) => {

        const newGroups: Accessibility.AccResourceGroup[] = [];
        for (let group of this.props.itemPage.accResourceGroups) {
            const newGroup = { ...group };
            const newResources: Accessibility.AccessibilityResource[] = [];
            for (let res of newGroup.accessibilityResources) {
                const newRes = { ...res };
                newRes.currentSelectionCode = selections[newRes.resourceCode] || newRes.currentSelectionCode;
                newResources.push(newRes);
            }
            newGroup.accessibilityResources = newResources;
            newGroups.push(newGroup);
        }


        const itemPageVM = { ...this.state.itemPageVM };

        itemPageVM.accResourceGroups = newGroups;
        let cookieValue = ItemPageModels.toCookie(itemPageVM.accResourceGroups);
        document.cookie = itemPageVM.accessibilityCookieName.concat("=", cookieValue, "; path=/");
        itemPageVM.currentItem = Accessibility.isBrailleEnabled(newGroups) ? itemPageVM.brailleItem : itemPageVM.nonBrailleItem;

        this.fetchUpdatedAboutThisItem(); //TODO: why does this need to refetch?
        this.setState({
            itemPageVM: itemPageVM
        });
    }

    onReset = () => {
        const itemPageVM: ItemPageModels.ItemPageViewModel = { ...this.props.itemPage };
        document.cookie = itemPageVM.accessibilityCookieName.concat("=", "", "; path=/");

        const newAccResourceGroups = itemPageVM.accResourceGroups.map(g => {
            const newGroup = { ...g };
            newGroup.accessibilityResources = newGroup.accessibilityResources.map(ItemPageModels.resetResource);
            return newGroup;
        });


        itemPageVM.accResourceGroups = newAccResourceGroups;

        itemPageVM.currentItem = Accessibility.isBrailleEnabled(newAccResourceGroups) ? itemPageVM.brailleItem : itemPageVM.nonBrailleItem;
        this.fetchUpdatedAboutThisItem();

        this.setState({
            itemPageVM: itemPageVM
        });
    }

    fetchUpdatedAboutThisItem() {
        const item = this.state.itemPageVM.currentItem;

        const params = {
            bankKey: item.bankKey,
            itemKey: item.itemKey
        };

        AboutThisItemViewModelClient(params).then((data) => this.onFetchedUpdatedViewModel(data)).catch();
    }

    onFetchedUpdatedViewModel(viewModel: AboutThisItem.Props) {
        this.setState({
            aboutThisItem: { kind: "success", content: viewModel }
        });
    }

    onFetchUpdatedAboutError(err: any) {
        console.error(err);
        this.setState({
            aboutThisItem: { kind: "failure" }
        });
    }

    render() {
        const aboutThisItemResult = this.state.aboutThisItem;
        const ItemDetails = this.state.itemPageVM.currentItem;

        if ((aboutThisItemResult.kind === "success" || aboutThisItemResult.kind === "reloading") && aboutThisItemResult.content) {
            return <ItemPage.Page
                {...this.state.itemPageVM}
                aboutThisItemVM={aboutThisItemResult.content}
                onSave={this.onSave}
                onReset={this.onReset}
                currentItem={ItemDetails}
            />
        }
        else {
            return <div></div>
        }
    }
}

