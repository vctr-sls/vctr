/** @format */

import { Component, OnInit } from '@angular/core';
import { APIService } from 'src/app/api/api.service';
import { Router } from '@angular/router';
import { ShortLink } from 'src/app/api/api.models';

@Component({
  selector: 'app-route-main',
  templateUrl: './main.route.html',
  styleUrls: ['./main.route.scss'],
})
export class MainRouteComponent implements OnInit {
  public shortLinks: ShortLink[];
  public sortBy = 'creationDate';

  constructor(private api: APIService, private router: Router) {}

  public ngOnInit() {
    this.fetchData();
  }

  public fetchData() {
    this.api.slGet(0, 100, this.sortBy).subscribe((shortLinks) => {
      this.shortLinks = shortLinks;
    });
  }
}
