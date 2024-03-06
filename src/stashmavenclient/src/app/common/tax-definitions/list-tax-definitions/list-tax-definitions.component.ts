import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {CommonModule} from '@angular/common';
import {TaxDefinition, TaxDefinitionService} from "../../services/tax-definition.service";

@Component({
    selector: 'app-list-tax-definitions',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './list-tax-definitions.component.html',
    styleUrls: ['./list-tax-definitions.component.css']
})
export class ListTaxDefinitionsComponent implements OnInit {

    taxDefinitions: TaxDefinition[] = [];

    @Output() onEdit = new EventEmitter<TaxDefinition>();

    constructor(
        private listTaxDefinitionsService: TaxDefinitionService
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
