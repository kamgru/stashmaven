import {Component, Input, TemplateRef} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ISuperGridListService, SuperGridListRequest} from "./super-grid-list-service";

export class SuperGridColumn {
    constructor(
        public name: string,
        public label: string,
        public sortable: boolean,
        public visible: boolean) {
    }
}

@Component({
    selector: 'app-super-grid',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './super-grid.component.html',
    styleUrls: ['./super-grid.component.css']
})
export class SuperGridComponent<TData> {
    @Input() columns: SuperGridColumn[] = [];
    @Input() listService?: ISuperGridListService<TData>;
    @Input() body!: TemplateRef<any>;

    data: TData[] = [];
    totalCount: number = 0;

    columnsRepeat?: string;
    req: SuperGridListRequest = new SuperGridListRequest(1, 25);

    ngOnInit() {
        this.columnsRepeat = `repeat(${this.columns.length}, 1fr)`;

        if (this.listService) {
            this.listService.list(this.req)
                .subscribe((response) => {
                    this.data = response.items;
                    this.totalCount = response.totalCount;
                });
        }
    }
}
