import {Component, Input, OnInit} from "@angular/core";
import {IApiError} from "../../../common/IApiError";
import {ICreatedPartner} from "../create-partner.component";
import {CreatePartnerRequest} from "../create-partner.service";
import {ViewportScroller} from "@angular/common";

@Component({
    selector: 'app-create-partner-error',
    standalone: true,
    templateUrl: './create-partner-error.component.html',
    styleUrls: ['./create-partner-error.component.css']
})
export class CreatePartnerErrorComponent implements OnInit {
    @Input()
    public error!: IApiError;

    public message: string = '';

    ngOnInit() {
        const request = this.error.requestBody as CreatePartnerRequest;
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