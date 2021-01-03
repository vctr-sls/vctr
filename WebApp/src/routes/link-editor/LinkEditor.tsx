/** @format */

import './LinkEditor.scss';

import { Component } from 'react';
import { RouteComponentProps, withRouter } from 'react-router';
import { LinkCreateModel } from '../../services/api/models';
import APIService from '../../services/api/api';
import Switch from '../../components/switch/Switch';
import TimeUtil from '../../util/time';
import { ReactComponent as Back } from '../../assets/back.svg';
import RandomUtil from '../../util/random';
import SnackBarService from '../../components/snackbar/SnackBarService';

interface LinkEditorProps extends RouteComponentProps {
  id: string;
}

class LinkEditor extends Component<LinkEditorProps> {
  state = {
    link: (null as any) as LinkCreateModel,
  };

  async componentDidMount() {
    if (this.isNew) {
      const link = {
        ident: RandomUtil.getString(6),
        enabled: true,
        permanent_redirect: true,
        total_access_limit: 0,
      } as LinkCreateModel;
      this.setState({ link });
    } else {
      try {
        const link = await APIService.getLink(this.props.id);
        this.setState({ link });
      } catch {}
    }
  }

  render() {
    return !!this.state.link ? this.editorDom : <div>loading...</div>;
  }

  private get editorDom(): JSX.Element {
    const link = this.state.link;
    return (
      <div className="link-editor-container">
        <div className="heading-bar">
          <Back onClick={() => this.props.history.goBack()} />
          <h1>{link.ident}</h1>
        </div>

        {!this.isNew && (
          <div className="links-stats">
            <div>
              <span>Total Accesses</span>
              <span>{link.access_count}</span>
            </div>
            <div>
              <span>Unique Accesses</span>
              <span>{link.unique_access_count}</span>
            </div>
          </div>
        )}

        <div>
          <div className="spacing">
            <Switch
              enabled={link.enabled}
              onChange={(v) => this.wrapSetState(() => (link.enabled = v))}
            >
              <label>Enabled</label>
            </Switch>
          </div>

          <div className="spacing">
            <Switch
              enabled={link.permanent_redirect}
              onChange={(v) =>
                this.wrapSetState(() => (link.permanent_redirect = v))
              }
            >
              <label>Permanent Redirect</label>
            </Switch>
          </div>

          <label htmlFor="i-ident">Ident</label>
          <input
            id="i-ident"
            className="spacing"
            value={link.ident}
            placeholder="i.e. mycoollink"
            onChange={(e) =>
              this.wrapSetState(() => (link.ident = e.target.value))
            }
          />

          <label htmlFor="i-destination">Destination</label>
          <input
            id="i-destination"
            className="spacing"
            value={link.destination}
            placeholder="i.e. https://zekro.de"
            onChange={(e) =>
              this.wrapSetState(() => (link.destination = e.target.value))
            }
          />

          <label htmlFor="i-password">
            Set {link.password_required ? 'new' : ''} Password
            <br />
            <i>
              {link.password_required
                ? 'This link requires a password. Enter a new one to overwrite the current password.'
                : 'This link is currently not password protected. Enter a password to do so.'}
            </i>
          </label>
          <input
            id="i-password"
            className="spacing"
            value={link.password}
            placeholder="i.e. 4_5TR0NG_P455w0rd_!"
            onChange={(e) =>
              this.wrapSetState(() => (link.password = e.target.value))
            }
          />

          <label htmlFor="i-accesslimit">
            Unique Access Limit
            <br />
            <i>
              <code>0</code> means that no access limit is set.
            </i>
          </label>
          <input
            id="i-accesslimit"
            className="spacing"
            type="number"
            min="0"
            value={link.total_access_limit}
            onChange={(e) =>
              this.wrapSetState(
                () => (link.total_access_limit = parseInt(e.target.value))
              )
            }
          />

          <label htmlFor="i-expires">Expires</label>
          <input
            id="i-expires"
            className="spacing"
            type="datetime-local"
            value={
              new Date(link.expires) <= TimeUtil.nullDate ? '' : link.expires
            }
            onChange={(e) =>
              this.wrapSetState(() => (link.expires = e.target.value))
            }
          />

          <button
            disabled={!this.isValid}
            className="save-button"
            onClick={() => this.saveChanges()}
          >
            {this.isNew ? 'Create Link' : 'Save Changes'}
          </button>
        </div>
      </div>
    );
  }

  private get isNew(): boolean {
    return this.props.id === 'new';
  }

  private get isValid(): boolean {
    const link = this.state.link;
    return !!link.ident && !!link.destination;
  }

  private wrapSetState(action: () => void) {
    action();
    this.setState({});
  }

  private async saveChanges() {
    try {
      if (this.isNew) {
        await APIService.createLink(this.state.link);
        this.props.history.goBack();
        SnackBarService.show('Link successfully created.');
      } else {
        const link = await APIService.updateLink(this.state.link);
        this.setState({ link });
        SnackBarService.show('Link successfully updated.');
      }
    } catch {}
  }
}

export default withRouter(LinkEditor);
