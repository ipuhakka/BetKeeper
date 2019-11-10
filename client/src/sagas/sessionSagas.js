import { call, put, takeEvery} from 'redux-saga/effects';
import {logOut} from '../actions/sessionActions.js';
import {postToken, deleteToken, getToken} from '../js/Requests/Token.js';
import {setAlertStatus} from '../actions/alertActions';

function clearCredentials()
{
  window.sessionStorage.setItem('loggedUser', null);
  window.sessionStorage.setItem('token', null);
  window.sessionStorage.setItem('loggedUserId', -1);
}

export function* handleLogin(action){

  const { username, password, history, redirectTo} = action.payload;

  try {

    let res = yield call(postToken, username, password);

    window.sessionStorage.setItem('token', res.token);
    window.sessionStorage.setItem('loggedUser', res.username);
    window.sessionStorage.setItem('loggedUserId', res.owner);

    if (redirectTo !== undefined && history !== undefined)
    {
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

function* handleLogOut()
{

  try 
  {
    yield call(deleteToken);
  }
  catch (e)
  {

  }
  finally 
  {
    clearCredentials();
  }
}

function* checkLogin(action)
{
  const {userId, tokenString, redirectTo, history} = action.payload;

  try 
  {
    yield call(getToken, tokenString, userId);
    history.push(redirectTo);
  }
  catch (e)
  {
    clearCredentials();
  }
}

export function* watchLogin()
{
  yield takeEvery('LOGIN', handleLogin);
}

export function* watchLogOut()
{
  yield takeEvery('LOGOUT', handleLogOut);
}

export function* watchCheckLogin()
{
  yield takeEvery('CHECK_LOGIN', checkLogin);
}
