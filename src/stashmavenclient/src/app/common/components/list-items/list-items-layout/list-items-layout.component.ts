import {Component, EventEmitter, Input, Output, TemplateRef, ViewChild} from '@angular/core';
import {IListResponse} from "../ListServiceBase";
import {ListPageSizeSelectComponent} from "../list-page-size-select/list-page-size-select.component";
import {ListPagingInfoComponent} from "../list-paging-info/list-paging-info.component";
import {ListSearchInputComponent} from "../list-search-input/list-search-input.component";
import {NgTemplateOutlet} from "@angular/common";

@Component({
    selector: 'app-list-items-layout',
    standalone: true,
    imports: [
        ListPageSizeSelectComponent,
        ListPagingInfoComponent,
        ListSearchInputComponent,
        NgTemplateOutlet
    ],
    templateUrl: './list-items-layout.component.html',
})
export class ListItemsLayoutComponent {

    @ViewChild(ListSearchInputComponent)
    public searchInput!: ListSearchInputComponent;

    @Input({required: true})
    public listResponse!: IListResponse<any>;

    @Input({required: true})
    public itemTemplate: TemplateRef<any> | null = null;

    @Input()
    public toolbarTemplate: TemplateRef<any> | null = null;

    @Output()
    public OnSearch = new EventEmitter<string>();

    @Output()
    public OnPageSizeChanged = new EventEmitter<number>();

    @Output()
    public OnPrevPage = new EventEmitter<void>();

    @Output()
    public OnNextPage = new EventEmitter<void>();
}
