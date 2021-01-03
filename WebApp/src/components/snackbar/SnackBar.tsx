/** @format */

import './SnackBar.scss';

import { Component } from 'react';
import SnackBarService, {
  SnackBarType,
  SnackBarPayload,
} from './SnackBarService';

export default class SnackBar extends Component {
  state = {
    payload: (null as any) as SnackBarPayload,
    show: false,
  };

  componentDidMount() {
    SnackBarService.events.on('show', (payload) =>
      this.setState({ show: true, payload })
    );
    SnackBarService.events.on('hide', () => this.setState({ show: false }));
  }

  render() {
    return (
      <div className={`snackbar-container ${this.modifier}`} style={this.style}>
        {this.content}
      </div>
    );
  }

  private get style(): React.CSSProperties {
    const show = this.state.show;
    return {
      bottom: show ? 0 : -100,
      opacity: show ? 1 : 0,
    };
  }

  private get modifier(): string {
    switch (this.state.payload?.type) {
      case SnackBarType.ERROR:
        return 'error';
      default:
        return '';
    }
  }

  private get content(): JSX.Element {
    const content = this.state.payload?.content;
    return typeof content === 'string' ? <p>{content}</p> : content;
  }
}
