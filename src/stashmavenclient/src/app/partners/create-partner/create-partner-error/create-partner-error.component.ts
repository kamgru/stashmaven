import {Component, Input} from "@angular/core";
import {IApiError} from "../../../common/IApiError";

@Component({
    selector: 'app-create-partner-error',
    standalone: true,
    templateUrl: './create-partner-error.component.html',
    styleUrls: ['./create-partner-error.component.css']
})
export class CreatePartnerErrorComponent {
    @Input() error!: IApiError;


}