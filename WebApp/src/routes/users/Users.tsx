/** @format */

import './Users.scss';

import { Component } from 'react';
import { RouteComponentProps, withRouter } from 'react-router';
import { UserViewModel } from '../../services/api/models';
import APIService from '../../services/api/api';
import UsersTile from '../../components/user-tile/UserTile';
import ElementsUtil from '../../util/elements';
import TileSkeleton from '../../components/tile-skeleton/TileSkeleton';
import ModalWrapper from '../../components/modal/Modal';
import SnackBarService from '../../components/snackbar/SnackBarService';
import SearchBar from '../../components/search-bar/SearchBar';
import InputLimiter from '../../util/limiter';
import { ReactComponent as Add } from '../../assets/add.svg';

interface UsersProps extends RouteComponentProps {}

class Users extends Component<UsersProps> {
  state = {
    isSearch: false,
    selectedToDelete: (null as any) as UserViewModel,
    users: (null as any) as UserViewModel[],
  };

  private searchInputLimiter = InputLimiter.with(250);

  async componentDidMount() {
    await this.fetchUsers();
  }

  render() {
    const users = this.state.users?.map((u) => (
      <UsersTile
        user={u}
        onDelete={() => this.setState({ selectedToDelete: u })}
        onClick={() => this.props.history.push(`users/${u.guid}`)}
      />
    ));

    return (
      <div>
        <div className="links-top-bar">
          <SearchBar
            placeholder="Search for users"
            onChange={(v) => this.onSearchInput(v)}
          />
          <button
            className="add-btn"
            onClick={() => this.props.history.push('users/new')}
          >
            <Add />
          </button>
        </div>
        {this.state.selectedToDelete && this.deleteModal}
        {this.state.users === null &&
          ElementsUtil.repeat(4, (i) => (
            <TileSkeleton
              key={`tile-skeleton-${i}`}
              height="120px"
              delay={`0.${i}s`}
            />
          ))}
        {users}
      </div>
    );
  }

  private async fetchUsers(searchInput?: string) {
    try {
      this.setState({ links: null });
      let users;
      if (searchInput) {
        users = await APIService.searchUsers(searchInput);
      } else {
        users = await APIService.getUsers();
      }
      this.setState({ users });

      this.state.users.forEach(async (u) => {
        u.link_count = (await APIService.getUserLinksCount(u.guid)).count;
        this.setState({});
      });
    } catch {}
  }

  private get deleteModal(): JSX.Element {
    const user = this.state.selectedToDelete;
    return (
      <ModalWrapper
        heading={`Delete user '${user.username}'`}
        onClose={() => this.setState({ selectedToDelete: null })}
        bottomRow={
          <div>
            <button onClick={() => this.onUserDelete()}>DELETE</button>
            <button
              className="bg-blue"
              onClick={() => this.setState({ selectedToDelete: null })}
            >
              CANCEL
            </button>
          </div>
        }
      >
        <p>
          Do you really want to delete the user <code>{user.username}</code>.
        </p>
      </ModalWrapper>
    );
  }

  private async onUserDelete() {
    const user = this.state.selectedToDelete;
    if (!user) return;
    await APIService.deleteUser(user.guid);
    const i = this.state.users.findIndex((u) => u.guid === user.guid);
    if (i >= 0) this.state.users.splice(i, 1);
    this.setState({ selectedToDelete: null });
    SnackBarService.show('Link successfully removed.');
  }

  private onSearchInput(v: string) {
    this.searchInputLimiter.input(v, (res) => {
      this.setState({ isSearch: !!res });
      this.fetchUsers(res);
    });
  }
}

export default withRouter(Users);
