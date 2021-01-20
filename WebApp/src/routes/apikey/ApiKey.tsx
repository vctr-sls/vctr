/** @format */

import './ApiKey.scss';

import { Component } from 'react';
import { RouteComponentProps, withRouter } from 'react-router';
import { ApiKeyCreateModel } from '../../services/api/models';
import APIService from '../../services/api/api';
import Copy2ClipboardService from '../../components/copy2cb/Copy2ClipboardService';
import SnackBarService, {
  SnackBarType,
} from '../../components/snackbar/SnackBarService';
import dateformat from 'dateformat';
import { NULL_DATE } from '../../util/time';

interface ApiKeyProps extends RouteComponentProps {
  id: string;
}

class ApiKey extends Component<ApiKeyProps> {
  state = {
    apiKey: (null as any) as ApiKeyCreateModel,
    show: false,
  };

  async componentDidMount() {
    try {
      const apiKey = await APIService.getApiKey(false);
      this.setState({ apiKey });
    } catch (err) {
      if (err?.message !== 'Not Found') {
        APIService.emitError(err);
      }
    }
  }

  render() {
    return !this.state.apiKey
      ? this.notGeneratedView
      : !this.state.apiKey.key
      ? this.infoView
      : this.generatedView;
  }

  private get notGeneratedView(): JSX.Element {
    return (
      <div>
        <div>
          <p>No API key generated.</p>
        </div>
        <div>
          <button onClick={() => this.generateApiKey()}>
            Generate API Key
          </button>
        </div>
      </div>
    );
  }

  private get generatedView(): JSX.Element {
    return (
      <div>
        <div>
          <div className="apikey-warning">
            <span>
              The token is only shown after re-initialization{' '}
              <strong>once</strong>. As soon as you reload this page, the token
              will not be shown again!
            </span>
          </div>
          {this.state.show && (
            <div className="apikey-key-view">
              <span>{this.state.apiKey.key}</span>
            </div>
          )}
          <div className="apikey-control">
            <button className="apikey-secbtn" onClick={() => this.copyApiKey()}>
              Copy API Key
            </button>
            <button
              className="apikey-secbtn"
              onClick={() => this.setState({ show: !this.state.show })}
            >
              {this.state.show ? 'Hide API Key' : 'Show API Key'}
            </button>
            <button onClick={() => this.generateApiKey()}>
              Regenerate API Key
            </button>
            <button onClick={() => this.resetApiKey()}>Reset API Key</button>
          </div>
        </div>
      </div>
    );
  }

  private get infoView(): JSX.Element {
    const key = this.state.apiKey;
    return (
      <div>
        <div>
          <table className="apikey-infotable">
            <tbody>
              <tr>
                <th>Created</th>
                <td>{dateformat(key.created)}</td>
              </tr>
              <tr>
                <th>Last Access</th>
                <td>
                  {key.last_access === NULL_DATE
                    ? 'Never'
                    : dateformat(key.last_access)}
                </td>
              </tr>
              <tr>
                <th>Access Count</th>
                <td>{key.access_count}</td>
              </tr>
            </tbody>
          </table>
        </div>
        <div className="apikey-control">
          <button onClick={() => this.generateApiKey()}>
            Regenerate API Key
          </button>
          <button onClick={() => this.resetApiKey()}>Reset API Key</button>
        </div>
      </div>
    );
  }

  private async generateApiKey() {
    try {
      const apiKey = await APIService.createApiKey();
      this.setState({ apiKey });
      SnackBarService.show('API key generated.', SnackBarType.DEFAULT);
    } catch {}
  }

  private async resetApiKey() {
    try {
      await APIService.resetApiKey();
      this.setState({ apiKey: null });
      SnackBarService.show('API key reset.', SnackBarType.DEFAULT);
    } catch {}
  }

  private copyApiKey() {
    Copy2ClipboardService.copy(this.state.apiKey?.key);
    SnackBarService.show('API key copied to clipboard.', SnackBarType.DEFAULT);
  }
}

export default withRouter(ApiKey);
