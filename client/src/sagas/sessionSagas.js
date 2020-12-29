import { call, put, takeEvery} from 'redux-saga/effects';
import {logOut} from '../actions/sessionActions.js';
import {postToken, deleteToken, getToken} from '../js/Requests/Token';
import { postUser } from '../js/Requests/Users';
import {setAlertStatus, setErrorResponseAlertStatus} from '../actions/alertActions';
import {setLoading} from '../actions/loadingActions';
import _ from 'lodash';

function clearCredentials()
{
  window.sessionStorage.setItem('loggedUser', null);
  window.sessionStorage.setItem('token', null);
  window.sessionStorage.setItem('loggedUserId', -1);
}

export function* handleLogin(action){
  const { username, password, history, redirectTo} = action.payload;

  try 
  {
    yield put(setLoading(true));

    let res = yield call(postToken, username, password);

    window.sessionStorage.setItem('token', res.token);
    window.sessionStorage.setItem('loggedUser', res.username);
    window.sessionStorage.setItem('loggedUserId', res.owner);

    if (redirectTo !== undefined && history !== undefined)
    {
      history.push(redirectTo);
    }
  }
  catch (error)
  {
    yield call(logOut);
    switch(error.status)
    {
      case 401:
        yield put(setAlertStatus(error.status, "Username or password given was incorrect"));
        break;
      case 0:
        yield put(setAlertStatus(error.status, "Connection refused, server is likely down"));
        break;
      default:
        yield put(setAlertStatus(error.status, "Unexpected error occurred"));
      }
  }
  finally 
  {
    yield put(setLoading(false));
  }

}

function* handleLogOut()
{
  try 
  {
    yield put(setLoading(true));
    yield call(deleteToken);
  }
  catch (err)
  {
    // Empty catch for handling error response properly
  }
  finally 
  {
    yield put(setLoading(false));
    clearCredentials();
  }
}

function* checkLogin(action)
{
  const {userId, tokenString, redirectTo, history} = action.payload;

  if (_.isNil(tokenString) || userId.toString() === '-1')
  {
    return;
  }

  try 
  {
    yield put(setLoading(true));
    yield call(getToken, tokenString, userId);
    history.push(redirectTo);
  }
  catch (e)
  {
    clearCredentials();
  }
  finally 
  {
    yield put(setLoading(false));
  }
}

function* signUp(action)
{
  const {username, password, callback} = action.payload;

  try 
  {
    yield put(setLoading(true));

    let user = yield call(postUser, username, password);
    
    callback(user.username, user.password);
  } 
  catch (error)
  {
    yield put(setErrorResponseAlertStatus(error));
  }
  finally 
  {
    yield put(setLoading(false));
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

export function* watchSignUp()
{
  yield takeEvery('SIGNUP', signUp);
}
