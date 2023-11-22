export class PartnerAddress {
    constructor(
        public street: string,
        public city: string,
        public postalCode: string,
        public countryCode: string,
        public streetAdditional?: string,
        public state?: string,
    ) {
    }
}