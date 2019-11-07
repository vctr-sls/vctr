/** @format */

import { Component } from '@angular/core';
import { APIService } from 'src/app/api/api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-route-login',
  templateUrl: './login.route.html',
  styleUrls: ['./login.route.scss'],
})
export class LoginRouteComponent {
  public password: string;
  public error = {
    visible: false,
    text: '',
  };

  constructor(private api: APIService, private router: Router) {}

  public get isValidEntry(): boolean {
    return !!this.password;
  }

  public onLogin() {
    if (this.isValidEntry) {
      this.api
        .authLogin(this.password)
        .then(() => {
          this.router.navigate(['/']);
        })
        .catch((err) => {
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
