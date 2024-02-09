import {Component, ElementRef, EventEmitter, HostListener, Output, ViewChild} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {IPartner, ListPartnersService} from "./list-partners.service";
import {AsyncPipe} from "@angular/common";

@Component({
    selector: 'app-list-partners',
    standalone: true,
    imports: [
        AsyncPipe
    ],
    templateUrl: './list-partners.component.html',
    styleUrl: './list-partners.component.css'
})
export class ListPartnersComponent {

    private _destroy$ = new Subject<void>();

    @ViewChild('searchInput')
    private _searchInput?: ElementRef<HTMLInputElement>;

    @Output()
    public OnPartnerSelected: EventEmitter<IPartner> = new EventEmitter<IPartner>();

    @HostListener('window:keydown', ['$event'])
    keyEvent(event: KeyboardEvent) {
        if (this.listPartner.tryHandleKey(event)) {
            return;
        }

        if (event.ctrlKey && event.key == '/') {
            this._searchInput?.nativeElement.focus();
            event.preventDefault();
        } else if (event.key == 'Escape') {
            this._searchInput?.nativeElement.blur();
        }
    }

    public totalPages_$ = this.listPartner.totalPages_$;
    public currentIndex_$ = this.listPartner.currentIndex_$;
    public pageSize_$ = this.listPartner.pageSize_$;
    public page_$ = this.listPartner.page_$;
    public search_$ = this.listPartner.search_$;
    public partners$ = this.listPartner.partners$;

    constructor(
        private listPartner: ListPartnersService,
    ) {
        this.listPartner.selectedPartner$
            .pipe(
                takeUntil(this._destroy$))
            .subscribe(x => {
                this.OnPartnerSelected.emit(x);
            });
    }

    public changePageSize = (value: number) => this.listPartner.changePageSize(value);
    public tryNextPage = () => this.listPartner.tryNextPage();
    public tryPrevPage = () => this.listPartner.tryPrevPage();
    public sortBy = (value: string) => this.listPartner.sortBy(value);
    public search = (value: string) => this.listPartner.search(value);

    handleRowClick(index: number, partner: IPartner) {
        this.currentIndex_$.set(index);
        this.listPartner.selectedPartner$.next(partner);
    }

    ngOnDestroy() {
        this._destroy$.next();
        this._destroy$.complete();
    }
}
