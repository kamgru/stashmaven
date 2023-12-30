export class Shipment {
    shipmentId: string;
    stockpileId: string;
    kindName: string;
    kindShortCode: string;
    currency: string;

    constructor(
        shipmentId: string,
        stockpileId: string,
        kindName: string,
        kindShortCode: string,
        currency: string,
    ) {
        this.shipmentId = shipmentId;
        this.stockpileId = stockpileId;
        this.kindName = kindName;
        this.kindShortCode = kindShortCode;
        this.currency = currency;
    }
}