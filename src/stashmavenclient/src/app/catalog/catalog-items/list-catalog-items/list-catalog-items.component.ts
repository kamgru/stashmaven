import {Component, ElementRef, EventEmitter, HostListener, Output, ViewChild} from '@angular/core';
import {CommonModule} from '@angular/common';
import {
    ListCatalogItemsRequest,
    ListCatalogItemsResponse,
    ListCatalogItemsService
} from "./list-catalog-items.service";
import {debounceTime, distinctUntilChanged, Subject, switchMap} from "rxjs";
import {FormsModule} from "@angular/forms";

@Component({
    selector: 'app-list-catalog-items',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './list-catalog-items.component.html',
    styleUrls: ['./list-catalog-items.component.css']
})
export class ListCatalogItemsComponent {

    data: ListCatalogItemsResponse = {
        items: [],
        totalCount: 0
    };

    req: ListCatalogItemsRequest = new ListCatalogItemsRequest(1, 25, null, true, null);
    delimiter?: HTMLElement;
    observer?: IntersectionObserver;
    currentIndex = 0;

    search = '';
    search$ = new Subject<string>();
    kbdEvent$ = new Subject<KeyboardEvent>();

    rowHeight = 33;
    headerHeight = 40;

    @ViewChild('searchInput') searchInput?: ElementRef;
    @ViewChild('table') table?: ElementRef;

    @HostListener('window:keydown', ['$event'])
    handleKeyDown(event: KeyboardEvent) {
        this.kbdEvent$.next(event);
    }

    intersectionObserverCallback: IntersectionObserverCallback
        = (entries: IntersectionObserverEntry[]) => {
        for (let entry of entries) {
            if (entry.target === this.delimiter && entry.isIntersecting) {
                this.loadMore();
            }
        }
    };
    @Output() onAdd = new EventEmitter<void>();

    constructor(
        private listCatalogItemsService: ListCatalogItemsService
    ) {
    }

    ngOnInit() {
        this.observer = new IntersectionObserver(this.intersectionObserverCallback);
        this.delimiter = document.getElementById('grid-delimiter') as HTMLElement;

        this.listCatalogItemsService.list(this.req)
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
                        this.req.page = 1;
                        this.req.search = searchPhrase;
                        return this.listCatalogItemsService.list(this.req);
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
                if (event.key == 'Escape') {
                    event.preventDefault();
                    this.search = '';
                    this.searchInput?.nativeElement.blur();
                    this.search$.next(this.search);
                }
                if (event.key == 'Enter') {
                    event.preventDefault();
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
            })
    }

    loadMore() {
        this.observer!.unobserve(this.delimiter!);

        if (this.data.totalCount == this.data.items.length) {
            return;
        }

        this.req.page += 1;
        this.listCatalogItemsService.list(this.req).subscribe(data => {
            this.data.items = [...this.data.items!, ...(data.items)];
            this.observer!.observe(this.delimiter!);
        });
    }

    onMouseover($event: MouseEvent) {
        console.log($event);

    }
}
