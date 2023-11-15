import {Component} from '@angular/core';
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

    private observedElement?: HTMLElement;
    private request: ListPartnersRequest = new ListPartnersRequest();

    private totalCount: number = 0;
    private searchPhrase$ = new Subject<string>();
    private firstLoadDone: boolean = false;

    private intersectionObserverCallback: IntersectionObserverCallback
        = (entries: IntersectionObserverEntry[]) => {
        const entry = entries[0];

        if (entry.isIntersecting) {
            if (this.partners.length < this.totalCount || !this.firstLoadDone) {
                this.request.page++;
                this.loadMore();
            }
        }
    };

    constructor(private partnersService: PartnersService) {
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
}
