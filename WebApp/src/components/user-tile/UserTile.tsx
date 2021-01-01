/** @format */

import './UserTile.scss';

import { Component } from 'react';
import { UserViewModel } from '../../services/api/models';
import { ReactComponent as Delete } from '../../assets/delete.svg';
import PermissionsUtil from '../../util/permissions';
import TileSkeleton from '../tile-skeleton/TileSkeleton';

interface UserTileProps {
  user: UserViewModel;
  onDelete: (link: UserViewModel) => void;
  onClick: (link: UserViewModel) => void;
}

export default class UserTile extends Component<UserTileProps> {
  static defaultProps = {
    onDelete: (_: UserViewModel) => {},
    onClick: (_: UserViewModel) => {},
  };

  render() {
    const user = this.props.user;

    return (
      <div
        className="users-tile-container"
        onClick={() => this.props.onClick.call(this, user)}
      >
        <div className="details-container">
          <span className="username">{user.username}</span>
          {(user.link_count === undefined && (
            <TileSkeleton height="19px" width="100px" />
          )) || <span>{user.link_count} Links</span>}
          <span>{PermissionsUtil.getNames(user.permissions).join(', ')}</span>
        </div>
        <div className="controls-container">
          <button onClick={(e) => this.onDeleteClick(e)}>
            <Delete />
          </button>
        </div>
      </div>
    );
  }

  private onDeleteClick(e: React.MouseEvent<HTMLButtonElement, MouseEvent>) {
    e.stopPropagation();
    this.props.onDelete.call(this, this.props.user);
  }
}
