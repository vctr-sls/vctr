/** @format */

import './PermsSelector.scss';

import { Component } from 'react';
import { Permissions } from '../../services/api/models';
import PermissionsUtil from '../../util/permissions';

interface PermsSelectorProps {
  value: Permissions;
  onChange: (v: Permissions) => void;
}

export default class PermsSelector extends Component<PermsSelectorProps> {
  static defaultProps = {
    value: 0,
    onChange: (_: Permissions) => {},
  };

  render() {
    const value = this.props.value as number;

    const elements = Object.keys(PermissionsUtil.map).map((k) => {
      const val = PermissionsUtil.map[k] as number;
      const id = `i-perms-${k}`;
      return (
        <div className="perms-container">
          <input
            type="checkbox"
            id={id}
            checked={(value & val) === val && val > 0}
            onChange={(e) => {
              this.props.onChange.call(
                this,
                e.target.checked ? value | val : value ^ val
              );
            }}
          />
          <label htmlFor={id}>{PermissionsUtil.transformName(k)}</label>
        </div>
      );
    });

    return <div>{elements}</div>;
  }
}
