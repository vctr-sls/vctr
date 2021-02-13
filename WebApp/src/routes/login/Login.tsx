/** @format */

import './Login.scss';

import { Component } from 'react';
import { ReactComponent as LogoLarge } from '../../assets/logo-large.svg';
import APIService from '../../services/api/api';
import StateService from '../../services/state/state';
import { RouteComponentProps, withRouter } from 'react-router';
import Switch from '../../components/switch/Switch';
import SnackBarService, {
  SnackBarType,
} from '../../components/snackbar/SnackBarService';

interface LoginProps extends RouteComponentProps {
  state: StateService;
}

class Login extends Component<LoginProps> {
  state = {
    username: '',
    password: '',
    remember: false,
  };

  render() {
    return (
      <div className="login-container">
        <div>
          <LogoLarge width="200" />
          <input
            style={{ marginTop: '40px' }}
            placeholder="Username..."
            type="text"
            value={this.state.username}
            onChange={(e) => this.setState({ username: e.target.value })}
            onKeyPress={(e) => this.loginKeyPress(e)}
          />
          <input
            style={{ marginTop: '20px' }}
            placeholder="Password..."
            type="password"
            value={this.state.password}
            onChange={(e) => this.setState({ password: e.target.value })}
            onKeyPress={(e) => this.loginKeyPress(e)}
          />
          <div style={{ marginTop: '30px' }}>
            <Switch
              enabled={this.state.remember}
              onChange={(v) => this.setState({ remember: v })}
              size={30}
            >
              Remember for 30 days
            </Switch>
          </div>
          <button
            disabled={!this.passwordEnabled}
            style={{ marginTop: '30px' }}
            onClick={() => this.login()}
          >
            LOGIN
          </button>
        </div>
      </div>
    );
  }

  private get passwordEnabled(): boolean {
    return !!this.state.username && !!this.state.password;
  }

  private async login(remember: boolean = false) {
    if (!this.passwordEnabled) return;
    try {
      const me = await APIService.authLogin({
        ident: this.state.username,
        password: this.state.password,
        remember: this.state.remember || remember,
      });
      this.props.state.selfUser = me;
      this.props.history.replace('/links');
    } catch (err) {
      console.error(err);
      SnackBarService.show('Invalid username or password.', SnackBarType.ERROR);
    }
  }

  private async loginKeyPress(e: React.KeyboardEvent<HTMLInputElement>) {
    if (e.key !== 'Enter') return;
    await this.login(e.ctrlKey);
  }
}

export default withRouter(Login);
