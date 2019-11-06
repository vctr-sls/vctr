/** @format */

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { InitializeRouteComponent } from './routes/initialize/initialize.route';
import { APIRestProvider } from './api/api.rest-provider';

@NgModule({
  declarations: [
    AppComponent,

    // ROUTES
    InitializeRouteComponent,
  ],
  imports: [BrowserModule, AppRoutingModule, HttpClientModule, FormsModule],
  providers: [{ provide: 'APIProvider', useClass: APIRestProvider }],
  bootstrap: [AppComponent],
})
export class AppModule {}
