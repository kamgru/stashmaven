import {Component} from '@angular/core';
import {AddProductComponent} from "../../catalog/products/add-product/add-product.component";
import {FaIconComponent, FaIconLibrary} from "@fortawesome/angular-fontawesome";
import {ListProductsComponent} from "../../common/components/list-products/list-products.component";
import {IPartner, ListPartnersComponent} from "../../common/components/list-partners";
import {faPlus} from "@fortawesome/free-solid-svg-icons";
import {AddPartnerComponent, PartnerAddedEvent} from "./add-partner/add-partner.component";
import {BusinessIdentifierService, IBusinessIdentifierType} from "../../common/services/business-identifier.service";
import {AsyncPipe} from "@angular/common";
import {CountryService, IAvailableCountry} from "../../common/services/country.service";
import {catchError, combineLatest} from "rxjs";
import {map} from "rxjs";
import {
    AddPartnerAddress,
    AddPartnerBusinessIdentifier,
    AddPartnerRequest,
    PartnerService
} from "../../common/services/partner.service";
import {Notification, NotificationComponent} from "../../common/components/notification/notification.component";

@Component({
    selector: 'app-partners',
    standalone: true,
    imports: [
        AddProductComponent,
        FaIconComponent,
        ListProductsComponent,
        ListPartnersComponent,
        AddPartnerComponent,
        AsyncPipe,
        NotificationComponent
    ],
    templateUrl: './partners.component.html'
})
export class PartnersComponent {

    private _businessIdentifiers$ = this.businessIdentifierService.getBusinessIdentifierTypes();
    private _countries$ = this.countryService.getAvailableCountries();

    public uiState: 'list' | 'add' = 'list';
    public notification: Notification | null = null;

    public data$ = combineLatest([this._businessIdentifiers$, this._countries$])
        .pipe(
            map(([businessIdentifiers, countries]) => (
                <{
                    businessIdentifiers: IBusinessIdentifierType[],
                    countries: IAvailableCountry[]
                }>{businessIdentifiers, countries}
            ))
        )

    public constructor(
        private businessIdentifierService: BusinessIdentifierService,
        private countryService: CountryService,
        private partnerService: PartnerService,
        fa: FaIconLibrary
    ) {
        fa.addIcons(faPlus);
    }

    handleItemConfirmed($event: IPartner) {

    }

    handlePartnerAdded($event: PartnerAddedEvent) {
        const req = new AddPartnerRequest(
            $event.customIdentifier,
            $event.legalName,
            new AddPartnerAddress(
                $event.street,
                $event.streetAdditional,
                $event.city,
                $event.postalCode,
                $event.country
            ),
            $event.businessIdentifiers.map(x => new AddPartnerBusinessIdentifier(
                x.type,
                x.value
            ))
        );

        this.partnerService.addPartner(req)
            .pipe(
                catchError(err => {
                    this.notification = Notification.error(err.error);
                    window.scrollTo(0, 0);
                    return [];
                }))
            .subscribe(x => {
                this.notification = null;
                this.uiState = 'list'
            });
    }

    handleAddClicked() {
        this.notification = null;
        this.uiState = 'add';
    }
}
