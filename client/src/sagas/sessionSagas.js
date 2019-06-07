import { call, put, takeEvery} from 'redux-saga/effects';
import {logOut} from '../actions/sessionActions.js';
import {postToken, deleteToken} from '../js/Requests/Token.js';
import {setAlertStatus} from '../actions/alertActions';

export function* handleLogin(action){

  const { username, password, history, redirectTo} = action.payload;

  try {

    let res = yield call(postToken, username, password);

    window.sessionStorage.setItem('token', res.token);
    window.sessionStorage.setItem('loggedUser', res.username);
    window.sessionStorage.setItem('loggedUserID', res.owner);

    if (redirectTo !== undefined && history !== undefined){
      history.push(redirectTo);
    }
  }
  catch (error){

    yield call(logOut);

    switch(error){
      case 401:
        yield put(setAlertStatus(error, "Username or password given was incorrect"));
        break;
      case 0:
        yield put(setAlertStatus(error, "Connection refused, server is likely down"));
        break;
      default:
        yield put(setAlertStatus(error, "Unexpected error occurred"));
      }
  }

}

function* handleLogOut(){

  try {
    yield call(deleteToken);
  }
  catch (e){

  }
  finally {
    window.sessionStorage.setItem('loggedUser', null);
    window.sessionStorage.setItem('token', null);
    window.sessionStorage.setItem('loggedUserID', -1);
  }
}

export function* watchLogin(){
  yield takeEvery('LOGIN', handleLogin);
}

export function* watchLogOut(){
  yield takeEvery('LOGOUT', handleLogOut);
}
