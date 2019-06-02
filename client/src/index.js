import React from 'react';
import ReactDOM from 'react-dom';
import { Route, BrowserRouter as Router, Redirect} from 'react-router-dom';
import { Provider } from 'react-redux';
import store from './store';
import App from './App.jsx';
import BetsContainer from './containers/BetsContainer.jsx';
import Folders from './views/Folders/Folders.jsx';
import Home from './views/Home/Home.jsx';
import StatisticsContainer from './containers/StatisticsContainer.jsx';
import {loginCredentialsExist} from './js/utils.js';
import {getToken} from './js/Requests/Token.js';
import './index.css';
import registerServiceWorker from './registerServiceWorker';

const routes = (
  <Provider store={store}>
    <Router>
      <Route exact path='/' component={App}/>
      <Route path='/folders' component={Folders} authenticated={checkLogin()}/>
      <Route path='/bets' component={BetsContainer} authenticated={checkLogin()} />
      <Route path='/statistics' component={StatisticsContainer} authenticated={checkLogin()} />
      <Route path='/home' component={Home} authenticated={checkLogin()} />
    </Router>
  </Provider>
);

//Checks if user is still logged in.
//On error, set's alertStatus and a message.
async function checkLogin(){

  if (loginCredentialsExist()){

    try {
      await getToken(sessionStorage.getItem('token'), sessionStorage.getItem('loggedUserID'));

      return true;
    }
    catch (err){

      switch(err){

        case 404:
          store.dispatch({type: 'SET_ALERT_STATUS',
            status: err,
            message: "Login expired"
          });

          return false;

        default:
          store.dispatch({type: 'SET_ALERT_STATUS', payload: {
              status: err,
              message: "Unexpected error occurred"
            }
          });
          return false;
      }
    }
  }

  return false;
}

function PrivateRoute ({component: Component, ...rest}) {
  return (
    <Route
      {...rest}
      render={(props) => {
        console.log(props);

        return props.authenticated
        ? <Component {...props} />
        : <Redirect to={{pathname: '/', state: {from: props.location}}} />}}
    />
  )
}

ReactDOM.render(routes, document.getElementById('root'));
registerServiceWorker();
