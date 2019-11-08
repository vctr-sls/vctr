/** @format */

import { Component, Output, EventEmitter, Input } from '@angular/core';
import { Router } from '@angular/router';
import { APIService } from 'src/app/api/api.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent {
  @Input() public displaySearch = true;

  @Output() public searchInput = new EventEmitter<string>();

  public searchValue: string;

  constructor(private api: APIService, private router: Router) {}

  public onLogout() {
    this.api
      .authLogout()
      .then(() => {
        this.router.navigate(['/login']);
      })
      .catch((err) => {
        console.error(err);
      });
  }

  public onSearchInput() {
    this.searchInput.emit(this.searchValue);
  }
}
