import { call, put, takeLatest} from 'redux-saga/effects';
import {FETCH_FOLDERS, POST_FOLDER, DELETE_FOLDER, FETCH_FOLDERS_OF_BET,
  fetchFoldersSuccess, fetchFoldersOfBetSuccess} from '../actions/foldersActions';
import {getFolders, getFoldersOfBet, postFolder, deleteFolder} from '../js/Requests/Folders.js';
import {setAlertStatus} from '../actions/alertActions';
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

export function* createFolder(action)
{
  yield call(withLoading, function*() 
  { 
    yield call(withErrorResponseHandler, function*()
    {
      yield call(postFolder, action.payload.newFolderName);
      yield call(fetchFolders);
      yield put(setAlertStatus(201, "Folder added successfully"));
    }); 
  });
}

export function* removeFolder(action)
{
  yield call(withLoading, function*() 
  { 
    yield call(withErrorResponseHandler, function*()
    {
      yield call(deleteFolder, action.payload.folderToDelete);
      yield call(fetchFolders);
      yield put(setAlertStatus(204, "Folder deleted successfully"));
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

export function* watchPostFolder(){
  yield takeLatest(POST_FOLDER, createFolder);
}

export function* watchDeleteFolder(){
  yield takeLatest(DELETE_FOLDER, removeFolder);
}
