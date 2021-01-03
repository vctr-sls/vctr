/** @format */

import './LinkTile.scss';

import { Component } from 'react';
import { LinkModel } from '../../services/api/models';
import { ReactComponent as Arrow } from '../../assets/arrow.svg';
import { ReactComponent as Lock } from '../../assets/lock.svg';
import { ReactComponent as Clock } from '../../assets/clock.svg';
import { ReactComponent as Disabled } from '../../assets/disabled.svg';
import { ReactComponent as EyeOutlined } from '../../assets/eye-outlined.svg';
import { ReactComponent as EyeFilled } from '../../assets/eye-filled.svg';
import { ReactComponent as Delete } from '../../assets/delete.svg';
import { ReactComponent as Copy } from '../../assets/copy.svg';
import UrlUtils from '../../util/urls';
import TimeUtil from '../../util/time';
import Copy2ClipboardService from '../copy2cb/Copy2ClipboardService';

interface LinkTileProps {
  link: LinkModel;
  onDelete: (link: LinkModel) => void;
  onClick: (link: LinkModel) => void;
}

export default class LinkTile extends Component<LinkTileProps> {
  static defaultProps = {
    onDelete: (_: LinkModel) => {},
    onClick: (_: LinkModel) => {},
  };

  render() {
    const link = this.props.link;

    return (
      <div
        className="link-tile-container"
        onClick={() => this.props.onClick.call(this, link)}
      >
        <div className="details-container">
          <div className="title-bar">
            <span className="ident">{link.ident}</span>
            <Arrow />
            <span>{UrlUtils.formatDestination(link.destination)}</span>
          </div>
          <div className="properties-bar">
            {link.enabled || <Disabled />}
            {link.password_required && <Lock />}
            {new Date(link.expires) > TimeUtil.nullDate && <Clock />}
          </div>
          <div className="stats-bar">
            <EyeOutlined /> <span>{link.access_count}</span>
            <EyeFilled /> <span>{link.unique_access_count}</span>
          </div>
        </div>
        <div className="controls-container">
          <button onClick={(e) => this.onCopyClick(e)} className="button-copy">
            <Copy />
          </button>
          <button onClick={(e) => this.onDeleteClick(e)}>
            <Delete />
          </button>
        </div>
      </div>
    );
  }

  private onDeleteClick(e: React.MouseEvent<HTMLButtonElement, MouseEvent>) {
    e.stopPropagation();
    this.props.onDelete.call(this, this.props.link);
  }

  private onCopyClick(e: React.MouseEvent<HTMLButtonElement, MouseEvent>) {
    e.stopPropagation();
    const shortLink = `${window.location.origin}/${this.props.link.ident}`;
    Copy2ClipboardService.copy(shortLink);
  }
}
