import React, { Component } from 'react';
import { connect } from 'react-redux';
import {withRouter} from 'react-router-dom';
import store from '../../store';
import Header from '../../components/Header/Header.jsx';
import Info from '../../components/Info/Info.jsx';
import Login from '../../components/Login/Login.jsx';
import SignUp from '../../components/SignUp/SignUp.jsx';
import './loginView.css';

class LoginView extends Component 
{

  render() 
  {
    return (
    <div className='loginView'>
      <Header title={"Welcome to Betkeeper"}></Header>
      <Info></Info>
      <Login requestToken={this.requestToken}></Login>
      <p>Or</p>
      <SignUp requestToken={this.requestToken}></SignUp>
    </div>
    );
  }

  //Makes a post request to resource at URI/token. On success, sets the token from response, and user inputted username
  //to sessionStorage and changes html page.
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
        redirectTo: '/home'
      }
    });
  }
};

const mapStateToProps = (state, ownProps) => {
  return { ...state.session};
};

export default withRouter(connect(mapStateToProps)(LoginView));
