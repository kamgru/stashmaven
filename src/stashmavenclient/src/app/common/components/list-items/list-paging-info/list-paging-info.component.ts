import {Component, Input} from '@angular/core';
import {IListResponse} from "../ListServiceBase";

@Component({
    selector: 'app-list-paging-info',
    standalone: true,
    imports: [],
    templateUrl: './list-paging-info.component.html',
    styleUrl: './list-paging-info.component.css'
})
export class ListPagingInfoComponent {

    @Input({required: true})
    public items!: IListResponse<any>;
}
