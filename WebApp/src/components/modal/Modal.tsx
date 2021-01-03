/** @format */

import './Modal.scss';

import { Component } from 'react';

interface ModalWrapperProps {
  heading?: string;
  bottomRow?: JSX.Element;
  onClose: () => void;
}

export default class ModalWrapper extends Component<ModalWrapperProps> {
  static defaultProps = {
    onClose: () => {},
  };

  render() {
    return (
      <div
        id="modal-background"
        className="modal-background-container"
        onClick={(e) => this.onBackgroundClick(e)}
      >
        <div>
          <div>
            {this.props.heading && <h2>{this.props.heading}</h2>}
            {this.props.children}
            {this.props.bottomRow && (
              <div className="modal-bottomrow">{this.props.bottomRow}</div>
            )}
          </div>
        </div>
      </div>
    );
  }

  private onBackgroundClick(e: React.MouseEvent<HTMLDivElement, MouseEvent>) {
    if ((e.target as HTMLElement).id === 'modal-background') {
      this.props.onClose.call(this);
    }
  }
}
