import {Component, Input} from '@angular/core';
import {ErrorCodeTranslatorService} from "../../services/error-code-translator.service";

export class Notification {
    private constructor(
        public readonly type: 'success' | 'error' | 'warning' | 'info',
        public readonly errorCode: number | null,
        public readonly message: string | null = null
    ) {

    }

    public static success(message: string): Notification {
        return new Notification('success', null, message);
    }

    public static error(errorCode: number): Notification {
        return new Notification('error', errorCode);
    }

    public static warning(errorCode: number): Notification {
        return new Notification('warning', errorCode);
    }

    public static info(errorCode: number): Notification {
        return new Notification('info', errorCode);
    }
}

@Component({
    selector: 'app-notification',
    standalone: true,
    imports: [],
    templateUrl: './notification.component.html'
})
export class NotificationComponent {

    public alertClass: string | null = null;
    public notificationMessage: string | null = null;

    @Input({required: true})
    public set notification(value: Notification | null) {
        if (value == null) {
            this.alertClass = null;
            this.notificationMessage = null;
            return;
        }

        switch (value.type) {
            case 'success':
                this.alertClass = 'alert-success';
                break;
            case 'error':
                this.alertClass = 'alert-danger';
                break;
            case 'warning':
                this.alertClass = 'alert-warning';
                break;
            case 'info':
                this.alertClass = 'alert-info';
                break;
            default:
                this.alertClass = null;
        }

        this.notificationMessage = value.type == 'success'
            ? this.notificationMessage = value.message
            : this.errorCodeTranslator.translateErrorCode(value.errorCode!);
    }

    constructor(
        private errorCodeTranslator: ErrorCodeTranslatorService
    ) {
    }
}
