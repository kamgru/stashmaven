import {Component, ElementRef, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormBuilder, FormControl, ReactiveFormsModule, Validators} from "@angular/forms";

export class BrandAddedEvent {
    constructor(
        public readonly name: string,
        public readonly shortCode: string
    ) {
    }
}

@Component({
    selector: 'app-add-brand',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './add-brand.component.html',
})
export class AddBrandComponent implements OnInit {

    public addBrandForm = this.fb.group({
        name: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        shortCode: ['', [Validators.required, Validators.maxLength(10), Validators.minLength(2)]],
    });

    public get name(): FormControl<string> {
        return this.addBrandForm.get('name') as FormControl<string>;
    }

    public get shortCode(): FormControl<string> {
        return this.addBrandForm.get('shortCode') as FormControl<string>;
    }

    @Output()
    public OnBrandAdded = new EventEmitter<BrandAddedEvent>();

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
        if (!this.addBrandForm.valid) {
            return;
        }
        this.OnBrandAdded.emit(new BrandAddedEvent(this.name.value, this.shortCode.value));
    }
}
