import React from 'react';
import { Route, BrowserRouter as Router, Switch} from 'react-router-dom';
import LoginView from './views/LoginView/LoginView';
import PrivateRoute from './components/PrivateRoute/PrivateRoute';
import PageContainer from './containers/PageContainer';

const Routes = () => {
  return (
    <Router>
      <Switch>
        <Route exact path='/' component={LoginView}/>
        <PrivateRoute path='/page' component={PageContainer} />
      </Switch>
    </Router>
)};

export default Routes;
