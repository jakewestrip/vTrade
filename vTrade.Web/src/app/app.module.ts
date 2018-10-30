import { BrowserModule } from '@angular/platform-browser';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { SidebarModule } from 'ng-sidebar';
import { HttpClientModule } from '@angular/common/http';
import { AngularFontAwesomeModule } from 'angular-font-awesome';

import { AppComponent } from './app.component';
import { PortfolioComponent } from './portfolio/portfolio.component';

import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import { FormsModule } from '@angular/forms';
import { AuthService } from './auth/auth.service';
import { JwtModule, JWT_OPTIONS } from '@auth0/angular-jwt';
import { AppConfig } from './shared/app.config';

export function tokenGetter() {
  return localStorage.getItem('accesstoken');
}
export const whitelistedDomains = [new RegExp('[\s\S]*')] as RegExp[];
export function jwtOptionsFactory() { return { tokenGetter: tokenGetter, whitelistedDomains: whitelistedDomains }; }

export function initializeApp(appConfig: AppConfig): Function {
  return () => appConfig.load();
}

@NgModule({
  declarations: [
    AppComponent,
    PortfolioComponent
  ],
  imports: [
    NgbModule,
    BrowserModule, 
    FormsModule,
    SidebarModule.forRoot(),
    HttpClientModule,
    AngularFontAwesomeModule,
    JwtModule.forRoot({
      jwtOptionsProvider: {
        provide: JWT_OPTIONS,
        useFactory: jwtOptionsFactory
      }
    })
  ],
  providers: [
    AppConfig,
    {
        provide: APP_INITIALIZER,
        useFactory: initializeApp,
        deps: [AppConfig], multi: true
    },
    AuthService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
