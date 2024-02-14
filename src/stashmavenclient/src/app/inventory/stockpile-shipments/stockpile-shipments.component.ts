import {Component, OnInit, ViewChild} from '@angular/core';
import {AsyncPipe} from "@angular/common";
import {ListShipmentsComponent} from "../../common/list-shipments/list-shipments.component";
import {forkJoin, map, Subject} from "rxjs";
import {StockpileService} from "../../common/services/stockpile.service";
import {IStockpileListItem} from "../../common/IStockpileListItem";
import {AddShipmentRequest, ShipmentService} from "../../common/services/shipment.service";
import {DropdownComponent, IDropdownItem} from "../../common/components/dropdown/dropdown.component";
import {ActivatedRoute, Router} from "@angular/router";
import {IShipment} from "../../common/list-shipments/list-shipments.service";

@Component({
    selector: 'app-stockpile-shipments',
    standalone: true,
    imports: [
        AsyncPipe,
        ListShipmentsComponent,
        DropdownComponent
    ],
    templateUrl: './stockpile-shipments.component.html',
    styleUrl: './stockpile-shipments.component.css'
})
export class StockpileShipmentsComponent implements OnInit {

    private _selectedStockpile?: IStockpileListItem;
    private _selectedShipment?: IShipment;

    public stockpiles: IStockpileListItem[] = [];
    public shipmentKinds$ = this.shipmentService.listShipmentKinds()
        .pipe(
            map(x => {
                return x.items.map(y => {
                    return <IDropdownItem>{
                        id: y.shipmentKindId,
                        name: y.name
                    };
                });
            })
        );

    @ViewChild(ListShipmentsComponent)
    private _listShipments?: ListShipmentsComponent;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private stockpileService: StockpileService,
        private shipmentService: ShipmentService) {
    }

    ngOnInit(): void {
        forkJoin([
            this.stockpileService.listStockpiles(),
            this.stockpileService.getDefaultStockpileId()
        ])
            .pipe(
                map(([stockpiles, defaultStockpile]) => {
                        return stockpiles.items.map(x => {
                            return {
                                stockpileId: x.stockpileId,
                                name: x.name,
                                shortCode: x.shortCode,
                                isDefault: x.stockpileId === defaultStockpile.stockpileId
                            };
                        });
                    }
                ))
            .subscribe(x => {
                this.stockpiles = x;
                this.stockpiles.sort((a, b) => a.isDefault ? -1 : b.isDefault ? 1 : 0);
                this._selectedStockpile = this.stockpiles[0];
            })
    }

    handleShipmentKindSelected($event: string) {
        this.shipmentService.addShipment(new AddShipmentRequest(this._selectedStockpile!.stockpileId, $event, 'Pln'))
            .subscribe(response => {
                this.navigateToShipment(response.shipmentId);
            })
    }

    handleStockpileChanged($event: IStockpileListItem) {
        this._selectedStockpile = $event;
    }

    handleShipmentSelected($event: IShipment) {
        this._selectedShipment = $event;
    }

    private navigateToShipment(shipmentId: string) {
        this.router.navigate(['./', shipmentId], {relativeTo: this.route}).then(r => {});
    }

    deleteShipment($event: MouseEvent) {
        if (!this._selectedShipment) {
            return;
        }

        this.shipmentService.deleteShipment(this._selectedShipment.shipmentId)
            .subscribe(_ => {
                this._listShipments?.reload();
            })
    }

    handlerShipmentConfirmed($event: IShipment) {
        this.navigateToShipment($event.shipmentId);
    }
}
