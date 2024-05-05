import {Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";

export class ShipmentKindAddedEvent {
    constructor(
        public readonly name: string,
        public readonly shortCode: string,
        public readonly direction: string
    ) {
    }
}

@Component({
    selector: 'app-add-shipment-kind',
    standalone: true,
    imports: [
        FormsModule,
        ReactiveFormsModule
    ],
    templateUrl: './add-shipment-kind.component.html'
})
export class AddShipmentKindComponent implements OnInit {

    @Input({required: true})
    public directions: string[] = [];

    @Output()
    public OnShipmentKindAdded = new EventEmitter<ShipmentKindAddedEvent>();

    @Output()
    public OnCancelled = new EventEmitter<void>();

    @ViewChild('nameInput', {static: true})
    public nameInput!: ElementRef<HTMLInputElement>;

    public addShipmentKindForm = this.fb.group({
        name: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(100)]],
        shortCode: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(10)]],
        direction: ['', [Validators.required]]
    });

    public get name(): FormControl<string> {
        return this.addShipmentKindForm.get('name') as FormControl<string>;
    }

    public get shortCode(): FormControl<string> {
        return this.addShipmentKindForm.get('shortCode') as FormControl<string>;
    }

    constructor(
        private fb: FormBuilder
    ) {
    }

    public ngOnInit() {
        this.nameInput.nativeElement.focus();
    }

    handleSubmit() {
        if (this.addShipmentKindForm.valid) {
            this.OnShipmentKindAdded.emit(new ShipmentKindAddedEvent(
                this.name.value,
                this.shortCode.value,
                this.addShipmentKindForm.get('direction')!.value as 'In' | 'Out'
            ));
        }
    }
}
