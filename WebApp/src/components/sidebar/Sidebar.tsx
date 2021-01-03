/** @format */

import './Sidebar.scss';

import { Component } from 'react';
import { ReactComponent as LogoLarge } from '../../assets/logo-large.svg';

interface SidebarProps {
  onActivate: (r: string) => void;
  entries: SidebarEntry[];
}

export interface SidebarEntry {
  displayname: string;
  route: string;
}

export default class Sidebar extends Component<SidebarProps> {
  render() {
    const sidebarTiles = this.props.entries.map((e) => this.sidebarTile(e));
    sidebarTiles.push(
      <div
        key={`sbtile-logout`}
        className="sidebar-tile logout"
        onClick={() => this.props.onActivate.call(this, 'logout')}
      >
        logout
      </div>
    );

    return (
      <div className="sidebar-container">
        <div className="flex">
          <LogoLarge className="sidebar-logo" />
        </div>
        {sidebarTiles}
      </div>
    );
  }

  private sidebarTile(e: SidebarEntry): JSX.Element {
    const isActive = window.location.pathname
      .replace(process.env.PUBLIC_URL, '')
      .substr(1)
      .startsWith(e.route);
    return (
      <div
        key={`sbtile-${e.route}`}
        className={`sidebar-tile ${isActive ? 'active' : ''}`}
        onClick={() => this.props.onActivate.call(this, e.route)}
      >
        {e.displayname}
      </div>
    );
  }
}
