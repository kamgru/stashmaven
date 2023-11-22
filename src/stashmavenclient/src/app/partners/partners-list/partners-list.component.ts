import {Component, ElementRef, EventEmitter, HostListener, Output, ViewChild} from '@angular/core';
import {FormsModule} from "@angular/forms";
import {CommonModule} from '@angular/common';
import {ListPartnersRequest, ListPartnersResponse, Partner, PartnersService} from "../partners.service";
import {debounceTime, distinctUntilChanged, Subject, switchMap} from "rxjs";
import {ColumnClicked, ColumnModel, TableColumnComponent} from "../table-column/table-column.component";

export class SelectedPartnerChanged {
    constructor(
        public partnerId: string) {
    }
}

@Component({
    selector: 'app-partners-list',
    standalone: true,
    imports: [CommonModule, TableColumnComponent, FormsModule],
    templateUrl: './partners-list.component.html',
    styleUrls: ['./partners-list.component.css']
})
export class PartnersListComponent {

    partners: Partner[] = [];
    observer?: IntersectionObserver;
    searchPhrase: string = '';
    currentIndex: number = 0;
    @ViewChild('searchPhraseInput') searchPhraseInput?:  ElementRef;

    columns: ColumnModel[] = [
        new ColumnModel('no', 'No.'),
        new ColumnModel('customIdentifier', 'Id', true, null),
        new ColumnModel('legalName', 'Name', true, null),
        new ColumnModel('street', 'Street'),
        new ColumnModel('city', 'City'),
        new ColumnModel('zipCode', 'Zip code'),
    ];

    private observedElement?: HTMLElement;
    private request: ListPartnersRequest = new ListPartnersRequest();
    private totalCount: number = 0;
    private searchPhrase$ = new Subject<string>();
    private currentEvent$ = new Subject<KeyboardEvent>();
    private firstLoadDone: boolean = false;

    constructor(private partnersService: PartnersService) {
    }

    @HostListener('window:keydown', ['$event'])
    handleKeyDown(event: KeyboardEvent) {
        this.currentEvent$.next(event);
    }

    ngOnInit() {
        this.currentEvent$
            .subscribe((event: KeyboardEvent) => {
                if (event.key == 'Enter') {
                    event.preventDefault();
                    const partner = this.partners[this.currentIndex];
                    this.partnersService.selectPartner(partner);
                } else if (event.ctrlKey && event.key == '/'){
                    event.preventDefault();
                    this.searchPhraseInput?.nativeElement.focus();
                } else if (event.key == 'ArrowDown') {
                    event.preventDefault();
                    if (this.currentIndex < this.partners.length - 1) {
                        this.currentIndex++;
                    }
                    let table = document.querySelector('table');
                    let tableBottom = document.querySelector('table')?.getBoundingClientRect().bottom;
                    let cursorBottom = document.querySelector('.cursor-pointer')?.getBoundingClientRect().bottom

                    if (tableBottom! - cursorBottom! <= 5) {
                        table?.scrollBy(0, 27);
                    }
                } else if (event.key == 'ArrowUp') {
                    event.preventDefault();
                    if (this.currentIndex > 0) {
                        this.currentIndex--;
                    }

                    let table = document.querySelector('table');
                    let tableTop = document.querySelector('table')?.getBoundingClientRect().top;
                    let cursorTop = document.querySelector('.cursor-pointer')?.getBoundingClientRect().top

                    if (cursorTop! - tableTop! <= 5 + 27) {
                        table?.scrollBy(0, -27);
                    }
                }
            })
        this.searchPhrase$
            .pipe(
                debounceTime(500),
                distinctUntilChanged(),
                switchMap((searchPhrase: string) => {
                    this.currentIndex = 0;
                    this.firstLoadDone = false;
                    this.request.reset();
                    this.request.search = searchPhrase;
                    this.searchPhrase = searchPhrase;
                    return this.partnersService.listPartners(this.request);
                }))
            .subscribe(
                (response: ListPartnersResponse) => {
                    this.observer?.unobserve(this.observedElement!);
                    this.partners = response.partners;
                    this.totalCount = response.totalCount;
                    this.firstLoadDone = true;
                    this.observer?.observe(this.observedElement!);
                },
            );

        this.partnersService.listPartners(this.request)
            .subscribe(
                (response: ListPartnersResponse) => {
                    this.partners = response.partners;
                    this.totalCount = response.totalCount;
                    this.firstLoadDone = true;

                    this.observedElement = document.querySelector('tfoot') as HTMLElement;
                    this.observer = new IntersectionObserver(this.intersectionObserverCallback, {threshold: 1});
                    this.observer.observe(this.observedElement);
                },
            )
    }

    ngOnDestroy() {
        this.observer?.disconnect();
        this.searchPhrase$.unsubscribe();
        this.currentEvent$.unsubscribe();
    }

    loadMore() {
        this.partnersService.listPartners(this.request)
            .subscribe(
                (response: ListPartnersResponse) => {
                    this.observer?.unobserve(this.observedElement!);
                    this.partners = [...this.partners, ...response.partners]
                    this.totalCount = response.totalCount;
                    this.firstLoadDone = true;

                    if (this.partners.length < this.totalCount) {
                        this.observer?.observe(this.observedElement!);
                    }
                },
            );
    }

    search($event: KeyboardEvent) {
        const searchPhrase = ($event.target as HTMLInputElement).value;
        this.searchPhrase$.next(searchPhrase);
    }

    sortBy($event: ColumnClicked) {
        for (let column of this.columns) {
            if (column.id != $event.id) {
                column.sortOrder = null;
            }
        }
        this.request.reset();

        if ($event.sortOrder != null) {
            this.request.sortBy = $event.id;
            this.request.isAscending = $event.sortOrder === 'asc';
        }

        this.partners.splice(0, this.partners.length);
        this.firstLoadDone = false;
        this.loadMore();
    }

    private intersectionObserverCallback: IntersectionObserverCallback
        = (entries: IntersectionObserverEntry[]) => {
        for (let entry of entries) {
            if (entry.target === this.observedElement && entry.isIntersecting) {
                if (this.partners.length < this.totalCount || !this.firstLoadDone) {
                    this.request.page++;
                    this.loadMore();
                }
            }
        }
    };
}
