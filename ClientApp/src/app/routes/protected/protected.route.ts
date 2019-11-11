/** @format */

import { Component } from '@angular/core';
import { APIService } from 'src/app/api/api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { ProtectedLogin } from 'src/app/api/api.models';

@Component({
  selector: 'app-route-protected',
  templateUrl: './protected.route.html',
  styleUrls: ['./protected.route.scss'],
})
export class ProtectedRouteComponent {
  public guid: string;
  public disableTracking: boolean;

  public password: string;
  public error = {
    visible: false,
    text: '',
  };
  public redirecting: boolean;

  constructor(
    private api: APIService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.route.queryParams.subscribe((params) => {
      if (!params.guid) {
        router.navigate(['/']);
      }

      this.guid = params.guid;
      this.disableTracking = params.disableTracking;
    });
  }

  public get isValidEntry(): boolean {
    return !!this.password;
  }

  public onLogin() {
    if (this.isValidEntry) {
      this.redirecting = true;

      const payload = {
        password: this.password,
        disableTracking: this.disableTracking,
      } as ProtectedLogin;

      this.api
        .protectedRedirect(this.guid, payload)
        .then((res) => {
          window.location.assign(res.rootURL);
        })
        .catch((err) => {
          this.redirecting = false;
          console.error(err);
          this.error.visible = true;
          if (err && err.status === 401) {
            this.error.text = 'Invalid password.';
          } else {
            // TODO: Better error displaying depending on error format.
            this.error.text = err.toString();
          }
        });
      this.password = '';
    }
  }

  public onKeyPress(event: any) {
    if (event.keyCode === 13) {
      event.preventDefault();
      this.onLogin();
    }
  }
}
