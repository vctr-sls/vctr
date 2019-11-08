/** @format */

import { Component, Input, OnInit } from '@angular/core';
import { ShortLink } from 'src/app/api/api.models';
import { getFavicon } from '../../util/favicon';
import { Router } from '@angular/router';
import { LinkStatus, getLinkStatus } from 'src/app/util/linkstatus';

@Component({
  selector: 'app-linktile',
  templateUrl: './linktile.component.html',
  styleUrls: ['./linktile.component.scss'],
})
export class LinkTileComponent implements OnInit {
  @Input() public shortLink: ShortLink;
  public favicon: string;

  constructor(private router: Router) {}

  public ngOnInit() {
    this.favicon = getFavicon(this.shortLink.rootURL);
  }

  public onFaviconError() {
    this.favicon = 'assets/no-favicon.svg';
  }

  public onClick(event: any) {
    if (event.shiftKey) {
      return;
    }

    this.router.navigate([this.shortLink.guid, 'edit']);
  }

  public get linkStatus(): LinkStatus {
    return getLinkStatus(this.shortLink);
  }
}
