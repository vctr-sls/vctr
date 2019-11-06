/** @format */

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClient } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { InitializeRouteComponent } from './routes/initialize/initialize.route';
import { APIRestProvider } from './api/api.rest-provider';
import { NoteComponent } from './compontents/note/note.component';
import { ButtonComponent } from './compontents/button/button.component';

@NgModule({
  declarations: [
    AppComponent,
    NoteComponent,
    ButtonComponent,

    // ROUTES
    InitializeRouteComponent,
  ],
  imports: [BrowserModule, AppRoutingModule, HttpClientModule, FormsModule],
  providers: [
    { provide: 'HttpClient', useClass: HttpClient },
    { provide: 'APIProvider', useClass: APIRestProvider },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
