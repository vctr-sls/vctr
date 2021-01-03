/** @format */

import './Copy2Clipboard.scss';

import { Component } from 'react';
import Copy2ClipboardService from './Copy2ClipboardService';

export default class Copy2Clipboard extends Component {
  state = {
    text: '',
  };

  private inputElement: HTMLInputElement | null = null;

  componentDidMount() {
    Copy2ClipboardService.events.on('copy', this.onCopy.bind(this));
  }

  render() {
    return (
      <input
        ref={(c) => (this.inputElement = c)}
        type="text"
        className="copy2clipboard-input"
        value={this.state.text}
      />
    );
  }

  private onCopy(text: string) {
    this.setState({ text }, () => {
      this.inputElement?.select();
      document.execCommand('copy');
    });
  }
}
