import {Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {IBusinessIdentifierType} from "../../../common/services/business-identifier.service";
import {IAvailableCountry} from "../../../common/services/country.service";

export class PartnerAddedEvent {
    constructor(
        public readonly customIdentifier: string,
        public readonly legalName: string,
        public readonly isConsumer: boolean,
        public readonly businessIdentifiers: { value: string, type: string }[],
        public readonly city: string,
        public readonly street: string,
        public readonly streetAdditional: string,
        public readonly postalCode: string,
        public readonly country: string
    ) {
    }
}

@Component({
    selector: 'app-add-partner',
    standalone: true,
    imports: [
        ReactiveFormsModule
    ],
    templateUrl: './add-partner.component.html'
})
export class AddPartnerComponent implements OnInit {

    public addPartnerForm = this.fb.group({
        customIdentifier: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
        legalName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(2048)]],
        isConsumer: [false],
        businessIdentifiers: this.fb.array([]),
        city: ['', [Validators.required]],
        street: ['', [Validators.required]],
        streetAdditional: [''],
        postalCode: ['', [Validators.required]],
        country: ['', [Validators.required]]
    });

    @Input({required: true})
    public businessIdentifierTypes: IBusinessIdentifierType[] = [];

    @Input({required: true})
    public countries: IAvailableCountry[] = [];

    @Output()
    public OnPartnerAdded = new EventEmitter<PartnerAddedEvent>();

    @Output()
    public OnCancelled = new EventEmitter<void>();

    @ViewChild('nameInput', {static: true})
    public nameInput!: ElementRef<HTMLInputElement>;

    public get customIdentifier(): FormControl<string> {
        return this.addPartnerForm.get('customIdentifier') as FormControl<string>;
    }

    public get legalName(): FormControl<string> {
        return this.addPartnerForm.get('legalName') as FormControl<string>;
    }

    public get isConsumer(): FormControl<boolean> {
        return this.addPartnerForm.get('isConsumer') as FormControl<boolean>;
    }

    public get businessIdentifiers(): FormArray<FormGroup> {
        return this.addPartnerForm.get('businessIdentifiers') as FormArray<FormGroup>;
    }

    public get city(): FormControl<string> {
        return this.addPartnerForm.get('city') as FormControl<string>;
    }

    public get street(): FormControl<string> {
        return this.addPartnerForm.get('street') as FormControl<string>;
    }

    public get streetAdditional(): FormControl<string> {
        return this.addPartnerForm.get('streetAdditional') as FormControl<string>;
    }

    public get postalCode(): FormControl<string> {
        return this.addPartnerForm.get('postalCode') as FormControl<string>;
    }

    public get country(): FormControl<string> {
        return this.addPartnerForm.get('country') as FormControl<string>;
    }

    constructor(
        private fb: FormBuilder
    ) {
    }

    public ngOnInit(): void {
        this.nameInput.nativeElement.focus();

        this.businessIdentifierTypes.forEach((bi) => {
            const validators = bi.isPrimary ? [Validators.required, Validators.minLength(2)] : [];
            const ctrl = this.fb.group({
                value: ['', validators],
                typeId: [bi.id],
                typeName: [bi.name],
                isPrimary: [bi.isPrimary]
            });
            this.businessIdentifiers.push(ctrl);
        });

        this.addPartnerForm.updateValueAndValidity();
        this.addPartnerForm.markAsPristine();

        this.isConsumer.valueChanges.subscribe((value) => {
            const businessIdentifierGroup = this.businessIdentifiers.controls.find(c => c.value.isPrimary);
            const primaryIdentifierControl = businessIdentifierGroup?.get('value');

            if (primaryIdentifierControl) {
                if (value) {
                    primaryIdentifierControl.clearValidators();
                } else {
                    primaryIdentifierControl.setValidators([Validators.required, Validators.minLength(10)]);
                }
                primaryIdentifierControl.updateValueAndValidity();
            }
        });

    }

    handleSubmit() {
        if (!this.addPartnerForm.valid) {
            return;
        }

        const businessIdentifiers = this.businessIdentifiers.controls
            .map(c => ({
                value: c.get('value')?.value,
                type: c.get('typeId')?.value
            }));

        const evt = new PartnerAddedEvent(
            this.addPartnerForm.value.customIdentifier!,
            this.addPartnerForm.value.legalName!,
            this.addPartnerForm.value.isConsumer!,
            businessIdentifiers,
            this.addPartnerForm.value.city!,
            this.addPartnerForm.value.street!,
            this.addPartnerForm.value.streetAdditional!,
            this.addPartnerForm.value.postalCode!,
            this.addPartnerForm.value.country!
        );

        console.log(evt);

        this.OnPartnerAdded.emit(evt);
    }
}
