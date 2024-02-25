import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormBuilder, ReactiveFormsModule, Validators} from "@angular/forms";
import {CreatePartnerRequest, CreatePartnerService} from "./create-partner.service";
import {PartnerAddress} from "../partnerAddress";
import {TaxIdentifierType} from "../taxIdentifierType";
import {TaxIdentifier} from "../taxIdentifier";
import {CountryService} from "../../common/services/country.service";

export interface ICreatedPartner {
    partnerId: string;
    customIdentifier: string;
    legalName: string;
    isRetail: boolean;
    nip: string;
    krs: string;
    regon: string;
    address: PartnerAddress;
}
@Component({
    selector: 'app-create-partner',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './create-partner.component.html',
    styleUrls: ['./create-partner.component.css']
})
export class CreatePartnerComponent implements OnInit {

    @Output()
    public OnPartnerCreated: EventEmitter<ICreatedPartner> = new EventEmitter<ICreatedPartner>();

    public partnerForm = this.formBuilder.group({
        customIdentifier: ['', [Validators.required, Validators.minLength(3)]],
        legalName: ['', [Validators.required, Validators.minLength(3)]],
        isRetail: [false],
        nip: ['', [Validators.required, Validators.minLength(10)]],
        krs: [''],
        regon: [''],
        address: this.formBuilder.group({
            city: ['', { validators: [Validators.required], nonNullable: true }],
            street: ['', { validators: [Validators.required], nonNullable: true }],
            streetAdditional: [''],
            postalCode: ['', { validators: [Validators.required], nonNullable: true }],
            country: ['', { validators: [Validators.required], nonNullable: true }],
        }),
    })

    public countries$ = this.countryService.getAvailableCountries();

    constructor(
        private formBuilder: FormBuilder,
        private partnersService: CreatePartnerService,
        private countryService: CountryService) {
    }

    ngOnInit() {
        this.partnerForm.get('isRetail')!.valueChanges.subscribe((value) => {
            const nipControl = this.partnerForm.get('nip');
            if (!nipControl) {
                throw new Error('Nip control not found');
            }

            if (value) {
                nipControl.clearValidators();
                nipControl.disable();
            } else {
                nipControl.setValidators([Validators.required, Validators.minLength(10)]);
                nipControl.enable();
            }
            nipControl.updateValueAndValidity();
        });
    }

    onSubmit() {
        if (this.partnerForm.invalid) {
            return;
        }

        const addressControl = this.partnerForm.get('address');
        if (!addressControl) {
            throw new Error('Address control not found');
        }

        const addressValue = addressControl.value;
        if (!addressValue) {
            throw new Error('Address value not found');
        }

        const address = new PartnerAddress(
            addressValue.street!,
            addressValue.city!,
            addressValue.postalCode!,
            addressValue.country!,
        );

        const nipIdentifier = new TaxIdentifier(
            TaxIdentifierType.Nip,
            this.partnerForm.value.nip ?? '0',
            true
        )

        const request = new CreatePartnerRequest(
            this.partnerForm.value.customIdentifier!,
            this.partnerForm.value.legalName!,
            [nipIdentifier],
            address
        )

        this.partnersService.createPartner(request)
            .subscribe(() => {
                console.log('Partner created')
            });
    }
}
