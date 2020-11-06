import { call, put } from 'redux-saga/effects';
import { setErrorResponseAlertStatus } from '../actions/alertActions';
import { setLoading } from '../actions/loadingActions';

/**
 * Performs a function with loading
 * @param {*} func 
 */
export function* withLoading(func)
{
    yield put(setLoading(true));
    try
    {
        yield func();
    }
    finally
    {
        yield put(setLoading(false));
    }
}

/**
 * Perform a request function with error response handling
 * @param {*} func 
 */
export function* withErrorResponseHandler(func)
{
    try 
    {
        yield call(func);
    }
    catch (errorResponse)
    {
        yield put(setErrorResponseAlertStatus(errorResponse));
    }
}