import {Component, ElementRef, EventEmitter, HostListener, Input, OnDestroy, Output, ViewChild} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {IInventoryItem, ListInventoryService} from "./list-inventory.service";
import {AsyncPipe} from "@angular/common";

export interface IStockpileListItem {
    stockpileId: string;
    name: string;
    shortCode: string;
    isDefault: boolean;
}

@Component({
    selector: 'app-list-inventory',
    standalone: true,
    imports: [
        AsyncPipe
    ],
    templateUrl: './list-inventory.component.html',
    styleUrl: './list-inventory.component.css'
})
export class ListInventoryComponent implements OnDestroy {

    private _destroy$ = new Subject<void>();

    @ViewChild('searchInput')
    private _searchInput?: ElementRef<HTMLInputElement>;

    @Output()
    public OnInventoryItemSelected: EventEmitter<IInventoryItem> = new EventEmitter<IInventoryItem>();

    @Input()
    public stockpiles: IStockpileListItem[] = [];

    @HostListener('window:keydown', ['$event'])
    keyEvent(event: KeyboardEvent) {
        if (this.listInventory.tryHandleKey(event)) {
            return;
        }

        if (event.ctrlKey && event.key == '/') {
            this._searchInput?.nativeElement.focus();
            event.preventDefault();
        } else if (event.key == 'Escape') {
            this._searchInput?.nativeElement.blur();
        }
    }

    public totalPages_$ = this.listInventory.totalPages_$;
    public currentIndex_$ = this.listInventory.currentIndex_$;
    public pageSize_$ = this.listInventory.pageSize_$;
    public page_$ = this.listInventory.page_$;
    public search_$ = this.listInventory.search_$;
    public inventoryItems$ = this.listInventory.inventoryItems$;

    constructor(
        private listInventory: ListInventoryService,
    ) {
        this.listInventory.selectedInventoryItem$
            .pipe(
                takeUntil(this._destroy$))
            .subscribe(x => {
                this.OnInventoryItemSelected.emit(x);
            });

        // this.listInventory.getDefaultStockpileId()
        //     .subscribe(res => {
        //         this.listInventory.changeStockpile(res.stockpileId);
        //     })
    }

    public changePageSize = (value: number) => this.listInventory.changePageSize(value);
    public tryNextPage = () => this.listInventory.tryNextPage();
    public tryPrevPage = () => this.listInventory.tryPrevPage();
    public sortBy = (value: string) => this.listInventory.sortBy(value);
    public search = (value: string) => this.listInventory.search(value);

    handleRowClick(index: number, inventoryItem: IInventoryItem) {
        this.currentIndex_$.set(index);
        this.listInventory.selectedInventoryItem$.next(inventoryItem);
    }

    ngOnDestroy() {
        this._destroy$.next();
        this._destroy$.complete();
    }
}
