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

interface LinksProps extends RouteComponentProps {}

class Links extends Component<LinksProps> {
  state = {
    selectedToDelete: (null as any) as LinkModel,
    links: [] as LinkModel[],
  };

  async componentDidMount() {
    try {
      const links = await APIService.getUserLinks('me');
      this.setState({ links });
    } catch {}
  }

  render() {
    const links = this.state.links.map((l) => (
      <LinkTile
        key={`link-tile-${l.guid}`}
        link={l}
        onDelete={() => this.setState({ selectedToDelete: l })}
      />
    ));

    return (
      <div>
        <SearchBar placeholder="Search for links" />
        {this.state.selectedToDelete && this.deleteModal}
        {!!links.length ||
          ElementsUtil.repeat(5, (i: number) => (
            <TileSkeleton key={`tile-skeleton-${i}`} delay={`0.${i}s`} />
          ))}
        {links}
      </div>
    );
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
  }
}

export default withRouter(Links);
