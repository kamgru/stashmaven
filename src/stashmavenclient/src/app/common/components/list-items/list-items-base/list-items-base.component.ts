import {
    Component, EventEmitter,
    HostListener, Input,
    OnDestroy,
    Output, Signal, TemplateRef,
    ViewChild
} from "@angular/core";
import {IListRequest, IListResponse, ListServiceBase} from "../ListServiceBase";
import {Observable, Subject, takeUntil} from "rxjs";
import {ListSearchInputComponent} from "../list-search-input/list-search-input.component";
import {AsyncPipe, NgTemplateOutlet} from "@angular/common";
import {ListPageSizeSelectComponent} from "../list-page-size-select/list-page-size-select.component";
import {ListPagingInfoComponent} from "../list-paging-info/list-paging-info.component";
import {ListItemsLayoutComponent} from "../list-items-layout/list-items-layout.component";

@Component({
    selector: 'app-list-items-base',
    template: '',
    imports: [
        NgTemplateOutlet,
        AsyncPipe,
        ListPageSizeSelectComponent,
        ListPagingInfoComponent,
        ListSearchInputComponent
    ],
    standalone: true
})
export class ListItemsBaseComponent<TItem, TListRequest extends IListRequest,
    TListResponse extends IListResponse<any>, TService extends ListServiceBase<TItem, TListRequest, TListResponse>>
    implements OnDestroy {

    protected _destroy$ = new Subject<void>();
    protected listService!: TService;

    @ViewChild(ListItemsLayoutComponent)
    private _layout?: ListItemsLayoutComponent;

    @Input()
    public itemsTemplate: TemplateRef<any> | null = null;

    @Output()
    public OnItemSelected = new EventEmitter<TItem>();

    @Output()
    public OnItemConfirmed = new EventEmitter<TItem>();

    public currentIndex_$!: Signal<number>;
    public listResponse$!: Observable<TListResponse>;

    @HostListener('window:keydown', ['$event'])
    protected keyEvent(event: KeyboardEvent) {
        if (this.listService.tryHandleKey(event)) {
            return;
        }

        if (event.ctrlKey && event.key == '/') {
            this._layout?.searchInput?.focus();
            event.preventDefault();
        } else if (event.key == 'Escape') {
            this._layout?.searchInput?.blur();
        }
    }

    protected bootstrap(service: TService) {
        this.listService = service;

        this.currentIndex_$ = this.listService.currentIndex_$;
        this.listResponse$ = this.listService.items$;

        this.listService.selectedItem$
            .pipe(
                takeUntil(this._destroy$))
            .subscribe(item => {
                if (item) {
                    this.OnItemSelected.emit(item);
                }
            });

        this.listService.confirmedItem$
            .pipe(
                takeUntil(this._destroy$))
            .subscribe(item => {
                this.OnItemConfirmed.emit(item);
            });
    }

    public changePageSize = (value: number) => this.listService.changePageSize(value);
    public tryNextPage = () => this.listService.tryNextPage();
    public tryPrevPage = () => this.listService.tryPrevPage();
    public sortBy = (value: string) => this.listService.sortBy(value);
    public search = (value: string) => this.listService.search(value);
    public handleRowClick = (item: TItem) => this.listService.select(item);
    public handleRowDblClick = (item: TItem) => this.listService.confirm(item);
    public reload = () => this.listService.reload();

    ngOnDestroy() {
        this._destroy$.next();
        this._destroy$.complete();
    }
}