/** @format */

import { Component } from '@angular/core';
import { APIService } from 'src/app/api/api.service';
import { Router } from '@angular/router';
import { ShortLink } from 'src/app/api/api.models';

@Component({
  selector: 'app-route-main',
  templateUrl: './main.route.html',
  styleUrls: ['./main.route.scss'],
})
export class MainRouteComponent {
  public shortLinks: ShortLink[];

  constructor(private api: APIService, private router: Router) {
    this.api.slGet(0, 100).subscribe((shortLinks) => {
      this.shortLinks = shortLinks;
    });
  }
}
