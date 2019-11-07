/** @format */

import { IAPIProvider } from './api.provider';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { of, Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

import {
  GeneralSettings,
  SetPasswordPost,
  GeneralSettingsPost,
  ShortLink,
} from './api.models';
import { Inject, EventEmitter } from '@angular/core';

export class APIRestProvider implements IAPIProvider {
  public authorizationError = new EventEmitter<any>();

  private readonly errorCatcher = (err) => {
    console.error(err);

    if (err && err.status === 401) {
      this.authorizationError.emit();
    }

    return of(null);
  };

  private readonly defopts = (obj?: object) => {
    const defopts = {
      withCredentials: true,
    };

    if (obj) {
      Object.keys(obj).forEach((k) => {
        defopts[k] = obj[k];
      });
    }

    return defopts;
  };

  constructor(@Inject('HttpClient') private http: HttpClient) {
    console.log(this.http.post);
  }

  public authLogin(password: string): Promise<any> {
    const opts = this.defopts({
      headers: new HttpHeaders({
        Authorization: `Basic ${password}`,
      }),
    });
    return this.http.post('/api/authorization/login', null, opts).toPromise();
  }

  public authLogout(): Promise<any> {
    return this.http
      .post('/api/authorization/logout', null, this.defopts())
      .pipe(catchError(this.errorCatcher))
      .toPromise();
  }

  public settingsGet(): Observable<GeneralSettings> {
    return this.http
      .get<GeneralSettingsPost>('/api/settings', this.defopts())
      .pipe(catchError(this.errorCatcher));
  }

  public settingsSet(settings: GeneralSettingsPost): Promise<GeneralSettings> {
    return this.http
      .post('/api/settings', settings, this.defopts())
      .pipe(catchError(this.errorCatcher))
      .toPromise();
  }

  public settingsInit(password: string): Promise<any> {
    return this.http
      .post(
        '/api/settings/initialize',
        { password } as GeneralSettingsPost,
        this.defopts()
      )
      .pipe(catchError(this.errorCatcher))
      .toPromise();
  }

  public slGet(page: number, size: number): Observable<ShortLink[]> {
    return this.http
      .get<ShortLink[]>('/api/shortlinks', this.defopts())
      .pipe(catchError(this.errorCatcher));
  }

  public slGetSingle(guid: string): Observable<ShortLink> {
    return this.http
      .get<ShortLink>(`/api/shortlinks/${guid}`, this.defopts())
      .pipe(catchError(this.errorCatcher));
  }

  public slCreate(shortLink: ShortLink): Promise<ShortLink> {
    return this.http
      .post<ShortLink>('/api/shortlinks', shortLink, this.defopts())
      .pipe(catchError(this.errorCatcher))
      .toPromise();
  }

  public slEdit(shortLink: ShortLink): Promise<ShortLink> {
    return this.http
      .put(`/api/shortLinks/${shortLink.guid}`, shortLink, this.defopts())
      .pipe(catchError(this.errorCatcher))
      .toPromise();
  }

  public slDelete(guid: string): Promise<any> {
    return this.http
      .delete(`/api/shortLinks/${guid}`, this.defopts())
      .pipe(catchError(this.errorCatcher))
      .toPromise();
  }

  public slSetPassword(guid: string, password: SetPasswordPost): Promise<any> {
    return this.http
      .post(`/api/shortLinks/${guid}/password`, password, this.defopts())
      .pipe(catchError(this.errorCatcher))
      .toPromise();
  }
}
