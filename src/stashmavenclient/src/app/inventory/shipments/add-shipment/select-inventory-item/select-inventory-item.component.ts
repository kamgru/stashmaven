import {Component, ElementRef, HostListener, Input, Output, signal, ViewChild} from '@angular/core';
import {CommonModule} from '@angular/common';
import {
    IInventoryItem,
    IListInventoryItemsResponse, ListInventoryItemsRequest,
    StockpilesService
} from "../../../../common/services/stockpiles.service";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {debounceTime, distinctUntilChanged, Subject, switchMap} from "rxjs";
import {InputDetailsComponent} from "./input-details/input-details.component";

enum UiState {
    List = 'list',
    Select = 'select'
}

@Component({
    selector: 'app-select-inventory-item',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormsModule, InputDetailsComponent],
    templateUrl: './select-inventory-item.component.html',
    styleUrls: ['./select-inventory-item.component.css']
})
export class SelectInventoryItemComponent {
    private _stockpileId?: string;
    private _partnerId?: string;
    private _req: ListInventoryItemsRequest = new ListInventoryItemsRequest(1, 25, '', 'name', true);
    private delimiter?: HTMLElement;
    private observer?: IntersectionObserver;
    private kbdEvent$ = new Subject<KeyboardEvent>();
    private rowHeight = 33;
    private headerHeight = 40;
    private _selectedItem?: IInventoryItem;

    @ViewChild('searchInput')
    private searchInput?: ElementRef;
    @ViewChild('table')
    private table?: ElementRef;

    intersectionObserverCallback: IntersectionObserverCallback
        = (entries: IntersectionObserverEntry[]) => {
        for (let entry of entries) {
            if (entry.target === this.delimiter && entry.isIntersecting) {
                this.loadMore();
            }
        }
    };

    @HostListener('window:keydown', ['$event'])
    handleKeyDown(event: KeyboardEvent) {
        this.kbdEvent$.next(event);
    }

    public uiState: UiState = UiState.List;

    public get selectedItem(): IInventoryItem | undefined {
        return this._selectedItem;
    }

    public data: IListInventoryItemsResponse = {
        items: [],
        totalCount: 0
    };
    public currentIndex = 0;
    public search = '';
    public search$ = new Subject<string>();

    @Input()
    public set stockpileId(value: string) {
        this._stockpileId = value;
    }

    @Input()
    public set partnerId(value: string) {
        this._partnerId = value;
    }

    constructor(
        private stockpilesService: StockpilesService,
    ) {
    }

    ngAfterViewInit() {
        if (!this._stockpileId) {
            return;
        }

        this.observer = new IntersectionObserver(this.intersectionObserverCallback);
        this.delimiter = document.getElementById('grid-delimiter') as HTMLElement;

        this.stockpilesService.listInventoryItems(this._stockpileId, this._req)
            .subscribe(data => {
                this.observer?.unobserve(this.delimiter!);
                this.data = data;

                this.rowHeight = document.querySelector('tbody tr')?.clientHeight ?? this.rowHeight;

                if (this.data.totalCount > this.data.items.length) {
                    this.observer!.observe(this.delimiter!);
                }
            });


        this.search$
            .pipe(
                debounceTime(300),
                distinctUntilChanged(),
                switchMap((searchPhrase: string) => {
                        this._req.page = 1;
                        this._req.search = searchPhrase;
                        return this.stockpilesService.listInventoryItems(this._stockpileId!, this._req);
                    }
                )
            )
            .subscribe(data => {
                this.observer?.unobserve(this.delimiter!);
                this.data = data;

                if (this.data.totalCount > this.data.items.length) {
                    this.observer!.observe(this.delimiter!);
                }
            });

        this.kbdEvent$
            .subscribe((event: KeyboardEvent) => {
                if (this.uiState == UiState.Select) {
                    return;
                } else if (this.uiState == UiState.List) {

                    if (event.key == 'Escape') {
                        event.preventDefault();
                        this.search = '';
                        this.searchInput?.nativeElement.blur();
                        this.search$.next(this.search);
                    }
                    if (event.key == 'Enter') {
                        event.preventDefault();
                        this._selectedItem = this.data.items[this.currentIndex];
                        this.uiState = UiState.Select;
                    } else if (event.ctrlKey && event.key == '/') {
                        event.preventDefault();
                        this.searchInput?.nativeElement.focus();
                    } else if (event.key == 'ArrowDown') {
                        event.preventDefault();
                        if (this.currentIndex < this.data.items.length - 1) {
                            this.currentIndex++;
                        }

                        const tableBottom = this.table?.nativeElement.getBoundingClientRect().bottom;
                        const cursorBottom = document.querySelector('.current-idx')?.getBoundingClientRect().bottom;

                        if (tableBottom! - cursorBottom! <= 5) {
                            this.table?.nativeElement.scrollBy(0, this.rowHeight);
                        }

                    } else if (event.key == 'ArrowUp') {
                        event.preventDefault();
                        if (this.currentIndex > 0) {
                            this.currentIndex--;
                        }

                        const tableTop = this.table?.nativeElement.getBoundingClientRect().top;
                        const cursorTop = document.querySelector('.current-idx')?.getBoundingClientRect().top;

                        if (cursorTop! - tableTop! <= 5 + this.headerHeight) {
                            this.table?.nativeElement.scrollBy(0, -this.rowHeight);
                        }
                    }
                }
            })
    }


    loadMore() {
        this.observer!.unobserve(this.delimiter!);

        console.log(this.data.totalCount, this.data.items.length)
        if (this.data.totalCount == this.data.items.length) {
            return;
        }

        this._req.page += 1;
        this.stockpilesService.listInventoryItems(this._stockpileId!, this._req).subscribe(data => {
            this.data.items = [...this.data.items!, ...(data.items)];
            this.observer!.observe(this.delimiter!);
        });
    }

    handleInputDone() {

        this.uiState = UiState.List;
    }
}
