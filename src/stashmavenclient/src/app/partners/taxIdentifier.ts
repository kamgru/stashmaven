import {TaxIdentifierType} from "./taxIdentifierType";

export class TaxIdentifier {
    constructor(
        public type: TaxIdentifierType,
        public value: string,
        public isPrimary: boolean,
    ) {
    }
}
