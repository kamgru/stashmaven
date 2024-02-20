import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormBuilder, FormControl, ReactiveFormsModule, Validators} from "@angular/forms";
import {CreatePartnerRequest, CreatePartnerService} from "./create-partner.service";
import {PartnerAddress} from "../partnerAddress";
import {TaxIdentifierType} from "../taxIdentifierType";
import {TaxIdentifier} from "../taxIdentifier";
import {CountryService, IAvailableCountry} from "../../common/services/country.service";

@Component({
    selector: 'app-create-partner',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './create-partner.component.html',
    styleUrls: ['./create-partner.component.css']
})
export class CreatePartnerComponent {

    private _availableCountries: IAvailableCountry[] = [];

    public partnerForm = this.formBuilder.group({
        customIdentifier: ['', [Validators.required, Validators.minLength(3)]],
        legalName: ['', [Validators.required, Validators.minLength(3)]],
        isRetail: [false],
        nip: ['', [Validators.required, Validators.minLength(10)]],
        krs: [''],
        regon: [''],
        address: this.formBuilder.group({
            city: ['', Validators.required],
            street: ['', Validators.required],
            streetAdditional: [''],
            postalCode: ['', Validators.required],
            country: [null]
        }),
    })

    constructor(
        private formBuilder: FormBuilder,
        private partnersService: CreatePartnerService,
        private countryService: CountryService) {
    }

    ngOnInit(): void {
        this.countryService.getAvailableCountries()
            .subscribe(countries => {
                this._availableCountries = countries
                this.partnerForm.patchValue({
                    address: {
                        country: countries.find(c => c.code === 'PL') ?? null
                    })
                });
            });
    }

    onSubmit() {
        const address = new PartnerAddress(
            this.partnerForm.value!.address!.street!,
            this.partnerForm.value!.address!.city!,
            this.partnerForm.value!.address!.postalCode!,
            'PL',
        );

        const nipIdentifier = new TaxIdentifier(
            TaxIdentifierType.Nip,
            this.partnerForm.value.nip!,
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
