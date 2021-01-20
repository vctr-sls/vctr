/** @format */

import './Links.scss';

import { Component } from 'react';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import APIService from '../../services/api/api';
import { LinkModel } from '../../services/api/models';
import LinkTile from '../../components/link-tile/LinkTile';
import ModalWrapper from '../../components/modal/Modal';
import TileSkeleton from '../../components/tile-skeleton/TileSkeleton';
import ElementsUtil from '../../util/elements';
import SearchBar from '../../components/search-bar/SearchBar';
import InputLimiter from '../../util/limiter';
import { ReactComponent as Add } from '../../assets/add.svg';
import SnackBarService from '../../components/snackbar/SnackBarService';

interface LinksProps extends RouteComponentProps {}

class Links extends Component<LinksProps> {
  state = {
    isSearch: false,
    selectedToDelete: (null as any) as LinkModel,
    links: (null as any) as LinkModel[],
  };

  private searchInputLimiter = InputLimiter.with(250);

  async componentDidMount() {
    await this.fetchLinks();
  }

  render() {
    const links = this.state.links?.map((l) => (
      <LinkTile
        key={`link-tile-${l.guid}`}
        link={l}
        onDelete={() => this.setState({ selectedToDelete: l })}
        onClick={() => this.props.history.push(`links/${l.guid}`)}
      />
    ));

    return (
      <div>
        <div className="links-top-bar">
          <SearchBar
            placeholder="Search for links"
            onChange={(v) => this.onSearchInput(v)}
          />
          <button
            className="add-btn"
            onClick={() => this.props.history.push('links/new')}
          >
            <Add />
          </button>
        </div>
        {this.state.selectedToDelete && this.deleteModal}
        {this.state.links === null &&
          ElementsUtil.repeat(5, (i) => (
            <TileSkeleton key={`tile-skeleton-${i}`} delay={`0.${i}s`} />
          ))}
        {links?.length === 0 && (
          <p className="links-no-links">No links available.</p>
        )}
        {links}
      </div>
    );
  }

  private async fetchLinks(searchInput?: string) {
    try {
      this.setState({ links: null });
      let links;
      if (searchInput) {
        links = await APIService.searchUserLinks('me', searchInput!);
      } else {
        links = await APIService.getUserLinks('me');
      }
      this.setState({ links });
    } catch {}
  }

  private get deleteModal(): JSX.Element {
    const link = this.state.selectedToDelete;
    return (
      <ModalWrapper
        heading={`Delete link '${link.ident}'`}
        onClose={() => this.setState({ selectedToDelete: null })}
        bottomRow={
          <div>
            <button onClick={() => this.onLinkDelete()}>DELETE</button>
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
          Do you really want to delete the link <code>{link.ident}</code>{' '}
          (redirecting to <code>{link.destination}</code>).
        </p>
      </ModalWrapper>
    );
  }

  private async onLinkDelete() {
    const link = this.state.selectedToDelete;
    if (!link) return;

    try {
      await APIService.deleteLink(link.guid);
      const i = this.state.links.findIndex((l) => l.guid === link.guid);
      if (i >= 0) this.state.links.splice(i, 1);
      this.setState({ selectedToDelete: null });
      SnackBarService.show('Link successfully removed.');
    } catch {}
  }

  private onSearchInput(v: string) {
    this.searchInputLimiter.input(v, (res) => {
      this.setState({ isSearch: !!res });
      this.fetchLinks(res);
    });
  }
}

export default withRouter(Links);
