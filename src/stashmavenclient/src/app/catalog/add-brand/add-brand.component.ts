import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {AddBrandService} from "./add-brand.service";

@Component({
  selector: 'app-add-brand',
  standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-brand.component.html',
  styleUrls: ['./add-brand.component.css']
})
export class AddBrandComponent {

    brandForm = this.formBuilder.group({
        name: ['', [Validators.required, Validators.minLength(2)]],
        shortCode: ['', [Validators.required, Validators.minLength(2)]],
    })

    constructor(
        private formBuilder: FormBuilder,
        private brandsService: AddBrandService
    ) {
    }

    onSubmit() {
        this.brandsService.addBrand(
            this.brandForm.value.name!,
            this.brandForm.value.shortCode!,
        ).subscribe(data => {
            console.log('Brand added: ' + data)
        });
    }
}
