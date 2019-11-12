/** @format */

import {
  Component,
  Input,
  OnInit,
  TemplateRef,
  ViewChild,
} from '@angular/core';
import { ShortLink } from 'src/app/api/api.models';
import { getFavicon } from '../../util/favicon';
import { Router } from '@angular/router';
import { LinkStatus, getLinkStatus } from 'src/app/util/linkstatus';

import dateFormat from 'dateformat';
import * as copyToClipboard from 'copy-to-clipboard';
import { getShortLinkURL } from 'src/app/util/shortlinks';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-linktile',
  templateUrl: './linktile.component.html',
  styleUrls: ['./linktile.component.scss'],
})
export class LinkTileComponent implements OnInit {
  @Input() public shortLink: ShortLink;

  public favicon: string;
  public dateFormat = dateFormat;

  constructor(private router: Router, private snackBar: MatSnackBar) {}

  public ngOnInit() {
    this.favicon = getFavicon(this.shortLink.rootURL);
  }

  public onFaviconError() {
    this.favicon = 'assets/no-favicon.svg';
  }

  public onClick(event: any) {
    if (event.shiftKey) {
      const url = getShortLinkURL(this.shortLink);
      if (copyToClipboard(url)) {
        this.snackBar.open('Copied Short Link to Clip Board!', 'Ok', {
          panelClass: 'dark-snack-bar',
        });
      } else {
        this.snackBar.open('Copied Short Link to Clip Board!', 'Ok', {
          panelClass: ['dark-snack-bar', 'error'],
        });
      }
      return;
    }

    this.router.navigate([this.shortLink.guid, 'edit']);
  }

  public onRootLinkClick(event: Event) {
    event.stopPropagation();
  }

  public get linkStatus(): LinkStatus {
    return getLinkStatus(this.shortLink);
  }
}
