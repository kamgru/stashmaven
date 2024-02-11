import {
    AfterViewInit,
    Component,
    ElementRef,
    EventEmitter,
    HostListener,
    Input,
    OnDestroy,
    Output,
    ViewChild
} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {IInventoryItem, ListInventoryService} from "./list-inventory.service";
import {AsyncPipe} from "@angular/common";
import {IStockpileListItem} from "../IStockpileListItem";
import {FormsModule} from "@angular/forms";

@Component({
    selector: 'app-list-inventory',
    standalone: true,
    imports: [
        AsyncPipe,
        FormsModule
    ],
    templateUrl: './list-inventory.component.html',
    styleUrl: './list-inventory.component.css'
})
export class ListInventoryComponent implements OnDestroy, AfterViewInit {

    private _destroy$ = new Subject<void>();

    @ViewChild('searchInput')
    private _searchInput?: ElementRef<HTMLInputElement>;

    @Output()
    public OnInventoryItemSelected: EventEmitter<IInventoryItem> = new EventEmitter<IInventoryItem>();

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

    }

    ngOnInit(): void {
    }

    ngAfterViewInit() {
        this.listInventory.changeStockpile(this.selectedStockpile!.stockpileId);
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

    handleStockpileChanged(value: string) {
        this.listInventory.changeStockpile(value);
    }
}
