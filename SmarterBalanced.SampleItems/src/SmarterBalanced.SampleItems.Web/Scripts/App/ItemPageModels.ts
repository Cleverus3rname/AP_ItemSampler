import * as Accessibility from '../Accessibility/Accessibility';
import * as MoreLikeThis from '../Modals/MoreLikeThisModal';

export interface ItemIdentifier {
    itemName: string;
    bankKey: number;
    itemKey: number;
}

export interface ItemPageViewModel {
    itemViewerServiceUrl: string;
    itemNames: string;
    brailleItemNames: string;
    brailleItem: ItemIdentifier;
    nonBrailleItem: ItemIdentifier;
    currentItem: ItemIdentifier;
    accessibilityCookieName: string;
    isPerformanceItem: boolean;
    performanceItemDescription: string;
    subject: string;
    accResourceGroups: Accessibility.AccResourceGroup[];
    moreLikeThisVM: MoreLikeThis.Props;
    brailleItemCodes: string[];
    braillePassageCodes: string[];
}