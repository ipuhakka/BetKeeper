import React from 'react';
import { Route, BrowserRouter as Router, Switch} from 'react-router-dom';
import LoginView from './views/LoginView/LoginView';
import BetsContainer from './containers/BetsContainer';
import Home from './views/Home/Home';
import StatisticsContainer from './containers/StatisticsContainer';
import PrivateRoute from './components/PrivateRoute/PrivateRoute';
import PageContainer from './containers/PageContainer';

const Routes = () => {
  return (
    <Router>
      <Switch>
        <Route exact path='/' component={LoginView}/>
        <PrivateRoute path='/bets' component={BetsContainer} />
        <PrivateRoute path='/statistics' component={StatisticsContainer} />
        <PrivateRoute path='/home' component={Home} />
        <PrivateRoute path='/page' component={PageContainer} />
      </Switch>
    </Router>
)};

export default Routes;
