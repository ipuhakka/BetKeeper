import { call, put, takeLatest } from 'redux-saga/effects';
import _ from 'lodash';
import * as PageApi from '../js/Requests/Page';
import * as PageActions from '../actions/pageActions';
import * as PageUtils from '../js/pageUtils';
import {setLoading} from '../actions/loadingActions';
import {setAlertStatus} from '../actions/alertActions';

export function* getPage(action)
{
    yield put(setLoading(true));

    try 
    {
        const response = yield call(PageApi.getPage, action.payload.pathname);

        yield call(PageActions.getPageSuccess, JSON.parse(response.responseText));
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
    const response = yield call(PageApi.postAction, page, actionName, parameters);
  
    const pageActionResponse = JSON.parse(response.responseText);

    if (_.get(pageActionResponse, 'components.length', null) > 0)
    {
      yield call(PageActions.updateComponents, page, pageActionResponse.components);
      return;
    }

    if (!_.isNil(callback))
    {
      callback();
    }

    yield put(setAlertStatus(response.status, response.responseText));

    PageActions.getPage(window.location.pathname);
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

export function* handleServerDropdownUpdate(action)
{
  const { payload } = action;

  const pageRoute = window.location.pathname.replace('page/', '');

  try 
  {
    const response = yield call(PageApi.handleServerDropdownUpdate, payload, pageRoute);

    const pageKey = PageUtils.getActivePageName();
    
    yield call(
      PageActions.updateComponents,
      pageKey, 
      JSON.parse(response.responseText).components
    );
  }
  catch (error)
  {
    yield put(setAlertStatus(error.status, error.responseText || 'Unexpected error on updating dropdown value'));
  }
}

export function* watchGetPage()
{
    yield takeLatest(PageActions.GET_PAGE, getPage);
}

export function* watchCallModalAction()
{
  yield takeLatest(PageActions.CALL_ACTION, callAction);
}

export function* watchHandleServerDropdownUpdate()
{
  yield takeLatest(PageActions.HANDLE_SERVER_DROPDOWN_UPDATE, handleServerDropdownUpdate);
}