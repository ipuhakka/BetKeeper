import React from 'react';
import { Route, BrowserRouter as Router, Switch} from 'react-router-dom';
import App from './App.jsx';
import BetsContainer from './containers/BetsContainer.jsx';
import Folders from './views/Folders/Folders.jsx';
import Home from './views/Home/Home.jsx';
import StatisticsContainer from './containers/StatisticsContainer.jsx';
import PrivateRoute from './components/PrivateRoute/PrivateRoute.jsx';

const Routes = () => {
  return (
    <Router>
      <Switch>
        <Route exact path='/' component={App}/>
        <PrivateRoute path='/folders' component={Folders} />
        <PrivateRoute path='/bets' component={BetsContainer} />
        <PrivateRoute path='/statistics' component={StatisticsContainer} />
        <PrivateRoute path='/home' component={Home} />
      </Switch>
    </Router>
)};

export default Routes;
