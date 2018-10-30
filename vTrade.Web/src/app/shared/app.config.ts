import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { IAppConfig } from '../models/app-config.model';

@Injectable()
export class AppConfig {

    static settings: IAppConfig;

    constructor(private readonly http: HttpClient) { }

    load(): Promise<any> {
        const jsonFile = `./../../assets/config.json`;

        return new Promise((resolve, reject) => {
            this.http.get(jsonFile)
                .subscribe((config: IAppConfig) => {
                    AppConfig.settings = config;
                    resolve();
                }, (err: any) => {
                    reject(`Could not load file '${jsonFile}': ${JSON.stringify(err)}`);
                });
        });
    }

    static getHomeUrlByRole(role: string): string {
        switch (role) {
            case "user":
                return "user";
            case "admin":
                return "admin";
            default:
                return null;
        }
    }
}
