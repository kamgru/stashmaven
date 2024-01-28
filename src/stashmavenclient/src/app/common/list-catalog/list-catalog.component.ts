import {Component, ElementRef, EventEmitter, HostListener, Output, ViewChild} from '@angular/core';
import {CommonModule} from '@angular/common';
import {Subject, takeUntil} from "rxjs";
import {ICatalogItem, ListCatalogService} from "./list-catalog.service";

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
    public OnCatalogItemSelected: EventEmitter<ICatalogItem> = new EventEmitter<ICatalogItem>();

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

    public totalPages_$ = this.listCatalog.totalPages_$;
    public currentIndex_$ = this.listCatalog.currentIndex_$;
    public pageSize_$ = this.listCatalog.pageSize_$;
    public page_$ = this.listCatalog.page_$;
    public search_$ = this.listCatalog.search_$;
    public catalogItems$ = this.listCatalog.catalogItems$;

    constructor(
        private listCatalog: ListCatalogService,
    ) {
        this.listCatalog.selectedCatalogItem$
            .pipe(
                takeUntil(this._destroy$))
            .subscribe(x => {
                this.OnCatalogItemSelected.emit(x);
            });
    }

    public changePageSize = (value: number) => this.listCatalog.changePageSize(value);
    public tryNextPage = () => this.listCatalog.tryNextPage();
    public tryPrevPage = () => this.listCatalog.tryPrevPage();
    public sortBy = (value: string) => this.listCatalog.sortBy(value);
    public search = (value: string) => this.listCatalog.search(value);

    handleRowClick(index: number, catalogItem: ICatalogItem) {
        this.currentIndex_$.set(index);
        this.listCatalog.selectedCatalogItem$.next(catalogItem);
    }

    ngOnDestroy() {
        this._destroy$.next();
        this._destroy$.complete();
    }
}
