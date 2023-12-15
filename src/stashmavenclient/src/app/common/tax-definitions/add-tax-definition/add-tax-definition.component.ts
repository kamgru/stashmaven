import {Component, EventEmitter, Output} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormBuilder, ReactiveFormsModule, Validators} from "@angular/forms";
import {AddTaxDefinitionRequest, AddTaxDefinitionResponse, AddTaxDefinitionService} from "./add-tax-definition.service";

export class TaxDefinitionAddedEvent {
    constructor(
        public id: string,
        public name: string,
        public rate: number
    ) {
    }
}

@Component({
    selector: 'app-add-tax-definition',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './add-tax-definition.component.html',
    styleUrls: ['./add-tax-definition.component.css']
})
export class AddTaxDefinitionComponent {

    @Output() taxDefinitionAdded = new EventEmitter<TaxDefinitionAddedEvent>();

    addTaxDefinitionForm = this.formBuilder.group({
        name: ['', [Validators.required, Validators.maxLength(50)]],
        rate: [0, [Validators.required, Validators.min(0), Validators.max(100)]]
    });

    constructor(
        private formBuilder: FormBuilder,
        private addTaxDefinitionService: AddTaxDefinitionService
    ) {
    }

    onSubmit() {
        if (this.addTaxDefinitionForm.valid) {
            const req = new AddTaxDefinitionRequest(
                this.addTaxDefinitionForm.value.name!,
                this.addTaxDefinitionForm.value.rate!
            );
            this.addTaxDefinitionService.add(req)
                .subscribe((res: AddTaxDefinitionResponse) => {
                    this.addTaxDefinitionForm.reset();
                    this.taxDefinitionAdded.emit(new TaxDefinitionAddedEvent(res.value, req.name, req.rate));
                });
        }
    }
}
