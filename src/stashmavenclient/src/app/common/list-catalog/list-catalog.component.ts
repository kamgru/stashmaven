import {Component, ElementRef, EventEmitter, HostListener, Output, ViewChild} from '@angular/core';
import {CommonModule} from '@angular/common';
import {debounceTime, distinctUntilChanged, Observable, Subject, takeUntil} from "rxjs";
import {ICatalogItem, IListCatalogItemsResponse, ListCatalogService} from "./list-catalog.service";

@Component({
    selector: 'app-list-catalog',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './list-catalog.component.html',
    styleUrls: ['./list-catalog.component.css']
})
export class ListCatalogComponent {

    private _destroy$ = new Subject<void>();

    @ViewChild('searchInput')
    private _searchInput?: ElementRef<HTMLInputElement>;

    @Output()
    public OnItemSelected = new EventEmitter<ICatalogItem>();

    @HostListener('window:keydown', ['$event'])
    keyEvent(event: KeyboardEvent) {
        if (this.listCatalog.tryHandleKey(event)) {
            return;
        }

        if (event.ctrlKey && event.key == '/') {
            this._searchInput?.nativeElement.focus();
            event.preventDefault();
        } else if (event.key == 'Escape') {
            this._searchInput?.nativeElement.blur();
        }
    }

    public currentIndex_$ = this.listCatalog.currentIndex_$;
    public catalogItems$ = this.listCatalog.items$;

    private _search$ = new Subject<string>();

    constructor(
        private listCatalog: ListCatalogService,
    ) {
        this.listCatalog.selectedItem$
            .pipe(
                takeUntil(this._destroy$))
            .subscribe(item => {
                if (item) {
                    this.OnItemSelected.emit(item);
                }
            });

        this._search$
            .pipe(
                distinctUntilChanged(),
                debounceTime(500))
            .subscribe(x => this.listCatalog.search(x))
    }

    public changePageSize = (value: number) => this.listCatalog.changePageSize(value);
    public tryNextPage = () => this.listCatalog.tryNextPage();
    public tryPrevPage = () => this.listCatalog.tryPrevPage();
    public sortBy = (value: string) => this.listCatalog.sortBy(value);
    public search = (value: string) => this._search$.next(value);
    public handleRowClick = (item: ICatalogItem) => this.listCatalog.select(item);

    ngOnDestroy() {
        this._destroy$.next();
        this._destroy$.complete();
    }
}
