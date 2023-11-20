import {Component, HostListener} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ListPartnersRequest, ListPartnersResponse, Partner, PartnersService} from "../partners.service";
import {debounceTime, distinctUntilChanged, Subject, switchMap} from "rxjs";
import {SortOrderChanged, TableColumnComponent} from "../table-column/table-column.component";
import {FormsModule} from "@angular/forms";

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

    private observedElement?: HTMLElement;
    private request: ListPartnersRequest = new ListPartnersRequest();

    private totalCount: number = 0;
    private searchPhrase$ = new Subject<string>();
    private firstLoadDone: boolean = false;

    constructor(private partnersService: PartnersService) {
    }

    @HostListener('window:keydown', ['$event'])
    handleKeyDown(event: KeyboardEvent) {
        if (event.key === 'ArrowDown') {
            event.preventDefault();
            if (this.currentIndex < this.partners.length - 1) {
                this.currentIndex++;

                let tableItems = document.getElementById('table-items');
                let cursorPointer = document.getElementById('cursor-pointer');
                let cursorPointerBottom = cursorPointer?.getBoundingClientRect().bottom;
                let tableItemsBottom = tableItems?.getBoundingClientRect().bottom;

                if (cursorPointerBottom! >= tableItemsBottom! - 27) {
                    tableItems?.scrollBy(0, 27);
                }
            }
        } else if (event.key === 'ArrowUp') {
            event.preventDefault();
            if (this.currentIndex > 0) {
                this.currentIndex--;

                let tableItems = document.getElementById('table-items');
                let cursorPointer = document.getElementById('cursor-pointer');
                let cursorPointerTop = cursorPointer?.getBoundingClientRect().top;
                let tableItemsTop = tableItems?.getBoundingClientRect().top;

                if (cursorPointerTop! <= tableItemsTop! + 27) {
                    tableItems?.scrollBy(0, -27);
                }

            }
        }
    }

    ngOnInit() {
        this.searchPhrase$
            .pipe(
                debounceTime(500),
                distinctUntilChanged(),
                switchMap((searchPhrase: string) => {
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
    }

    loadMore() {
        this.partnersService.listPartners(this.request)
            .subscribe(
                (response: ListPartnersResponse) => {
                    this.observer?.unobserve(this.observedElement!);
                    this.partners = [...this.partners, ...response.partners]
                    this.totalCount = response.totalCount;
                    this.firstLoadDone = true;

                    if (this.partners.length < this.totalCount)
                    {
                        this.observer?.observe(this.observedElement!);
                    }
                },
            );
    }

    search($event: KeyboardEvent) {
        const searchPhrase = ($event.target as HTMLInputElement).value;
        this.searchPhrase$.next(searchPhrase);
    }

    sortBy($event: SortOrderChanged) {
        this.searchPhrase = '';
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

        for(let entry of entries){

            if (entry.target === this.observedElement && entry.isIntersecting) {
                if (this.partners.length < this.totalCount || !this.firstLoadDone) {
                    this.request.page++;
                    this.loadMore();
                }
            }
            else {
                console.log(entry);
            }
        }
    };
}
