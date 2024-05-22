import {Component} from '@angular/core';
import {AddStockpileComponent} from "../stockpiles/add-stockpile/add-stockpile.component";
import {FaIconComponent, FaIconLibrary} from "@fortawesome/angular-fontawesome";
import {ListStockpilesComponent} from "../../common/components/list-stockpiles/list-stockpiles.component";
import {Notification, NotificationComponent} from "../../common/components/notification/notification.component";
import {ListShipmentKindsComponent} from "../../common/components/list-shipment-kinds/list-shipment-kinds.component";
import {IShipmentKindItem} from "../../common/components/list-shipment-kinds/list-shipment-kinds.service";
import {AddShipmentKindComponent, ShipmentKindAddedEvent} from "./add-shipment-kind/add-shipment-kind.component";
import {AddShipmentKindRequest, ShipmentKindService} from "../../common/services/shipment-kind.service";
import {AsyncPipe} from "@angular/common";
import {faPlus} from "@fortawesome/free-solid-svg-icons";

@Component({
    selector: 'app-shipment-kinds',
    standalone: true,
    imports: [
        AddStockpileComponent,
        FaIconComponent,
        ListStockpilesComponent,
        NotificationComponent,
        ListShipmentKindsComponent,
        AddShipmentKindComponent,
        AsyncPipe
    ],
    templateUrl: './shipment-kinds.component.html'
})
export class ShipmentKindsComponent {

    public uiState: 'list' | 'add' = 'list';

    public notification: Notification | null = null;

    public directions$ = this.shipmentKindsService.getDirections();

    constructor(
        private shipmentKindsService: ShipmentKindService,
        fa: FaIconLibrary
    ) {
        fa.addIcons(faPlus);
    }

    handleItemConfirmed($event: IShipmentKindItem) {

    }

    handleShipmentKindAdded($event: ShipmentKindAddedEvent) {
        const direction = $event.direction as 'In' | 'Out';
        const req = new AddShipmentKindRequest($event.name, $event.shortCode, direction);

        this.shipmentKindsService.addShipmentKind(req)
            .subscribe(() => {
                this.uiState = 'list';
                this.notification = null;
            });
    }
}
