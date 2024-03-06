import {Component, EventEmitter, Input, OnChanges, Output} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormBuilder, ReactiveFormsModule, Validators} from "@angular/forms";
import {TaxDefinition, TaxDefinitionService} from "../../services/tax-definition.service";

@Component({
    selector: 'app-edit-tax-definition',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './edit-tax-definition.component.html',
    styleUrls: ['./edit-tax-definition.component.css']
})
export class EditTaxDefinitionComponent implements OnChanges {

    @Input() taxDefinition?: TaxDefinition;
    @Output() onCancel = new EventEmitter<void>();
    @Output() onSave = new EventEmitter<TaxDefinition>();

    editTaxDefinitionForm = this.formBuilder.group({
        name: [this.taxDefinition?.name, [Validators.required, Validators.maxLength(50)]],
    });

    canDelete = {
        value: true,
        reason: ''
    }

    constructor(
        private formBuilder: FormBuilder,
        private editTaxDefinitionService: TaxDefinitionService
    ) {
    }

    notifyOnCancel() {
        this.onCancel.emit();
    }

    ngOnChanges() {
        if (this.taxDefinition) {
            this.editTaxDefinitionForm.patchValue({
                name: this.taxDefinition.name
            });
        }
    }

    onSubmit() {
        if (!this.editTaxDefinitionForm.valid) {
            return;
        }

        this.editTaxDefinitionService.patch(
            this.taxDefinition!.taxDefinitionId,
            this.editTaxDefinitionForm.value.name!)
            .subscribe(() => {
                this.onSave.emit(this.taxDefinition!);
            })
    }

    onDelete() {
        this.editTaxDefinitionService.tryDelete(this.taxDefinition!.taxDefinitionId)
            .subscribe(
                {
                    next: () => {
                        this.onSave.emit(this.taxDefinition!);
                    },
                    error: (err: Error) => {
                        this.canDelete = {
                            value: false,
                            reason: err.message
                        }
                    },
                }
            )
    }
}
