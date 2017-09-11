export interface SubjectClaims {
    [subject: string]: { text: string; value: string }[];
}

export interface InteractionType {
    code: string;
    label: string;
}

export interface Subject {
    code: string;
    label: string;
    claims: Claim[];
    interactionTypeCodes: string[];
}

export interface Claim {
    code: string;
    label: string;
    targets: Target[];
}


export interface Target {
    name: string;
    nameHash: number;
}
