import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormBuilder, ReactiveFormsModule, Validators} from "@angular/forms";
import {AddPartnerRequest, AddPartnerService} from "./add-partner.service";
import {PartnerAddress} from "../partnerAddress";
import {TaxIdentifierType} from "../taxIdentifierType";
import {TaxIdentifier} from "../taxIdentifier";
import {CountryService} from "../../common/services/country.service";
import {catchError} from "rxjs";
import {IApiError} from "../../common/IApiError";
import {environment} from "../../../environments/environment";
import {AddPartnerErrorComponent} from "./add-partner-error/add-partner-error.component";

export interface IAddedPartner {
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
    selector: 'app-add-partner',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, AddPartnerErrorComponent],
    templateUrl: './add-partner.component.html',
    styleUrls: ['./add-partner.component.css']
})
export class AddPartnerComponent implements OnInit {

    public error: IApiError | null = null;

    public get customIdentifier() {
        return this.partnerForm.get('customIdentifier')!;
    }

    public get legalName() {
        return this.partnerForm.get('legalName');
    }

    public get nip() {
        return this.partnerForm.get('nip');
    }

    public get street(){
        return this.partnerForm.get('address')!.get('street');
    }

    public get city(){
        return this.partnerForm.get('address')!.get('city');
    }

    public get postalCode(){
        return this.partnerForm.get('address')!.get('postalCode');
    }

    @Output()
    public OnPartnerAdded: EventEmitter<IAddedPartner> = new EventEmitter<IAddedPartner>();

    @Output()
    public OnAddPartnerCancelled: EventEmitter<void> = new EventEmitter<void>();

    public partnerForm = this.formBuilder.group({
        customIdentifier: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(20)]],
        legalName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(256)]],
        isRetail: [false],
        nip: ['', [Validators.required, Validators.minLength(10)]],
        krs: [''],
        regon: [''],
        address: this.formBuilder.group({
            city: ['', {validators: [Validators.required], nonNullable: true}],
            street: ['', {validators: [Validators.required], nonNullable: true}],
            streetAdditional: [''],
            postalCode: ['', {validators: [Validators.required], nonNullable: true}],
            country: ['', {validators: [Validators.required], nonNullable: true}],
        }),
    })

    public countries$ = this.countryService.getAvailableCountries();

    constructor(
        private formBuilder: FormBuilder,
        private partnersService: AddPartnerService,
        private countryService: CountryService,
    ) {
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

        const request = new AddPartnerRequest(
            this.partnerForm.value.customIdentifier!,
            this.partnerForm.value.legalName!,
            [nipIdentifier],
            address,
            this.partnerForm.value.isRetail!,
        )

        this.partnerForm.disable();

        this.partnersService.addPartner(request).pipe(
            catchError((error) => {
                if (!Number.isFinite(error.error)) {
                    throw error;
                }

                this.error = <IApiError>{
                    errorCode: error.error,
                    endpoint: error.url.replace(`${environment.apiUrl}/`, ''),
                    requestBody: request
                };

                this.partnerForm.enable();

                return [];
            })
        )
            .subscribe(p => {
                this.OnPartnerAdded.emit({
                    partnerId: p.partnerId,
                    customIdentifier: request.customIdentifier,
                    legalName: request.legalName,
                    isRetail: request.isRetail,
                    nip: request.taxIdentifiers[0].value,
                    krs: '',
                    regon: '',
                    address: request.address,
                });
            });
    }

    handleCancel() {
        this.OnAddPartnerCancelled.emit();
    }
}
