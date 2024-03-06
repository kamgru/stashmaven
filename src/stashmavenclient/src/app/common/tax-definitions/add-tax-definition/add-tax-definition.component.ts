import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormBuilder, ReactiveFormsModule, Validators} from "@angular/forms";
import {
    AddTaxDefinitionRequest,
    AddTaxDefinitionResponse,
    TaxDefinitionService
} from "../../services/tax-definition.service";
import {CountryService, IAvailableCountry} from "../../services/country.service";

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
export class AddTaxDefinitionComponent implements OnInit {

    @Output()
    public taxDefinitionAdded = new EventEmitter<TaxDefinitionAddedEvent>();

    public countries: IAvailableCountry[] = [];


    addTaxDefinitionForm = this.formBuilder.group({
        name: ['', [Validators.required, Validators.maxLength(50)]],
        rate: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
        country: ['', [Validators.required]]
    });

    constructor(
        private formBuilder: FormBuilder,
        private taxDefinitionService: TaxDefinitionService,
        private countryService: CountryService
    ) {
    }

    ngOnInit() {
        this.countryService.getAvailableCountries()
            .subscribe(x => this.countries = x);
    }

    onSubmit() {
        if (this.addTaxDefinitionForm.valid) {
            const req = new AddTaxDefinitionRequest(
                this.addTaxDefinitionForm.value.name!,
                this.addTaxDefinitionForm.value.rate!,
                this.addTaxDefinitionForm.value.country!
            );
            this.taxDefinitionService.addTaxDefinition(req)
                .subscribe((res: AddTaxDefinitionResponse) => {
                    this.addTaxDefinitionForm.reset();
                    this.taxDefinitionAdded.emit(new TaxDefinitionAddedEvent(res.value, req.name, req.rate));
                });
        }
    }
}
