/** @format */

import { Injectable, Inject, EventEmitter } from '@angular/core';
import { IAPIProvider } from './api.provider';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import {
  GeneralSettings,
  ShortLink,
  SetPasswordPost,
  GeneralSettingsPost,
  ProtectedLogin,
  ProtectedResponse,
} from './api.models';
import { MatSnackBar } from '@angular/material/snack-bar';

/** @format */

@Injectable({
  providedIn: 'root',
})
export class APIService implements IAPIProvider {
  public error: EventEmitter<any>;
  public authorizationError: EventEmitter<any>;

  constructor(
    @Inject('APIProvider') private provider: IAPIProvider,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.authorizationError = provider.authorizationError;
    this.error = this.provider.error;

    this.authorizationError.subscribe(() => {
      this.router.navigate(['/login']);
    });

    this.error.subscribe((err) => {
      let msg: string;

      if (!err) {
        msg = 'An unexpected error occured.';
      } else if (err.error && err.error.message) {
        msg = err.error.message;
      } else if (err.status && err.statusText) {
        msg = `${err.status}: ${err.statusText}`;
      }

      this.snackBar.open(msg, 'Ok', {
        panelClass: ['dark-snack-bar', 'error'],
      });
    });
  }

  public authLogin(password: string): Promise<any> {
    return this.provider.authLogin(password);
  }

  public authLogout(): Promise<any> {
    return this.provider.authLogout();
  }

  public settingsGet(): Observable<GeneralSettings> {
    return this.provider.settingsGet();
  }

  public settingsSet(settings: GeneralSettingsPost): Promise<GeneralSettings> {
    return this.provider.settingsSet(settings);
  }

  public settingsInit(password: string): Promise<any> {
    return this.provider.settingsInit(password);
  }

  public slGet(
    page: number,
    size: number,
    sortBy: string
  ): Observable<ShortLink[]> {
    return this.provider.slGet(page, size, sortBy);
  }

  public slGetSingle(guid: string): Observable<ShortLink> {
    return this.provider.slGetSingle(guid);
  }

  public slCreate(shortLink: ShortLink): Promise<ShortLink> {
    return this.provider.slCreate(shortLink);
  }

  public slEdit(shortLink: ShortLink): Promise<ShortLink> {
    return this.provider.slEdit(shortLink);
  }

  public slDelete(guid: string): Promise<any> {
    return this.provider.slDelete(guid);
  }

  public slSetPassword(guid: string, password: SetPasswordPost): Promise<any> {
    return this.provider.slSetPassword(guid, password);
  }

  public protectedRedirect(
    guid: string,
    login: ProtectedLogin
  ): Promise<ProtectedResponse> {
    return this.provider.protectedRedirect(guid, login);
  }
}
