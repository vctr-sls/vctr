/** @format */

import './Switch.scss';

import { Component } from 'react';

interface SwitchProps {
  size: number;
  enabled: boolean;
  onChange: (state: boolean) => void;
}

export default class Switch extends Component<SwitchProps> {
  static defaultProps = {
    size: 35,
    enabled: false,
    onChange: (state: boolean) => {},
  };

  state = {
    enabled: this.props.enabled,
  };

  render() {
    return (
      <div className="switch-container" onClick={() => this.onClick()}>
        <div
          className="switch-switcher"
          style={{ height: this.props.size, width: this.props.size * 2 }}
        >
          <div
            className={`switch-tile ${this.state.enabled ? 'enabled' : ''}`}
          ></div>
        </div>
        <div>{this.props.children}</div>
      </div>
    );
  }

  private onClick() {
    this.setState({ enabled: !this.state.enabled }, () =>
      this.props.onChange.call(this, this.state.enabled)
    );
  }
}
