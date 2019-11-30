import { call, put, takeEvery} from 'redux-saga/effects';
import {logOut} from '../actions/sessionActions.js';
import {postToken, deleteToken, getToken} from '../js/Requests/Token';
import { postUser } from '../js/Requests/Users';
import {setAlertStatus} from '../actions/alertActions';
import {setLoading} from '../actions/loadingActions';

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

    switch(error)
    {
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
  catch (e)
  {

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
    switch(error)
    {
      case 409:
        yield put(setAlertStatus(error, "User with same username already exists"));
        break;

      default:
        yield put(setAlertStatus(error, "Something went wrong with the request"));
        break;
    }
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
