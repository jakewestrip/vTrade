import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { JWT_OPTIONS, JwtModule } from '@auth0/angular-jwt';

import { AuthService } from './auth.service';

@NgModule({
    providers: [
        AuthService
    ],
    imports: [
        CommonModule
    ]
})
export class AuthModule {
}