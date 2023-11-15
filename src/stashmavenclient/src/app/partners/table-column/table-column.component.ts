import {Component, EventEmitter, Input, Output} from '@angular/core';
import {CommonModule} from '@angular/common';

export class SortOrderChanged {
    constructor(
        public id: string,
        public sortOrder: string | null) {
    }
}

@Component({
    selector: 'app-table-column',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './table-column.component.html',
    styleUrls: ['./table-column.component.css']
})
export class TableColumnComponent {

    @Input() name: string = 'table-column';
    @Input() id: string = 'table-column';
    @Output() sortingChanged: EventEmitter<SortOrderChanged> = new EventEmitter<SortOrderChanged>();

    sortOrder: string | null = null;

    handleClick() {
        if (this.sortOrder == null) {
            this.sortOrder = 'asc';
        } else if (this.sortOrder == 'asc') {
            this.sortOrder = 'desc';
        } else {
            this.sortOrder = null;
        }

        const evt = new SortOrderChanged(this.id, this.sortOrder);
        this.sortingChanged.emit(evt);
    }
}
