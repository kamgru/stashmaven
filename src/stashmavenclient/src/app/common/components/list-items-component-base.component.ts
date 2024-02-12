import {
    Component,
    ElementRef,
    EventEmitter,
    HostListener,
    OnDestroy,
    Output, Signal,
    ViewChild,
    WritableSignal
} from "@angular/core";
import {IListRequest, IListResponse, ListServiceBase} from "../ListServiceBase";
import {debounceTime, distinctUntilChanged, Observable, Subject, takeUntil} from "rxjs";

@Component({
    selector: 'app-list-base',
    template: '',
    standalone: true
})
export class ListItemsComponentBase<TItem, TListRequest extends IListRequest,
    TListResponse extends IListResponse<any>, TService extends ListServiceBase<TItem, TListRequest, TListResponse>>
    implements OnDestroy {

    protected _destroy$ = new Subject<void>();
    protected _search$ = new Subject<string>();
    protected listService!: TService;

    @ViewChild('searchInput')
    private _searchInput?: ElementRef<HTMLInputElement>;

    @Output()
    public OnItemSelected = new EventEmitter<TItem>();

    public currentIndex_$!: Signal<number>;
    public items$!: Observable<TListResponse>;

    @HostListener('window:keydown', ['$event'])
    protected keyEvent(event: KeyboardEvent) {
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

    protected bootstrap(service: TService) {
        this.listService = service;

        this.currentIndex_$ = this.listService.currentIndex_$;
        this.items$ = this.listService.items$;

        this.listService.selectedItem$
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
            .subscribe(x => this.listService.search(x))
    }

    public changePageSize = (value: number) => this.listService.changePageSize(value);
    public tryNextPage = () => this.listService.tryNextPage();
    public tryPrevPage = () => this.listService.tryPrevPage();
    public sortBy = (value: string) => this.listService.sortBy(value);
    public search = (value: string) => this._search$.next(value);
    public handleRowClick = (item: TItem) => this.listService.select(item);

    ngOnDestroy() {
        this._destroy$.next();
        this._destroy$.complete();
    }
}