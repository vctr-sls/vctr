/** @format */

import { Component, OnInit } from '@angular/core';
import { APIService } from 'src/app/api/api.service';
import { Router } from '@angular/router';
import { ShortLink } from 'src/app/api/api.models';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-route-main',
  templateUrl: './main.route.html',
  styleUrls: ['./main.route.scss'],
})
export class MainRouteComponent implements OnInit {
  public shortLinks: ShortLink[];
  public sortBy = 'creationDate';
  public lastSearchInput: string;

  constructor(private api: APIService, private router: Router) {}

  public ngOnInit() {
    this.fetchData();
  }

  public fetchData(query?: string) {
    let method: Observable<ShortLink[]>;

    if (query) {
      method = this.api.slSearch(query, 0, 100, this.sortBy);
    } else {
      method = this.api.slGet(0, 100, this.sortBy);
    }

    method.subscribe((shortLinks) => {
      this.shortLinks = shortLinks;
    });
  }

  public onSearchInput(input: string) {
    this.lastSearchInput = input;
    setTimeout(() => {
      if (input === this.lastSearchInput) {
        this.fetchData(input);
      }
    }, 250);
  }
}
