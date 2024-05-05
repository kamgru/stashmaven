import {Component, ElementRef, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';
import {FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";

export class BusinessIdentifierAddedEvent {
    constructor(
        public readonly name: string,
        public readonly shortCode: string,
    ) {
    }
}

@Component({
    selector: 'app-add-business-identifier',
    standalone: true,
    imports: [
        FormsModule,
        ReactiveFormsModule
    ],
    templateUrl: './add-business-identifier.component.html',
})
export class AddBusinessIdentifierComponent implements OnInit {

    public addBusinessIdentifierForm = this.fb.group({
        name: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        shortCode: ['', [Validators.required, Validators.maxLength(8), Validators.minLength(2)]],
    });

    public get name(): FormControl<string> {
        return this.addBusinessIdentifierForm.get('name') as FormControl<string>;
    }

    public get shortCode(): FormControl<string> {
        return this.addBusinessIdentifierForm.get('shortCode') as FormControl<string>;
    }

    @Output()
    public OnBusinessIdentifierAdded = new EventEmitter<BusinessIdentifierAddedEvent>();

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
        if (this.addBusinessIdentifierForm.valid) {
            this.OnBusinessIdentifierAdded.emit(new BusinessIdentifierAddedEvent(
                this.name.value,
                this.shortCode.value
            ));
        }
    }
}
