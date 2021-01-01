/** @format */

import './UserEditor.scss';

import { Component } from 'react';
import { RouteComponentProps, withRouter } from 'react-router';
import { Permissions, UserUpdateModel } from '../../services/api/models';
import APIService from '../../services/api/api';
import SnackBarService from '../../components/snackbar/SnackBarService';
import { ReactComponent as Back } from '../../assets/back.svg';
import PermsSelector from '../../components/perms-selector/PermsSelector';

interface UserEditorProps extends RouteComponentProps {
  id: string;
}

class UserEditor extends Component<UserEditorProps> {
  state = {
    user: (null as any) as UserUpdateModel,
  };

  async componentDidMount() {
    if (this.isNew) {
      const user = {
        permissions: Permissions.CREATE_LINKS,
      } as UserUpdateModel;
      this.setState({ user });
    } else {
      try {
        const user = await APIService.getUser(this.props.id);
        this.setState({ user });
      } catch {}
    }
  }

  render() {
    return !!this.state.user ? this.editorDom : <div>loading...</div>;
  }

  private get editorDom(): JSX.Element {
    const user = this.state.user;

    return (
      <div className="user-editor-container">
        <div className="heading-bar">
          <Back onClick={() => this.props.history.goBack()} />
          <h1>{user.username}</h1>
        </div>
        <div>
          <div className="spacing">
            <label htmlFor="i-username">Username</label>
            <input
              id="i-username"
              className="spacing"
              value={user.username}
              placeholder="i.e. dawgyg"
              onChange={(e) =>
                this.wrapSetState(() => (user.username = e.target.value))
              }
            />
          </div>
        </div>
        {this.isMe && (
          <div>
            <div className="spacing">
              <label htmlFor="i-current-password">Current Password</label>
              <input
                id="i-current-password"
                className="spacing"
                type="password"
                value={user.current_password}
                onChange={(e) =>
                  this.wrapSetState(
                    () => (user.current_password = e.target.value)
                  )
                }
              />
            </div>
          </div>
        )}
        <div>
          <div className="spacing">
            <label htmlFor="i-new-password">
              {!this.isNew && 'New '}Password
            </label>
            <input
              id="i-new-password"
              className="spacing"
              type="password"
              value={user.password}
              onChange={(e) =>
                this.wrapSetState(
                  () => (user.password = user.new_password = e.target.value)
                )
              }
            />
          </div>
          <div className="spacing">
            <PermsSelector
              value={user.permissions}
              onChange={(v) => this.wrapSetState(() => (user.permissions = v))}
            />
          </div>
        </div>
        <button
          disabled={!this.isValid}
          className="save-button"
          onClick={() => this.saveChanges()}
        >
          {this.isNew ? 'Create User' : 'Save Changes'}
        </button>
      </div>
    );
  }

  private get isNew(): boolean {
    return this.props.id === 'new';
  }

  private get isMe(): boolean {
    return this.props.id === 'me';
  }

  private get isValid(): boolean {
    const user = this.state.user;
    return (
      !!user.username &&
      (!this.isMe || !user.new_password || !!user.current_password)
    );
  }

  private wrapSetState(action: () => void) {
    action();
    this.setState({});
  }

  private async saveChanges() {
    try {
      if (this.isNew) {
        await APIService.createUser(this.state.user);
        this.props.history.goBack();
        SnackBarService.show('Link successfully created.');
      } else {
        if (this.isMe) this.state.user.guid = 'me';
        const link = await APIService.updateUser(this.state.user);
        this.setState({ link });
        SnackBarService.show('Link successfully updated.');
      }
    } catch {}
  }
}

export default withRouter(UserEditor);
