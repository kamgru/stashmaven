import {Component, ViewChild} from '@angular/core';
import {CommonModule} from '@angular/common';
import {IShipmentKind, IStockpile, ShipmentsService} from "./shipments.service";
import {AddShipmentComponent} from "./add-shipment/add-shipment.component";
import {Shipment} from "./shipment";

enum UiState {
    Add = 'add',
    List = 'list',
    View = 'view'
}

@Component({
    selector: 'app-shipments',
    standalone: true,
    imports: [CommonModule, AddShipmentComponent],
    templateUrl: './shipments.component.html',
    styleUrls: ['./shipments.component.css']
})
export class ShipmentsComponent {

    @ViewChild(AddShipmentComponent) addShipmentComponent?: AddShipmentComponent;

    uiState = UiState.List;

    currentStockpile?: IStockpile;
    shipmentKinds: IShipmentKind[] = [];


    constructor(
        private shipmentsService: ShipmentsService
    ) {
    }

    ngOnInit() {
        this.shipmentsService.getDefaultStockpile()
            .subscribe(data => {
                this.currentStockpile = data;
            });
        this.shipmentsService.listShipmentKinds()
            .subscribe(data => {
                this.shipmentKinds = data.items;
            });
    }

    onAdd(kind: IShipmentKind) {

        this.uiState = UiState.Add;
        const req = {
            stockpileId: this.currentStockpile!.stockpileId,
            shipmentKindId: kind.shipmentKindId,
            currency: 'Pln'
        };
        this.shipmentsService.addShipment(req)
            .subscribe(data => {
                this.addShipmentComponent!.shipment = new Shipment(
                    data.shipmentId,
                    this.currentStockpile!.stockpileId,
                    kind.name,
                    kind.shortCode,
                    'Pln')
            });
    }
}
