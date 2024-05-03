import {Injectable} from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class ErrorCodeTranslatorService {

    public translateErrorCode(code: number): string {
        switch (code) {
            case 400000:
                return 'Brand not found';
            case 400001:
                return 'Brand short code is already in use';
            default:
                return 'Unknown error';
        }
    }
}
