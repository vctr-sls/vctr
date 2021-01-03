/** @format */

import './SearchBar.scss';

import { Component } from 'react';
import { ReactComponent as Search } from '../../assets/search.svg';

interface SearchBarProps {
  placeholder?: string;
  onChange: (val: string) => void;
}

export default class SearchBar extends Component<SearchBarProps> {
  static defaultProps = {
    onChange: (val: string) => {},
  };

  render() {
    return (
      <div className="search-bar-container">
        <Search className="search-bar-icon" />
        <input
          className="search-bar-input"
          placeholder={this.props.placeholder}
          onChange={(v) => this.props.onChange.call(this, v.target.value)}
        />
      </div>
    );
  }
}
