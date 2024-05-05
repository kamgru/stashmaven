import {Component} from '@angular/core';
import {AsyncPipe} from "@angular/common";
import {DropdownComponent, IDropdownItem} from "../../common/components/dropdown/dropdown.component";
import {ListShipmentsComponent} from "../../common/components/list-shipments/list-shipments.component";
import {IGetShipmentResponse, ShipmentService} from "../../common/services/shipment.service";
import {StockpileService} from "../../common/services/stockpile.service";
import {ActivatedRoute, Router} from "@angular/router";
import {combineLatest, map, Observable} from "rxjs";
import {IStockpileListItem} from "../../common/IStockpileListItem";
import {IShipment} from "../../common/components/list-shipments/list-shipments.service";
import {FaIconComponent} from "@fortawesome/angular-fontawesome";

@Component({
    selector: 'app-shipments',
    standalone: true,
    imports: [
        AsyncPipe,
        DropdownComponent,
        ListShipmentsComponent,
        FaIconComponent,
    ],
    templateUrl: './shipments.component.html',
})
export class ShipmentsComponent {

    private _shipmentKinds: Observable<IDropdownItem[]> = this.shipmentService.listShipmentKinds()
        .pipe(map(response => response.items.map(item => <IDropdownItem>{id: item.shipmentKindId, name: item.name})));

    private _stockpiles$: Observable<IStockpileListItem[]> = this.stockpileService.listStockpiles()
        .pipe(map(response => response.items));

    public data$ = combineLatest([this._shipmentKinds, this._stockpiles$])
        .pipe(map(([shipmentKinds, stockpiles]) => (
            <{ shipmentKinds: IDropdownItem[], stockpiles: IStockpileListItem[] }>{ shipmentKinds, stockpiles })));


    public selectedShipment?: IGetShipmentResponse | null;
    public selectedStockpile?: IStockpileListItem | null;

    constructor(
        private shipmentService: ShipmentService,
        private stockpileService: StockpileService,
        private router: Router,
        private route: ActivatedRoute
    ) {
    }

    handleShipmentKindSelected($event: string) {

    }

    handleStockpileChanged($event: IStockpileListItem) {
        this.selectedStockpile = $event;
    }

    handleShipmentSelected($event: IShipment) {
    }

    deleteShipment($event: MouseEvent) {

    }

    handleItemConfirmed($event: IShipment) {
        this.router.navigate([`./${$event.shipmentId}`], {relativeTo: this.route});
    }
}
