/** @format */

import './NotFound.scss';

import { Component } from 'react';
import { ReactComponent as LogoLarge } from '../../assets/logo-large.svg';

export default class NotFound extends Component {
  render() {
    return (
      <div className="notfound-container">
        <LogoLarge height="50" width="100" />
        <p className="heading">
          This link does not exist or is no longer accessable.
        </p>
      </div>
    );
  }
}
