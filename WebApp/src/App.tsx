/** @format */

import './App.scss';

import { Component } from 'react';
import APIService from './services/api/api';
import StateService from './services/state/state';
import { BrowserRouter as Router, Route, Redirect } from 'react-router-dom';
import Sidebar, { SidebarEntry } from './components/sidebar/Sidebar';
import Login from './routes/login/Login';
import { Permissions, UserModel } from './services/api/models';
import Links from './routes/links/Links';
import LinkEditor from './routes/link-editor/LinkEditor';
import NotFound from './routes/not-found/NotFound';
import Password from './routes/password/Password';
import SnackBar from './components/snackbar/SnackBar';
import SnackBarService, {
  SnackBarType,
} from './components/snackbar/SnackBarService';
import Users from './routes/users/Users';
import UserEditor from './routes/user-editor/UserEditor';
import { createBrowserHistory } from 'history';
import Copy2Clipboard from './components/copy2cb/Copy2Clipboard';
import { STATUS_CODES } from 'http';
import ApiKey from './routes/apikey/ApiKey';

const IGNORE_AUTH_ROUTES = ['notfound', 'password'];

export const history = createBrowserHistory({
  basename: process.env.PUBLIC_URL,
});

export default class App extends Component {
  private stateService = new StateService();

  state = {
    loggedIn: false,
    redirect: (null as any) as string,
  };

  async componentDidMount() {
    this.stateService.events.on('update', () => this.setState({}));

    if (
      IGNORE_AUTH_ROUTES.includes(
        window.location.pathname.replace(process.env.PUBLIC_URL, '').substr(1)
      )
    )
      return;

    APIService.events.on('authentication-error', () => {
      this.stateService.selfUser = (null as any) as UserModel;
      this.redirect('login');
    });

    APIService.events.on('error', (res: Response) => this.handleApiError(res));

    if (this.stateService.selfUser === null) {
      try {
        const me = await APIService.getUser('me');
        this.stateService.selfUser = me;
        this.setState({ loggedIn: true });
      } catch {}
    }
  }

  render() {
    return (
      <div className="app-container">
        <Copy2Clipboard />

        {this.isLoggedIn && (
          <Sidebar
            entries={this.sidebarEntries}
            onActivate={(r) => this.onSidebarActivate(r)}
          />
        )}
        <SnackBar />
        <div className="app-router-outlet">
          <Router basename={process.env.PUBLIC_URL}>
            <Route
              exact
              path="/login"
              render={() => <Login state={this.stateService} />}
            />
            <Route exact path="/links" render={() => <Links />} />
            <Route
              exact
              path="/links/:id"
              render={({ match }) => <LinkEditor id={match.params.id} />}
            />
            <Route exact path="/users" render={() => <Users />} />
            <Route
              exact
              path="/users/:id"
              render={({ match }) => <UserEditor id={match.params.id} />}
            />
            <Route
              exact
              path="/apikey"
              render={({ match }) => <ApiKey id={match.params.id} />}
            />
            <Route exact path="/notfound" render={() => <NotFound />} />
            <Route exact path="/password" render={() => <Password />} />

            <Route exact path="/" render={() => <Redirect to="links" />} />

            {this.state.redirect && <Redirect to={this.state.redirect} />}
          </Router>
        </div>
      </div>
    );
  }

  private async onSidebarActivate(r: string) {
    if (r === 'logout') {
      try {
        await APIService.authLogout();
        this.stateService.selfUser = null as any;
        this.setState({ loggedIn: false });
        r = 'login';
      } catch (err) {
        console.error(err);
        return;
      }
    }
    this.redirect('/' + r);
  }

  private redirect(to: string) {
    this.setState({ redirect: to }, () => this.setState({ redirect: null }));
  }

  private get isLoggedIn(): boolean {
    return !!this.stateService.selfUser || this.state.loggedIn;
  }

  private get sidebarEntries(): SidebarEntry[] {
    const entries: SidebarEntry[] = [
      {
        displayname: 'links',
        route: 'links',
      },
    ];

    if (this.stateService.selfUser?.permissions & Permissions.UPDATE_USERS) {
      entries.push({
        displayname: 'users',
        route: 'users',
      });
    } else {
      entries.push({
        displayname: 'me',
        route: 'users/me',
      });
    }

    if (this.stateService.selfUser?.permissions & Permissions.CREATE_API_KEY) {
      entries.push({
        displayname: 'api key',
        route: 'apikey',
      });
    }

    return entries;
  }

  private async handleApiError(res: Response) {
    let content: string | JSX.Element = 'Unexpected error.';

    if (!!res.body) {
      let msg = res.statusText ?? STATUS_CODES[res.status] ?? 'Unknown Error';
      const body = await res.json();
      if (!!body.error) {
        msg = body.error;
      } else if (!!body.errors) {
        msg = body.errors[Object.keys(body.errors)[0]][0];
      }
      content = (
        <span>
          Request Failed: <code>{msg}</code>
        </span>
      );
    }

    SnackBarService.show(content, SnackBarType.ERROR, 5000);
    console.error(res);
  }
}
