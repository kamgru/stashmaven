import {Component, EventEmitter, Output} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormBuilder, ReactiveFormsModule, Validators} from "@angular/forms";
import {UnitOfMeasure} from "../../../common/unitOfMeasure";
import {AddCatalogItemRequest, AddCatalogItemService} from "./add-catalog-item.service";

@Component({
    selector: 'app-add-catalog-item',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './add-catalog-item.component.html',
    styleUrls: ['./add-catalog-item.component.css']
})
export class AddCatalogItemComponent {

    unitsOfMeasure: string[] = Object.keys(UnitOfMeasure).filter(k => typeof UnitOfMeasure[k as any] === "number");

    addCatalogItemForm = this.formBuilder.group({
        name: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        sku: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        unitOfMeasure: [this.unitsOfMeasure, Validators.required],
    });

    @Output() onAdded = new EventEmitter<void>();
    @Output() onCancel = new EventEmitter<void>();

    constructor(
        private formBuilder: FormBuilder,
        private addCatalogItemService: AddCatalogItemService
    ) {
    }

    onSubmit() {
        if (!this.addCatalogItemForm.valid) {
            return;
        }

        const req = new AddCatalogItemRequest(
            this.addCatalogItemForm.value.name!,
            this.addCatalogItemForm.value.sku!,
            this.addCatalogItemForm.value.unitOfMeasure!.toString()
        );

        this.addCatalogItemService.add(req)
            .subscribe(_ => {
                this.addCatalogItemForm.reset();
                this.onAdded.emit();
            });
    }
}
