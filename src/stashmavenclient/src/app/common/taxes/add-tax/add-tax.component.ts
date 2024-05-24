import {Component, ElementRef, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';
import {FormBuilder, FormControl, ReactiveFormsModule, Validators} from "@angular/forms";

export class TaxDefinitionAddedEvent {
    constructor(
        public readonly name: string,
        public readonly rate: number
    ) {
    }
}

@Component({
    selector: 'app-add-tax',
    standalone: true,
    imports: [
        ReactiveFormsModule
    ],
    templateUrl: './add-tax.component.html'
})
export class AddTaxComponent implements OnInit {

    public addTaxForm = this.fb.group({
        name: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        rate: [0, [Validators.required, Validators.min(0), Validators.max(100)]]
    });

    @ViewChild('nameInput', {static: true})
    public nameInput!: ElementRef<HTMLInputElement>;

    @Output()
    public OnTaxDefinitionAdded = new EventEmitter<TaxDefinitionAddedEvent>();

    @Output()
    public OnCancelled = new EventEmitter<void>();

    public get name(): FormControl<string> {
        return this.addTaxForm.get('name') as FormControl<string>;
    }

    public get rate(): FormControl<number> {
        return this.addTaxForm.get('rate') as FormControl<number>;
    }

    constructor(
        private fb: FormBuilder
    ) {
    }

    public ngOnInit() {
        this.nameInput.nativeElement.focus();
    }

    public handleSubmit(){
        if (this.addTaxForm.valid) {
            this.OnTaxDefinitionAdded.emit(new TaxDefinitionAddedEvent(
                this.name.value,
                this.rate.value
            ));
        }
    }
}
