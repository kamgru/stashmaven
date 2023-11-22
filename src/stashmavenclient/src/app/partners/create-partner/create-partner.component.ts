import {Component, Input} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormBuilder, ReactiveFormsModule, Validators} from "@angular/forms";
import {
    CreatePartnerRequest,
    CreatePartnerService
} from "./create-partner.service";
import {PartnerAddress} from "../partnerAddress";
import {TaxIdentifierType} from "../taxIdentifierType";
import {TaxIdentifier} from "../taxIdentifier";

@Component({
    selector: 'app-create-partner',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './create-partner.component.html',
    styleUrls: ['./create-partner.component.css']
})
export class CreatePartnerComponent {

    partnerForm = this.formBuilder.group({
        customIdentifier: ['', [Validators.required, Validators.minLength(3)]],
        legalName: ['', [Validators.required, Validators.minLength(3)]],
        primaryTaxIdentifier: ['', [Validators.required, Validators.minLength(10)]],
        address: this.formBuilder.group({
            city: ['', Validators.required],
            street: ['', Validators.required],
            postalCode: ['', Validators.required],
        }),
    })

    constructor(
        private formBuilder: FormBuilder,
        private partnersService: CreatePartnerService) {
    }

    onSubmit() {
        console.log(this.partnerForm)
        const address = new PartnerAddress(
            this.partnerForm.value!.address!.street!,
            this.partnerForm.value!.address!.city!,
            this.partnerForm.value!.address!.postalCode!,
            'PL',
        );

        const nipIdentifier = new TaxIdentifier(
            TaxIdentifierType.Nip,
            this.partnerForm.value.primaryTaxIdentifier!,
            true
        )

        const request = new CreatePartnerRequest(
            this.partnerForm.value.customIdentifier!,
            this.partnerForm.value.legalName!,
            [nipIdentifier],
            address
        )

        console.log(request)

        this.partnersService.createPartner(request)
            .subscribe(() => {
                console.log('Partner created')
            });
    }
}
