import { Injectable } from '@angular/core';

import { JwtHelperService } from '@auth0/angular-jwt';

import { AppConfig } from '../shared/app.config';

@Injectable()
export class AuthService {
    private academicRole: string;
    private studentRole: string;
    private staffRole: string;
    private supervisorRole: string;
    private jwtHelperService: JwtHelperService

    constructor() {
        this.jwtHelperService = new JwtHelperService();

        if (!this.isAuthenticated()) {
            const hash = window.location.hash.substr(1);
            if (hash) {
                this.saveTokens(decodeURIComponent(hash));
            } else {
                this.authorize();
            }
        }
    }

    isAuthenticated(): boolean {
        const accessToken = localStorage.getItem('accesstoken');
        if (accessToken) {
            return !this.jwtHelperService.isTokenExpired(accessToken);
        } else {
            return false;
        }
    }

    authorize(): void {
        localStorage.setItem('referrerUrl', window.location.href);

        const authorizationUrl = AppConfig.settings.signOnServerAdress;
        const redirectUri = `${AppConfig.settings.appAddress}/`;

        const clientId = AppConfig.settings.clientId; const responseType = 'code id_token token';
        const scope = 'openid profile role';
        const nonce = `N${Math.random()}${Date.now()}`;
        const state = `${Date.now()}${Math.random()}`;
        const url = `${authorizationUrl}connect/authorize?response_type=${encodeURI(responseType)}&client_id=${encodeURI(clientId)}&redirect_uri=${encodeURI(redirectUri)}&scope=${encodeURI(scope)}&nonce=${encodeURI(nonce)}&state=${encodeURI(state)}`;

        window.location.href = url;
    }

    logout(): void {
        const authorizationUrl = AppConfig.settings.signOnServerAdress;
        const id_token = localStorage.getItem('idtoken');
        const id_token_hint = id_token;
        const post_logout_redirect_uri = AppConfig.settings.appAddress;
        const state = `${Date.now()}${Math.random()}`;

        const url = `${authorizationUrl}connect/endsession?id_token_hint=${id_token_hint}&post_logout_redirect_uri=${encodeURI(post_logout_redirect_uri)}&state=${encodeURI(state)}`;

        this.clearAuthorizationData();
        window.location.href = url;
    }

    referrerUrlExist(): boolean {
        return !!localStorage.getItem('referrerUrl');
    }

    navigateToReferrerUrl(): void {
        const refUrl = localStorage.getItem('referrerUrl');

        if (refUrl) {
            localStorage.removeItem('referrerUrl');
            window.location.href = refUrl;
        }
    }

    getUsername(): string {
        const token = this.getAccessTokenDataFromStorage();

        return token.name;
    }

    isAdmin(): boolean {
        const token = this.getAccessTokenDataFromStorage();
        if (token) {
            return token.role == '1';
        }

        return false;
    }

    saveTokens(hash: string): void {
        const response = hash.split('&').reduce((result, item) => {
            const parts = item.split('=');
            result[parts[0]] = parts[1];

            return result;
        }, new AccessTokenResponse());

        this.setAuthorizationData(response.access_token, response.id_token);
    }

    setAuthorizationData(accessToken: string, idToken: string): void {
        localStorage.setItem('accesstoken', accessToken);
        localStorage.setItem('idtoken', idToken);
    }
    clearAuthorizationData(): void {
        localStorage.removeItem('accesstoken');
        localStorage.removeItem('idtoken');
        localStorage.removeItem('referrerUrl');
        localStorage.removeItem('preferredDateTimeFormat');
    }

    getAccessTokenDataFromStorage(): DataAccessToken {
        const accessToken = localStorage.getItem('accesstoken');
        const decodedToken = this.jwtHelperService.decodeToken(accessToken);

        return decodedToken;
    }
}

export class AccessTokenResponse {
    access_token: string;
    code: string;
    expires_in: number;
    id_token: string;
    scope: string;
    session_state: string;
    state: string;
    token_type: string;
}

export class DataAccessToken {
    role: string;
    sub: string;
    name: string;
}
