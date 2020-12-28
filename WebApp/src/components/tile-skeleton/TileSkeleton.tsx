/** @format */

import './TileSkeleton.scss';

import { Component } from 'react';

interface TileSkeletonProps {
  width: string | number;
  height: string | number;
  margin: string | number;
  delay: string;
}

export default class TileSkeleton extends Component<TileSkeletonProps> {
  static defaultProps = {
    width: '100%',
    height: '100px',
    margin: '0 0 20px 0',
    delay: '',
  };

  render() {
    const width = this.props.width;
    const height = this.props.height;
    const margin = this.props.margin;
    const animationDelay = this.props.delay;
    return (
      <div
        style={{ width, height, margin, animationDelay }}
        className="tile-skeleton"
      ></div>
    );
  }
}
