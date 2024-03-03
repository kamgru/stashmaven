import {Component, Input, OnInit} from "@angular/core";
import {IApiError} from "../../../common/IApiError";
import {IAddedPartner} from "../add-partner.component";
import {AddPartnerRequest} from "../add-partner.service";
import {ViewportScroller} from "@angular/common";

@Component({
    selector: 'app-add-partner-error',
    standalone: true,
    templateUrl: './add-partner-error.component.html',
    styleUrls: ['./add-partner-error.component.css']
})
export class AddPartnerErrorComponent implements OnInit {
    @Input()
    public error!: IApiError;

    public message: string = '';

    ngOnInit() {
        const request = this.error.requestBody as AddPartnerRequest;
        if (!request) {
            throw new Error('Request body is missing');
        }

        if (this.error.errorCode == 100001){
           this.message = `Partner with custom identifier ${request.customIdentifier} already exists`;
        }
        else if (this.error.errorCode == 100003){
            this.message = 'Only one tax identifier can be primary';
        }
        else if (this.error.errorCode == 100004){
            this.message = 'Tax identifier type not supported';
        }
        else if (this.error.errorCode == 100005){
            this.message = 'Tax identifier type not unique';
        }
        else if (this.error.errorCode == 100006){
            this.message = 'Tax identifier value not unique';
        }
        else if (this.error.errorCode == 100007){
            this.message = 'Country code not supported';
        }
    }
}