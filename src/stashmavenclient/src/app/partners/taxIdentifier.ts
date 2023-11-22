import {TaxIdentifierType} from "./taxIdentifierType";

export class TaxIdentifier {
    constructor(
        public taxIdentifierType: TaxIdentifierType,
        public value: string,
        public isPrimary: boolean,
    ) {
    }
}