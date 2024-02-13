import {Component, signal, ViewChild} from '@angular/core';
import {CommonModule} from '@angular/common';
import {AddTaxDefinitionComponent, TaxDefinitionAddedEvent} from "./add-tax-definition/add-tax-definition.component";
import {ListTaxDefinitionsComponent} from "./list-tax-definitions/list-tax-definitions.component";
import {TaxDefinition} from "./list-tax-definitions/list-tax-definitions.service";
import {EditTaxDefinitionComponent} from "./edit-tax-definition/edit-tax-definition.component";

enum UiState {
    List,
    Add,
    Edit
}

@Component({
    selector: 'app-tax-definitions',
    standalone: true,
    imports: [CommonModule, AddTaxDefinitionComponent, ListTaxDefinitionsComponent, EditTaxDefinitionComponent],
    templateUrl: './tax-definitions.component.html',
    styleUrls: ['./tax-definitions.component.css']
})
export class TaxDefinitionsComponent {

    @ViewChild(ListTaxDefinitionsComponent) listTaxDefinitionsComponent?: ListTaxDefinitionsComponent
    @ViewChild(EditTaxDefinitionComponent) editTaxDefinitionComponent?: EditTaxDefinitionComponent

    uiState = UiState.List;
    selectedTaxDefinition?: TaxDefinition;

    handleTaxDefinitionAdded($event: TaxDefinitionAddedEvent) {
        this.listTaxDefinitionsComponent?.reload();
        this.uiState = UiState.List;
    }

    showAdd() {
        this.uiState = UiState.Add;
    }

    handleOnEdit($event: TaxDefinition) {
        this.selectedTaxDefinition = $event;
        console.log(this.selectedTaxDefinition);
        this.uiState = UiState.Edit;
    }

    handleOnSave() {
        this.listTaxDefinitionsComponent?.reload();
        this.uiState = UiState.List;
    }

    handleOnCancel() {
        this.selectedTaxDefinition = undefined;
        this.uiState = UiState.List;
    }
}
