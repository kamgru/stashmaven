import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {IStockpile} from "../../../common/services/stockpile.service";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";

@Component({
    selector: 'app-edit-stockpile',
    standalone: true,
    imports: [
        ReactiveFormsModule
    ],
    templateUrl: './edit-stockpile.component.html',
    styleUrl: './edit-stockpile.component.css'
})
export class EditStockpileComponent implements OnInit {

    @Input()
    public stockpile: IStockpile | null = null;

    @Output()
    public OnEditCompleted: EventEmitter<IStockpile> = new EventEmitter<IStockpile>();

    @Output()
    public OnEditCancelled: EventEmitter<void> = new EventEmitter<void>();

    public editForm = new FormGroup({
        name: new FormControl<string>('', {
            validators: [Validators.required, Validators.minLength(3), Validators.maxLength(256)],
            nonNullable: true
        }),
        shortCode: new FormControl<string>('', {
            validators: [Validators.required, Validators.minLength(2), Validators.maxLength(2)],
            nonNullable: true
        }),
        isDefault: new FormControl<boolean>(false)
    });

    public get name(): FormControl<string> {
        return this.editForm.get('name') as FormControl<string>;
    }

    public get shortCode(): FormControl<string> {
        return this.editForm.get('shortCode') as FormControl<string>;
    }

    public get isDefault(): FormControl<boolean> {
        return this.editForm.get('isDefault') as FormControl<boolean>;
    }

    constructor() {
    }

    public ngOnInit() {
        if (this.stockpile) {
            this.name.setValue(this.stockpile.name);
            this.shortCode.setValue(this.stockpile.shortCode);
            this.isDefault.setValue(this.stockpile.isDefault);
        }
    }

    public handleSubmit() {
        if (this.editForm.valid) {
            this.OnEditCompleted.emit({
                stockpileId: this.stockpile ? this.stockpile.stockpileId : '',
                name: this.name.value,
                shortCode: this.shortCode.value,
                isDefault: this.isDefault.value
            });
        }
    }

    handleCancel() {
        this.OnEditCancelled.emit();
    }
}
