import {Component, EventEmitter, Output} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ListTaxDefinitionsService, TaxDefinition} from "./list-tax-definitions.service";

@Component({
    selector: 'app-list-tax-definitions',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './list-tax-definitions.component.html',
    styleUrls: ['./list-tax-definitions.component.css']
})
export class ListTaxDefinitionsComponent {

    taxDefinitions: TaxDefinition[] = [];

    @Output() onEdit = new EventEmitter<TaxDefinition>();

    constructor(
        private listTaxDefinitionsService: ListTaxDefinitionsService
    ) {
    }

    ngOnInit() {
        this.listTaxDefinitionsService.listAll()
            .subscribe((data) => {
                this.taxDefinitions = data.items;
            });
    }

    reload(){
        this.listTaxDefinitionsService.listAll()
            .subscribe((data) => {
                this.taxDefinitions = data.items;
            });
    }

    notifyOnEdit(taxDefinition: TaxDefinition){
        this.onEdit.emit(taxDefinition);
    }
}
