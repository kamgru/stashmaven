import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
    selector: 'app-list-page-size-select',
    standalone: true,
    imports: [],
    templateUrl: './list-page-size-select.component.html',
    styleUrl: './list-page-size-select.component.css'
})
export class ListPageSizeSelectComponent {

    @Input({required: true})
    public pageSize: number = 10;

    @Output()
    public OnPageSizeChanged: EventEmitter<number> = new EventEmitter<number>();

    changePageSize(number: number) {
        this.OnPageSizeChanged.emit(number);
    }
}
