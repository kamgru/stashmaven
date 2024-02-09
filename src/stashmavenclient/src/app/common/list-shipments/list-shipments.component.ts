import {Component, EventEmitter, HostListener, Input, OnDestroy, Output} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {IShipment, ListShipmentsService} from "./list-shipments.service";

import {IStockpileListItem} from "../IStockpileListItem";
import {IInventoryItem} from "../list-inventory/list-inventory.service";
import {AsyncPipe} from "@angular/common";

@Component({
    selector: 'app-list-shipments',
    standalone: true,
    imports: [
        AsyncPipe
    ],
    templateUrl: './list-shipments.component.html',
    styleUrl: './list-shipments.component.css'
})
export class ListShipmentsComponent implements OnDestroy {

    private _destroy$ = new Subject<void>();

    @Input()
    public stockpiles: IStockpileListItem[] = [];

    @Output()
    public OnShipmentSelected: EventEmitter<IShipment> = new EventEmitter<IShipment>();

    @Output()
    public OnStockpileChanged: EventEmitter<IStockpileListItem> = new EventEmitter<IStockpileListItem>();

    @HostListener('window:keydown', ['$event'])
    keyEvent(event: KeyboardEvent) {
        if (this.listShipments.tryHandleKey(event)) {
            return;
        }
    }

    public totalPages_$ = this.listShipments.totalPages_$;
    public currentIndex_$ = this.listShipments.currentIndex_$;
    public pageSize_$ = this.listShipments.pageSize_$;
    public page_$ = this.listShipments.page_$;
    public shipments$ = this.listShipments.shipments$;

    constructor(
        private listShipments: ListShipmentsService,
    ) {
        this.listShipments.selectedShipment$
            .pipe(
                takeUntil(this._destroy$))
            .subscribe(x => {
                this.OnShipmentSelected.emit(x);
            });
    }

    public changePageSize = (value: number) => this.listShipments.changePageSize(value);
    public tryNextPage = () => this.listShipments.tryNextPage();
    public tryPrevPage = () => this.listShipments.tryPrevPage();

    handleRowClick(index: number, shipment: IShipment) {
        this.currentIndex_$.set(index);
        this.listShipments.selectedShipment$.next(shipment);
    }

    ngOnDestroy() {
        this._destroy$.next();
        this._destroy$.complete();
    }

    handleStockpileChanged(value: string) {
        this.listShipments.changeStockpile(value);
    }

    sortBy(sku: string) {

    }
}
