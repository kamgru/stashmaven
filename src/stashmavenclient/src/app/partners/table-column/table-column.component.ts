import {Component, EventEmitter, Input, Output} from '@angular/core';
import {CommonModule} from '@angular/common';

export class ColumnClicked {
    constructor(
        public id: string,
        public sortOrder: string | null) {
    }
}

export class ColumnModel {
    constructor(
        public id: string,
        public name: string,
        public interactive: boolean = false,
        public sortOrder: string | null = null) {
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

    @Input() column: ColumnModel = new ColumnModel('table-column', 'table-column', false);
    @Output() sortingChanged: EventEmitter<ColumnClicked> = new EventEmitter<ColumnClicked>();

    handleClick() {
        if (!this.column.interactive){
            return;
        }

        if (this.column.sortOrder == null) {
            this.column.sortOrder = 'asc';
        } else if (this.column.sortOrder == 'asc') {
            this.column.sortOrder = 'desc';
        } else {
            this.column.sortOrder = null;
        }

        const evt = new ColumnClicked(this.column.id, this.column.sortOrder);
        this.sortingChanged.emit(evt);
    }
}
