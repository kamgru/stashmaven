import {Component, Input} from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";

@Component({
  selector: 'app-partner-details',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './partner-details.component.html',
  styleUrls: ['./partner-details.component.css']
})
export class PartnerDetailsComponent {
  @Input() set partnerId(partnerId: string | null) {
    console.log('partnerId', partnerId)
  }

  partnerForm: FormGroup = new FormGroup({
    customIdentifier: new FormControl('', [Validators.required, Validators.minLength(3)]),
    legalName: new FormControl('', [Validators.required, Validators.minLength(3)]),
    primaryTaxIdentifier: new FormControl(''),
    city: new FormControl(''),
    street: new FormControl(''),
    postalCode: new FormControl(''),
  });
}
