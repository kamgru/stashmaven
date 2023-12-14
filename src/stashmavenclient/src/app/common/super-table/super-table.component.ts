import {AfterViewInit, Component, Input, OnInit, TemplateRef} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ISuperTableListService, SuperTableListRequest, SuperTableListResponse} from "./super-table-list-service";
import {debounceTime, distinctUntilChanged, Subject, switchMap} from "rxjs";

export class SuperTableConfig<TData> {
    constructor(
        public searchConfig: SearchConfig,
        public listService: ISuperTableListService<TData>,
    ) {
    }
}

export class SearchConfig {
    constructor(
        public showSearch: boolean,
        public searchPlaceholder: string,
    ) {
    }
}

@Component({
    selector: 'app-super-table',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './super-table.component.html',
    styleUrls: ['./super-table.component.css']
})
export class SuperTableComponent<TData> implements AfterViewInit, OnInit {

    data?: TData[];
    @Input() header!: TemplateRef<any>;
    @Input() body!: TemplateRef<any>;
    @Input() config!: SuperTableConfig<TData>;

    observer?: IntersectionObserver;
    observedElement?: HTMLElement;
    req = new SuperTableListRequest(1, 25);
    totalCount = 0;
    searchPhrase$ = new Subject<string>();
    currentIndex = 0;

    ngOnInit() {
        this.searchPhrase$
            .pipe(
                debounceTime(300),
                distinctUntilChanged(),
                switchMap((searchPhrase: string) => {
                        this.req.firstPage();
                        this.req.search = searchPhrase;
                        return this.config.listService!.list(this.req);
                    }
                ))
            .subscribe(data => this.reloadData(data));
    }

    ngAfterViewInit() {
        this.observer = new IntersectionObserver(this.intersectionObserverCallback);
        this.observedElement = document.querySelector('tfoot') as HTMLElement;

        if (this.config.listService) {
            this.config.listService.list(this.req)
                .subscribe(data => this.reloadData(data));
        }
    }

    reloadData(data: SuperTableListResponse<TData>) {
        this.observer!.unobserve(this.observedElement!);
        this.data = data.items;
        this.totalCount = data.totalCount;
        this.observer!.observe(this.observedElement!);

        if (this.totalCount > this.data!.length) {
            this.observer!.observe(this.observedElement!);
        }
    }

    private intersectionObserverCallback: IntersectionObserverCallback
        = (entries: IntersectionObserverEntry[]) => {
        for (let entry of entries) {
            if (entry.target === this.observedElement && entry.isIntersecting) {
                this.loadMore();
            }
        }
    };

    loadMore() {
        this.observer!.unobserve(this.observedElement!);

        if (this.totalCount == this.data!.length) {
            return;
        }

        this.req.nextPage();
        this.config.listService!.list(this.req).subscribe(data => {
            this.data = [...this.data!, ...(data.items)];
            this.observer!.observe(this.observedElement!);
        });
    }
}
