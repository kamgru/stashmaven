import {Component, EventEmitter, Input, Output} from '@angular/core';
import {AddAvailableCountryRequest, CountryService, IAvailableCountry} from "../../services/country.service";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";

@Component({
    selector: 'app-edit-country',
    standalone: true,
    imports: [
        ReactiveFormsModule
    ],
    templateUrl: './edit-country.component.html',
    styleUrl: './edit-country.component.css'
})
export class EditCountryComponent {

    @Input()
    public country: IAvailableCountry | null = null;

    @Output()
    public OnCountryAdded: EventEmitter<IAvailableCountry> = new EventEmitter<IAvailableCountry>();

    @Output()
    public OnCountryAddCanceled: EventEmitter<void> = new EventEmitter<void>();

    public countryForm = new FormGroup({
        name: new FormControl<string>('', {
            validators: [Validators.required, Validators.minLength(3), Validators.maxLength(256)],
            nonNullable: true
        }),
        code: new FormControl<string>('', {
            validators: [Validators.required, Validators.minLength(2), Validators.maxLength(2)],
            nonNullable: true
        })

    })

    public get name(): FormControl<string> {
        return this.countryForm.get('name') as FormControl<string>;
    }

    public get code(): FormControl<string> {
        return this.countryForm.get('code') as FormControl<string>;
    }

    constructor(
        private countryService: CountryService
    ) {
    }

    public onSubmit() {
        if (!this.countryForm.valid) {
            return;
        }

        const name = this.name.value;
        const code = this.code.value;

        const req = new AddAvailableCountryRequest(name, code);
        this.countryService.addAvailableCountry(req)
            .subscribe(() => {
                this.OnCountryAdded.emit({name, code});
            });
    }

    onCancel() {
        this.OnCountryAddCanceled.emit();
    }
}
