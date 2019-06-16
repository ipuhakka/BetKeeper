import React, {Component} from 'react';
import { connect } from 'react-redux';
import { Route,  Redirect} from 'react-router-dom';
import store from '../../store';
import {loginCredentialsExist} from '../../js/utils.js';

class PrivateRoute extends Component{

  componentWillMount(){
    store.dispatch({type: 'CHECK_LOGIN_STATUS'});
  }

  render(){
    const {props} = this;

    return loginCredentialsExist() ?
      <Route path={props.path} component={props.component} /> :
      <Redirect to='/' from='props.path' />;
  }
};

const mapStateToProps = (state, ownProps) => {
  return { ...state.session};
};

export default connect(mapStateToProps)(PrivateRoute);
