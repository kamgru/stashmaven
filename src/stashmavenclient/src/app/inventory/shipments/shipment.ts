export class Shipment {
    shipmentId: string;
    stockpileId: string;
    kindName: string;
    kindShortCode: string;
    currency: string;
    partner: ShipmentPartner | null = null;

    constructor(
        shipmentId: string,
        stockpileId: string,
        kindName: string,
        kindShortCode: string,
        currency: string,
        partner?: ShipmentPartner
    ) {
        this.shipmentId = shipmentId;
        this.stockpileId = stockpileId;
        this.kindName = kindName;
        this.kindShortCode = kindShortCode;
        this.currency = currency;
        this.partner = partner || null;
    }
}

export class ShipmentPartner {
    partnerId: string;
    customIdentifier: string;
    legalName: string;
    address: string;

    constructor(
        partnerId: string,
        customIdentifier: string,
        legalName: string,
        address: string,
    ) {
        this.partnerId = partnerId;
        this.customIdentifier = customIdentifier;
        this.legalName = legalName;
        this.address = address;
    }
}