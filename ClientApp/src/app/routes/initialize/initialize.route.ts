/** @format */

import { Component } from '@angular/core';
import { APIService } from 'src/app/api/api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-route-initialize',
  templateUrl: './initialize.route.html',
  styleUrls: ['./initialize.route.scss'],
})
export class InitializeRouteComponent {
  public password: string;
  public passwordRepeated: string;

  constructor(private api: APIService, private router: Router) {}

  public get isValidEntry(): boolean {
    return (
      this.password &&
      this.passwordRepeated &&
      this.password === this.passwordRepeated
    );
  }

  public onSet() {
    if (this.isValidEntry) {
      this.api
        .settingsInit(this.password)
        .then(() => {
          this.router.navigate(['/login']);
        })
        .catch((err) => {
          console.error(err);
        });
    }
  }
}
