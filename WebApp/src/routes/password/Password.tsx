/** @format */

import './Password.scss';

import { Component } from 'react';
import { ReactComponent as LogoLarge } from '../../assets/logo-large.svg';

export default class Password extends Component {
  state = {
    linkIdent: null,
    password: '',
  };

  componentDidMount() {
    const params = new URLSearchParams(window.location.search);
    this.setState({ linkIdent: params.get('ident') });
  }

  render() {
    return (
      <div className="password-container">
        {(this.state.linkIdent && (
          <div>
            <LogoLarge width="200" />
            <p>This link requires a password to be accessed.</p>
            <input
              style={{ marginTop: '40px' }}
              placeholder="Password..."
              type="password"
              value={this.state.password}
              onChange={(e) => this.setState({ password: e.target.value })}
              onKeyPress={(e) => this.proceedKeyPress(e)}
            />
            <button
              style={{ marginTop: '30px' }}
              disabled={!this.state.password}
              onClick={() => this.proceed()}
            >
              PROCEED
            </button>
          </div>
        )) || <p>No ident provided.</p>}
      </div>
    );
  }

  private proceed() {
    window.location.assign(
      `/${this.state.linkIdent}?password=${this.state.password}`
    );
  }

  private async proceedKeyPress(e: React.KeyboardEvent<HTMLInputElement>) {
    if (e.key !== 'Enter') return;
    this.proceed();
  }
}
