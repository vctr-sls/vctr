/** @format */

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import {
  MatSnackBarModule,
  MAT_SNACK_BAR_DEFAULT_OPTIONS,
} from '@angular/material/snack-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { InitializeRouteComponent } from './routes/initialize/initialize.route';
import { APIRestProvider } from './api/api.rest-provider';
import { NoteComponent } from './compontents/note/note.component';
import { ButtonComponent } from './compontents/button/button.component';
import { MainRouteComponent } from './routes/main/main.route';
import { LoginRouteComponent } from './routes/login/login.route';
import { EditRouteComponent } from './routes/edit/edit.route';

import { HeaderComponent } from './compontents/header/header.component';
import { LinkTileComponent } from './compontents/linktile/linktile.component';
import { ErrorRouteComponent } from './routes/error/error.route';
import { ProtectedRouteComponent } from './routes/protected/protected.route';
import { SettingsRouteComponent } from './routes/settings/settings.route';

@NgModule({
  declarations: [
    // COMPONENTS
    AppComponent,
    NoteComponent,
    ButtonComponent,
    HeaderComponent,
    LinkTileComponent,

    // ROUTES
    InitializeRouteComponent,
    MainRouteComponent,
    LoginRouteComponent,
    EditRouteComponent,
    ErrorRouteComponent,
    ProtectedRouteComponent,
    SettingsRouteComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,

    // MATERIAL UI STUFF
    MatSlideToggleModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    MatButtonModule,
  ],
  providers: [
    { provide: 'HttpClient', useClass: HttpClient },
    { provide: 'APIProvider', useClass: APIRestProvider },
    {
      provide: MAT_SNACK_BAR_DEFAULT_OPTIONS,
      useValue: { duration: 10000, panelClass: 'dark-snack-bar' },
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
