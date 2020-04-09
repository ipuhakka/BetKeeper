import { call, put, takeLatest } from 'redux-saga/effects';
import _ from 'lodash';
import * as pageApi from '../js/Requests/Page';
import * as pageActions from '../actions/pageActions';
import {setLoading} from '../actions/loadingActions';
import {setAlertStatus} from '../actions/alertActions';

export function* getPage(action)
{
    yield put(setLoading(true));

    try 
    {
        const response = yield call(pageApi.getPage, action.payload.pathname);

        yield call(pageActions.getPageSuccess, JSON.parse(response.responseText));
    }
    catch (error)
    {
      switch(error.status)
      {
        case 401:
          yield put(setAlertStatus(error.status, "Session expired, please login again"));
          break;
        case 404:
          yield put(setAlertStatus(error.status, "Requested page was not found"));
          break;
        case 0:
          yield put(setAlertStatus(error.status, "Connection refused, server is likely down"));
          break;
        default:
          yield put(setAlertStatus(error.status, "Unexpected error occurred"));
          break;
      }
    }
    finally 
    {
      yield put(setLoading(false));
    }
}

export function* callAction(action)
{
  const { parameters, actionName, page, callback } = action.payload;
    
  yield put(setLoading(true));
  
  try 
  {
    const response = yield call(pageApi.postAction, page, actionName, parameters);
  
    const pageActionResponse = JSON.parse(response.responseText);

    if (_.get(pageActionResponse, 'components.length', null) > 0)
    {
      yield call(pageActions.updateComponents, page, pageActionResponse.components);
      return;
    }

    if (!_.isNil(callback))
    {
      callback();
    }

    yield put(setAlertStatus(response.status, response.responseText));

    pageActions.getPage(window.location.pathname);
  }
  catch (error)
  {
    switch (error.status)
    {
      case 401:
        yield put(setAlertStatus(error.status, error.responseText || "Session expired, please login again"));
        break;
      case 404:
        yield put(setAlertStatus(error.status, error.responseText || "Requested page was not found"));
        break;
      case 0:
        yield put(setAlertStatus(error.status, "Connection refused, server is likely down"));
        break;
      case 409:
        yield put(setAlertStatus(error.status, error.responseText));
        break;
      default:
        yield put(setAlertStatus(error.status, error.responseText || "Unexpected error occurred"));
        break;
    }
  }
  finally
  {
    yield put(setLoading(false));
  }
}

export function* updateOptions(action)
{
  const { payload } = action;

  const pageRoute = window.location.pathname.replace('page/', '');

  try 
  {
    const response = yield call(pageApi.updateOptions, payload, pageRoute);
    
    yield call(pageActions.updateOptionsSuccess, JSON.parse(response.responseText));
  }
  catch (error)
  {
    yield put(setAlertStatus(error.status, error.responseText || 'Unexpected error on updating dropdown option'));
  }
}

export function* watchGetPage()
{
    yield takeLatest(pageActions.GET_PAGE, getPage);
}

export function* watchCallModalAction()
{
  yield takeLatest(pageActions.CALL_ACTION, callAction);
}

export function* watchUpdateOptions()
{
  yield takeLatest(pageActions.UPDATE_OPTIONS, updateOptions);
}