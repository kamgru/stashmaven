import {Component, OnDestroy} from '@angular/core';
import * as li from "../list-items";
import * as br from "./list-shipment-kinds.service";
import {AsyncPipe, NgTemplateOutlet} from "@angular/common";
import {ListItemsLayoutComponent} from "../list-items";

@Component({
    selector: 'app-list-shipment-kinds',
    standalone: true,
    imports: [
        AsyncPipe,
        ListItemsLayoutComponent,
        NgTemplateOutlet
    ],
    templateUrl: './list-shipment-kinds.component.html',
})
export class ListShipmentKindsComponent
    extends li.ListItemsBaseComponent<br.IShipmentKindItem, br.ListShipmentKindsRequest, br.IListShipmentKindsResponse, br.ListShipmentKindsService>
    implements OnDestroy {

    constructor(
        listShipmentKinds: br.ListShipmentKindsService,
    ) {
        super();
        this.bootstrap(listShipmentKinds);
    }
}
