import {Component, EventEmitter, Input, Output} from '@angular/core';

export interface ISelectOption {
    value: string;
    label: string;
}

@Component({
    selector: 'app-select',
    standalone: true,
    imports: [],
    templateUrl: './select.component.html',
    styleUrl: './select.component.css',
})
export class SelectComponent {

    @Input()
    public selectedOption: ISelectOption | null = null;

    @Input()
    public options: ISelectOption[] = [];

    @Output()
    public OnSelect = new EventEmitter<ISelectOption>();

    handleChange($event: Event) {
        const selectedValue = (<HTMLSelectElement>$event.target).value;
        const selectedOption = this.options.find(x => x.value === selectedValue);
        this.OnSelect.emit(selectedOption);
    }
}
