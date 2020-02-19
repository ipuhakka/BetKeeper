import { call, put, takeLatest } from 'redux-saga/effects';
import * as pageApi from '../js/Requests/Page';
import * as pageActions from '../actions/pageActions';
import {setLoading} from '../actions/loadingActions';
import {setAlertStatus} from '../actions/alertActions';

export function* getPage(action)
{
    setLoading(true);

    try 
    {
        const response = yield call(pageApi.getPage, action.payload.pathname);

        yield call(pageActions.getPageSuccess, JSON.parse(response.responseText));
    }
    catch (error)
    {
        switch(error.statusCode)
        {
          case 401:
            yield put(setAlertStatus(error, "Session expired, please login again"));
            break;
          case 0:
            yield put(setAlertStatus(error, "Connection refused, server is likely down"));
            break;
          default:
            yield put(setAlertStatus(error, "Unexpected error occurred"));
            break;
        }
    }
    finally 
    {
        setLoading(false);
    }
}

export function* watchGetPage()
{
    yield takeLatest(pageActions.GET_PAGE, getPage);
}