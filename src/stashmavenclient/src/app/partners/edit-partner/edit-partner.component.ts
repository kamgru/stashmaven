import {Component, Input, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {EditPartnerService, Partner, PatchPartnerRequest} from "./edit-partner.service";
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule} from "@angular/forms";
import {PartnerAddress} from "../partnerAddress";

@Component({
    selector: 'app-edit-partner',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './edit-partner.component.html',
    styleUrls: ['./edit-partner.component.css']
})
export class EditPartnerComponent implements OnInit {

    @Input() partnerId: string | null = null;

    partner: Partner | null = null;

    showAddTaxIdentifier = false;

    partnerForm = new FormGroup({
        customIdentifier: new FormControl<string>(''),
        legalName: new FormControl<string>(''),
        taxIdentifiers: this.formBuilder.array<FormGroup>([]),
        address: new FormGroup({
            street: new FormControl<string>(''),
            streetAdditional: new FormControl<string | null>(''),
            city: new FormControl<string>(''),
            state: new FormControl<string | null>(''),
            postalCode: new FormControl<string>(''),
            countryCode: new FormControl<string>(''),
        })
    });

    addTaxIdentifierForm = new FormGroup({
        value: new FormControl<string>(''),
        type: new FormControl<string>(''),
    });

    constructor(
        private editPartnerService: EditPartnerService,
        private formBuilder: FormBuilder
    ) {
    }

    ngOnInit() {
        if (!this.partnerId) {
            return;
        }

        this.editPartnerService.getPartner(this.partnerId)
            .subscribe(partner => {
                if (!partner) return;

                this.partner = partner;
                this.partnerForm.patchValue(partner);

                for (let taxIdentifier of this.partner.taxIdentifiers) {
                    const formGroup = new FormGroup({
                        value: new FormControl<string>(taxIdentifier.value),
                        type: new FormControl<string>(taxIdentifier.type?.toString()),
                        isPrimary: new FormControl<boolean>(taxIdentifier.isPrimary)
                    });
                    this.partnerForm.controls.taxIdentifiers.push(formGroup);
                }
            });
    }

    onSubmit() {

        const req = new PatchPartnerRequest();
        let isDirty = false;

        if (this.partnerForm.controls.customIdentifier.dirty) {
            req.customIdentifier = this.partnerForm.controls.customIdentifier.value;
            isDirty = true;
        }

        if (this.partnerForm.controls.legalName.dirty) {
            req.legalName = this.partnerForm.controls.legalName.value;
            isDirty = true;
        }

        if (this.partnerForm.controls.address.dirty) {
            req.address = this.partnerForm.controls.address.value as PartnerAddress;
            isDirty = true;
        }

        if (this.partnerForm.controls.taxIdentifiers.dirty) {
            req.taxIdentifiers = this.partnerForm.controls.taxIdentifiers.value;
            isDirty = true;
        }

        if (!isDirty) {
            return;
        }

        this.editPartnerService.patchPartner(this.partnerId!, req)
            .subscribe(x => console.log(x))
    }

    removeTaxIdentifier(idx: number) {
       this.partnerForm.controls.taxIdentifiers.controls.splice(idx, 1);
    }

    onTaxIdPrimaryChange(idx: number) {
       for (let i = 0; i < this.partnerForm.controls.taxIdentifiers.controls.length; i++) {
           if (i === idx) continue;
           this.partnerForm.controls.taxIdentifiers.controls[i].controls['isPrimary'].setValue(false);
       }
    }

    addTaxIdentifier() {
        this.showAddTaxIdentifier = false;
        const formGroup = new FormGroup({
            value: new FormControl<string | null>(this.addTaxIdentifierForm.controls.value.value),
            type: new FormControl<string | null>(this.addTaxIdentifierForm.controls.type.value),
            isPrimary: new FormControl<boolean>(false)
        });
        this.partnerForm.controls.taxIdentifiers.push(formGroup);
        this.partnerForm.controls.taxIdentifiers.markAsDirty()
    }
}
