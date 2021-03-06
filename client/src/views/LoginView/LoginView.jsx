import React, { Component } from 'react';
import { connect } from 'react-redux';
import { withRouter } from 'react-router-dom';
import _ from 'lodash';
import store from '../../store';
import Header from '../../components/Header/Header.jsx';
import Login from '../../components/Login/Login.jsx';
import SignUp from '../../components/SignUp/SignUp.jsx';
import { checkCurrentLoginCredentials } from '../../actions/sessionActions';
import './loginView.css';

class LoginView extends Component 
{
  componentDidMount()
  {
    const loggedUserId = window.localStorage.getItem('loggedUserId');
    const tokenString = window.localStorage.getItem('token');

    if (!_.isNil(loggedUserId) 
      && !_.isNil(tokenString))
      {
        store.dispatch(checkCurrentLoginCredentials(
          loggedUserId, 
          tokenString, 
          '/page/home', 
          this.props.history));
      }
  }

  render() 
  {
    return (
    <div className='loginView'>
      <Header title={"Welcome to Betkeeper"}></Header>
      <Login requestToken={this.requestToken}></Login>
      <p>Or</p>
      <SignUp requestToken={this.requestToken}></SignUp>
    </div>
    );
  }

  //Makes a post request to resource at URI/token. On success, sets the token from response, and user inputted username
  //to localStorage and changes html page.
  requestToken = async (username, password) => 
  {
    const {history} = this.props;

    if (username === "" || password === ""){
      return;
    }

    store.dispatch({type: 'LOGIN', payload: {
        username,
        password,
        history,
        redirectTo: '/page/home'
      }
    });
  }
};

const mapStateToProps = (state, ownProps) => {
  return { ...state.session};
};

export default withRouter(connect(mapStateToProps)(LoginView));
