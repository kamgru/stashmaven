import {Component} from '@angular/core';
import {CountryService, IAvailableCountry} from "../services/country.service";
import {AsyncPipe} from "@angular/common";
import {EditCountryComponent} from "./edit-country/edit-country.component";

@Component({
    selector: 'app-countries',
    standalone: true,
    imports: [
        AsyncPipe,
        EditCountryComponent
    ],
    templateUrl: './countries.component.html',
    styleUrl: './countries.component.css'
})
export class CountriesComponent {

    public countries$ = this.countryService.getAvailableCountries();
    public uiState: 'list' | 'edit' = 'list';

    public selectedCountry: IAvailableCountry | null = null;

    constructor(
        private countryService: CountryService
    ) {
    }

    handleCountryClicked(country: IAvailableCountry) {
       this.selectedCountry = country;
       this.uiState = 'edit';
    }

    handleCountryAdded($event: IAvailableCountry) {
        this.countries$ = this.countryService.getAvailableCountries();
        this.uiState = 'list';
    }

    handleAddCountry() {
        this.selectedCountry = null;
        this.uiState = 'edit';
    }
}
