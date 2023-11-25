import {MsalGuardConfiguration, MsalInterceptorConfiguration} from "@azure/msal-angular";
import {
    BrowserCacheLocation,
    InteractionType,
    IPublicClientApplication,
    LogLevel,
    PublicClientApplication
} from "@azure/msal-browser";

export function MSALGuardConfigFactory(): MsalGuardConfiguration {
    return {
        interactionType: InteractionType.Redirect,
        authRequest: {
            scopes: ['user.read']
        },
        loginFailedRoute: '/login-failed'
    };
}

export function loggerCallback(logLevel: LogLevel, message: string) {
    //console.log(message);
}

export function MSALInstanceFactory(): IPublicClientApplication {

    return new PublicClientApplication({
        auth: {
            clientId: 'e2adeaf2-c331-4cb4-b1df-d9f35fbde577',
            authority: 'https://login.microsoftonline.com/50e6b2a2-7d3b-4cfe-9f2a-93ba754dd7f8',
            redirectUri: 'http://localhost:4200/auth',
            clientCapabilities: ['CP1']
        },
        cache: {
            cacheLocation: BrowserCacheLocation.LocalStorage,
            storeAuthStateInCookie: true,
        },
        system: {
            allowNativeBroker: false,
            loggerOptions: {
                loggerCallback,
                logLevel: LogLevel.Info,
                piiLoggingEnabled: true,
            }
        },
    });
}

export function MSALInterceptorConfigFactory(): MsalInterceptorConfiguration {
    const protectedResourceMap = new Map<string, Array<string>>();
    protectedResourceMap.set('https://graph.microsoft.com/v1.0/me', ['user.read']);
    protectedResourceMap.set('http://localhost:5253/api/v1', ['api://569781c4-ad95-4aec-863a-604e6c9f9a13/StashMaven.Read'])

    return {
        interactionType: InteractionType.Redirect,
        protectedResourceMap
    }
}
