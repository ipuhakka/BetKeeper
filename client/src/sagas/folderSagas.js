import { call, put, takeLatest} from 'redux-saga/effects';
import {FETCH_FOLDERS, FETCH_FOLDERS_OF_BET,
  fetchFoldersSuccess, fetchFoldersOfBetSuccess} from '../actions/foldersActions';
import {getFolders, getFoldersOfBet} from '../js/Requests/Folders.js';
import { withErrorResponseHandler, withLoading } from './helperSagas';

export function* fetchFolders()
{
  yield call(withLoading, function*() 
  { 
    yield call(withErrorResponseHandler, function*()
    {
      const response = yield call(getFolders);

      const folders = JSON.parse(response.responseText);
      yield put(fetchFoldersSuccess(folders));
    }); 
  });
}

export function* fetchFoldersOfBet(action)
{
  yield call(withLoading, function*() 
  { 
    yield call(withErrorResponseHandler, function*()
    {
      const response = yield call(getFoldersOfBet, action.payload.betId);
      const folders = JSON.parse(response.responseText);
      yield put(fetchFoldersOfBetSuccess(folders));
    }); 
  });
}

export function* watchFolders()
{
  yield takeLatest(FETCH_FOLDERS, fetchFolders);
}

export function* watchFoldersOfBet(){
  yield takeLatest(FETCH_FOLDERS_OF_BET, fetchFoldersOfBet);
}
