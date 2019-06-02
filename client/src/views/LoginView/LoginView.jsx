import React, { Component } from 'react';
import {withRouter} from 'react-router-dom';
import store from '../../store';
import Header from '../../components/Header/Header.jsx';
import Info from '../../components/Info/Info.jsx';
import Login from '../../components/Login/Login.jsx';
import SignUp from '../../components/SignUp/SignUp.jsx';
import {postToken, getToken} from '../../js/Requests/Token.js';
import {loginCredentialsExist} from '../../js/utils.js';

class LoginView extends Component {

  render() {
    return (
    <div onLoad={this.checkToken}>
      <Header title={"Welcome to Betkeeper"}></Header>
      <Info></Info>
      <Login requestToken={this.requestToken}></Login>
      <p>Or</p>
      <SignUp requestToken={this.requestToken}></SignUp>
    </div>
    );
  }

    //Checks if sessionStorage contains a valid token. If does, goes to user main page as logged user.
    //On error, set's alertStatus and proper message.
  checkToken = async () => {
    const {history} = this.props;

    if (loginCredentialsExist()){
      try {
        await getToken(sessionStorage.getItem('token'), sessionStorage.getItem('loggedUserID'));
        history.push('/home');
      }
      catch (err){
        switch(err){
          case 404:
            store.dispatch({type: 'SET_ALERT_STATUS',
              status: err,
              message: "Login expired"
            });
            break;
          default:
            store.dispatch({type: 'SET_ALERT_STATUS', payload: {
                status: err,
                message: "Unexpected error occurred"
              }
            });
        }
      }
    }
  }

  //Makes a post request to resource at URI/token. On success, sets the token from response, and user inputted username
  //to sessionStorage and changes html page.
  requestToken = async (user, passwd) => {
    const {history} = this.props;

    if (user === "" || passwd === ""){
      return;
    }

    try {
      let res = await postToken(user, passwd, this.handleReceiveToken);
      window.sessionStorage.setItem('token', res.token);
      window.sessionStorage.setItem('loggedUser', res.username);
      window.sessionStorage.setItem('loggedUserID', res.owner);
      store.dispatch({type: 'CLEAR_ALERT'});

      history.push('/home');
    } catch (err){
      switch(err){
        case 401:
          store.dispatch({type: 'SET_ALERT_STATUS',
            status: err,
            message: "Username or password given was incorrect"
          });
          break;
        case 0:
          store.dispatch({type: 'SET_ALERT_STATUS',
            status: err,
            message: "Network error"
          });
          break;
        default:
          store.dispatch({type: 'SET_ALERT_STATUS', payload: {
              status: err,
              message: "Unexpected error occurred"
            }
          });
          break;
      }
    }
  }
};

export default withRouter(LoginView);
