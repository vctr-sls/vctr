/** @format */

import { IAPIProvider } from './api.provider';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import {
  GeneralSettings,
  SetPasswordPost,
  GeneralSettingsPost,
  ShortLink,
  ProtectedLogin,
  ProtectedResponse,
  Size,
} from './api.models';
import { Inject, EventEmitter } from '@angular/core';

export class APIRestProvider implements IAPIProvider {
  public authorizationError = new EventEmitter<any>();
  public error = new EventEmitter<any>();

  private readonly errorCatcher = (err) => {
    if (err && err.status === 401) {
      this.authorizationError.emit();
    } else {
      this.error.emit(err);
    }

    return throwError(err);
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
      .post<GeneralSettings>('/api/settings', settings, this.defopts())
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

  public slGet(
    page: number,
    size: number,
    sortBy: string
  ): Observable<ShortLink[]> {
    const opts = this.defopts({
      params: new HttpParams()
        .set('page', page.toString())
        .set('size', size.toString())
        .set('sortBy', sortBy),
    });

    return this.http
      .get<ShortLink[]>('/api/shortlinks', opts)
      .pipe(catchError(this.errorCatcher));
  }

  public slGetSize(): Observable<Size> {
    return this.http
      .get<Size>('/api/shortlinks/size', this.defopts())
      .pipe(catchError(this.errorCatcher));
  }

  public slSearch(
    query: string,
    page: number,
    size: number,
    sortBy: string
  ): Observable<ShortLink[]> {
    const opts = this.defopts({
      params: new HttpParams()
        .set('page', page.toString())
        .set('size', size.toString())
        .set('sortBy', sortBy)
        .set('query', query),
    });

    return this.http
      .get<ShortLink[]>('/api/shortlinks/search', opts)
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
      .put<ShortLink>(
        `/api/shortLinks/${shortLink.guid}`,
        shortLink,
        this.defopts()
      )
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

  public protectedRedirect(
    guid: string,
    login: ProtectedLogin
  ): Promise<ProtectedResponse> {
    return this.http
      .post<ProtectedResponse>(
        `/protectedredirect/${guid}`,
        login,
        this.defopts()
      )
      .toPromise();
  }
}
