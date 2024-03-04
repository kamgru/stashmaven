import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {
    AddAvailableCountryRequest,
    CountryService,
    IAvailableCountry,
    UpdateAvailableCountryRequest
} from "../../services/country.service";
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
export class EditCountryComponent implements OnInit {

    private _mode: 'add' | 'edit' = 'add';

    @Input()
    public country: IAvailableCountry | null = null;

    @Output()
    public OnEditCompleted: EventEmitter<IAvailableCountry> = new EventEmitter<IAvailableCountry>();

    @Output()
    public OnEditCancelled: EventEmitter<void> = new EventEmitter<void>();

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

    public ngOnInit() {
        this._mode = this.country ? 'edit' : 'add';

        if (this.country) {
            this.name.setValue(this.country.name);
            this.code.setValue(this.country.code);
            this.code.disable();

            this.countryForm.markAsPristine();
        }
    }

    public onSubmit() {
        if (!this.countryForm.valid) {
            return;
        }

        const name = this.name.value;
        const code = this.code.value;

        if (this._mode === 'add') {
            this.addCountry(name, code);
        } else {
            this.updateCountry(name, code);
        }
    }

    onCancel() {
        this.OnEditCancelled.emit();
    }

    private addCountry(name: string, code: string) {
        const req = new AddAvailableCountryRequest(name, code);
        this.countryService.addAvailableCountry(req)
            .subscribe(() => {
                this.OnEditCompleted.emit({name, code});
            });
    }

    private updateCountry(name: string, code: string) {
        const req = new UpdateAvailableCountryRequest(name, code);
        this.countryService.updateAvailableCountry(req)
            .subscribe(() => {
                this.OnEditCompleted.emit({name, code});
            });
    }
}
