import {Component, ElementRef, EventEmitter, Output, ViewChild} from '@angular/core';
import {FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";

export class StockpileAddedEvent {
    constructor(
        public readonly name: string,
        public readonly shortCode: string,
        public readonly isDefault: boolean
    ) {
    }
}

@Component({
    selector: 'app-add-stockpile',
    standalone: true,
    imports: [
        FormsModule,
        ReactiveFormsModule
    ],
    templateUrl: './add-stockpile.component.html'
})
export class AddStockpileComponent {

    public addStockpileForm = this.fb.group({
        name: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        shortCode: ['', [Validators.required, Validators.maxLength(10), Validators.minLength(2)]],
        isDefault: [false]
    });

    public get name(): FormControl<string> {
        return this.addStockpileForm.get('name') as FormControl<string>;
    }

    public get shortCode(): FormControl<string> {
        return this.addStockpileForm.get('shortCode') as FormControl<string>;
    }

    public get isDefault(): FormControl<boolean> {
        return this.addStockpileForm.get('isDefault') as FormControl<boolean>;
    }

    @Output()
    public OnStockpileAdded = new EventEmitter<StockpileAddedEvent>();

    @Output()
    public OnCancelled = new EventEmitter<void>();

    @ViewChild('nameInput', {static: true})
    public nameInput!: ElementRef<HTMLInputElement>;

    constructor(
        private fb: FormBuilder
    ) {
    }

    public ngOnInit() {
        this.nameInput.nativeElement.focus();
    }

    public handleSubmit() {
        if (!this.addStockpileForm.valid) {
            return;
        }
        this.OnStockpileAdded.emit(
            new StockpileAddedEvent(
                this.name.value,
                this.shortCode.value,
                this.isDefault.value));
    }
}
