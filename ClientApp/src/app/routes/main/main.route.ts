/** @format */

import { Component, OnInit } from '@angular/core';
import { APIService } from 'src/app/api/api.service';
import { Router } from '@angular/router';
import { ShortLink } from 'src/app/api/api.models';
import { Observable } from 'rxjs';

const MAX_PAGE_SIZE = 50;

@Component({
  selector: 'app-route-main',
  templateUrl: './main.route.html',
  styleUrls: ['./main.route.scss'],
})
export class MainRouteComponent implements OnInit {
  public shortLinks: ShortLink[] = [];
  public totalSize: number;
  public sortBy = 'creationDate';
  public lastSearchInput: string;
  public isLoading = true;

  constructor(private api: APIService, private router: Router) {}

  public ngOnInit() {
    this.fetchData();

    window.onscroll = this.onScroll.bind(this);
  }

  public fetchData(query?: string) {
    let method: Observable<ShortLink[]>;

    const currSize = this.shortLinks.length;
    const page = currSize / MAX_PAGE_SIZE;

    if (this.totalSize && currSize >= this.totalSize) {
      this.isLoading = false;
      return;
    }

    this.api.slGetSize().subscribe((size) => {
      this.totalSize = size.size;
    });

    if (query) {
      method = this.api.slSearch(query, page, MAX_PAGE_SIZE, this.sortBy);
    } else {
      method = this.api.slGet(page, MAX_PAGE_SIZE, this.sortBy);
    }

    method.subscribe((shortLinks) => {
      this.shortLinks = this.shortLinks.concat(shortLinks);
      this.onScroll();
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

  private onScroll() {
    if (window.innerHeight + window.scrollY >= document.body.scrollHeight) {
      this.fetchData();
    }
  }
}
