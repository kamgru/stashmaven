import {Component, signal} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormBuilder, ReactiveFormsModule, Validators} from "@angular/forms";
import {
    ListTaxDefinitionsService, TaxDefinition
} from "../../../common/tax-definitions/list-tax-definitions/list-tax-definitions.service";
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

    taxDefinitions: TaxDefinition[] = [];
    unitsOfMeasure: string[] = Object.keys(UnitOfMeasure).filter(k => typeof UnitOfMeasure[k as any] === "number");

    addCatalogItemForm = this.formBuilder.group({
        name: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        sku: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        taxDefinitionId: [this.taxDefinitions, Validators.required],
        unitOfMeasure: [this.unitsOfMeasure, Validators.required],
    });

    constructor(
        private formBuilder: FormBuilder,
        private listTaxDefinitionsService: ListTaxDefinitionsService,
        private addCatalogItemService: AddCatalogItemService
    ) {
    }

    ngOnInit() {
        this.listTaxDefinitionsService.listAll().subscribe(data => {
            this.taxDefinitions = data.items;
            this.addCatalogItemForm.patchValue({
                taxDefinitionId: this.taxDefinitions
            });
        });
    }

    notifyOnCancel() {

    }

    onSubmit() {
        if (!this.addCatalogItemForm.valid) {
            return;
        }

        const req = new AddCatalogItemRequest(
            this.addCatalogItemForm.value.name!,
            this.addCatalogItemForm.value.sku!,
            this.addCatalogItemForm.value.taxDefinitionId!.toString(),
            this.addCatalogItemForm.value.unitOfMeasure!.toString()
        );

        this.addCatalogItemService.add(req)
            .subscribe(() => {
                this.addCatalogItemForm.reset();
                this.notifyOnCancel();
            });
    }
}
