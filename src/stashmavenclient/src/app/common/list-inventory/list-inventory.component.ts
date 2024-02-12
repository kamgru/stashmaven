import {
    Component,
    ElementRef,
    EventEmitter,
    HostListener,
    Input, OnDestroy,
    Output,
    ViewChild
} from '@angular/core';
import {debounceTime, distinctUntilChanged, Observable, Subject, takeUntil, tap} from "rxjs";
import {IInventoryItem, IListInventoryItemsResponse, ListInventoryService} from "./list-inventory.service";
import {AsyncPipe} from "@angular/common";
import {IStockpileListItem} from "../IStockpileListItem";
import {FormsModule} from "@angular/forms";
import {IListRequest, IListResponse, ListServiceBase} from "../ListServiceBase";


export class ListItemsComponentBase<TItem, TListRequest extends IListRequest, TListResponse extends IListResponse<any>> {

    private _destroy$ = new Subject<void>();

    @ViewChild('searchInput')
    private _searchInput?: ElementRef<HTMLInputElement>;

    @Output()
    public OnItemSelected = new EventEmitter<TItem>();

    @HostListener('window:keydown', ['$event'])
    keyEvent(event: KeyboardEvent) {
        if (this.listService.tryHandleKey(event)) {
            return;
        }

        if (event.ctrlKey && event.key == '/') {
            this._searchInput?.nativeElement.focus();
            event.preventDefault();
        } else if (event.key == 'Escape') {
            this._searchInput?.nativeElement.blur();
        }
    }

    constructor(
        private listService: ListServiceBase<TItem, TListRequest, TListResponse>
    ) {
        this.listService.selectedItem$
            .pipe(
                takeUntil(this._destroy$))
            .subscribe(item => {
                if (item) {
                    this.OnItemSelected.emit(item);
                }
            });
    }
}

@Component({
    selector: 'app-list-inventory',
    standalone: true,
    imports: [
        AsyncPipe,
        FormsModule
    ],
    templateUrl: './list-inventory.component.html',
    styleUrl: './list-inventory.component.css',
})
export class ListInventoryComponent implements OnDestroy {

    private _destroy$ = new Subject<void>();

    @ViewChild('searchInput')
    private _searchInput?: ElementRef<HTMLInputElement>;

    @Output()
    public OnItemSelected: EventEmitter<IInventoryItem> = new EventEmitter<IInventoryItem>();

    @Input()
    public stockpiles: IStockpileListItem[] = [];

    public selectedStockpile?: IStockpileListItem;

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

    public currentIndex_$ = this.listInventory.currentIndex_$;
    public inventoryItems$: Observable<IListInventoryItemsResponse>;

    private _search$ = new Subject<string>();

    constructor(
        private listInventory: ListInventoryService,
    ) {
        this.listInventory.selectedItem$
            .pipe(
                takeUntil(this._destroy$))
            .subscribe(item => {
                if (item) {
                    this.OnItemSelected.emit(item);
                }
            });


        this.inventoryItems$ = this.listInventory.items$
            .pipe(
                tap(x => {
                    this.selectedStockpile = this.stockpiles.find(y => y.stockpileId === x.stockpileId);
                })
            );

        this._search$.pipe(
            distinctUntilChanged(),
            debounceTime(500),
        )
            .subscribe(x => {
                this.listInventory.search(x);
            })
    }

    public changePageSize = (value: number) => this.listInventory.changePageSize(value);
    public tryNextPage = () => this.listInventory.tryNextPage();
    public tryPrevPage = () => this.listInventory.tryPrevPage();
    public sortBy = (value: string) => this.listInventory.sortBy(value);
    public search = (value: string) => this._search$.next(value);
    public handleRowClick = (inventoryItem: IInventoryItem) => this.listInventory.select(inventoryItem);

    ngOnDestroy() {
        this._destroy$.next();
        this._destroy$.complete();
    }

    handleStockpileChanged(value: string) {
        this.listInventory.changeStockpile(value);
    }
}
