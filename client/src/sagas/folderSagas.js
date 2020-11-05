import { call, put, takeLatest} from 'redux-saga/effects';
import {FETCH_FOLDERS, POST_FOLDER, DELETE_FOLDER, FETCH_FOLDERS_OF_BET,
  fetchFoldersSuccess, fetchFoldersOfBetSuccess} from '../actions/foldersActions';
import {getFolders, getFoldersOfBet, postFolder, deleteFolder} from '../js/Requests/Folders.js';
import {setLoading} from '../actions/loadingActions';
import {setAlertStatus, setErrorResponseAlertStatus} from '../actions/alertActions';

export function* fetchFolders(){
  try 
  {
    yield put(setLoading(true));
    let folders = yield call(getFolders);
    yield put(fetchFoldersSuccess(folders));
  }
  catch(error)
  {
    yield put(setErrorResponseAlertStatus(error));
  }
  finally
  {
    yield put(setLoading(false));
  }
}

export function* fetchFoldersOfBet(action)
{
  try 
  {
    yield put(setLoading(true));

    let folders = yield call(getFoldersOfBet, action.payload.betId);

    yield put(fetchFoldersOfBetSuccess(folders));
  }
  catch(error)
  {
    yield put(setErrorResponseAlertStatus(error));
  }
  finally
  {
    yield put(setLoading(false));
  }
}

export function* createFolder(action){
  try {
    yield put(setLoading(true));
    yield call(postFolder, action.payload.newFolderName);
    yield call(fetchFolders);
    yield put(setAlertStatus(201, "Folder added successfully"));
  }
  catch(error)
  {
    yield put(setErrorResponseAlertStatus(error));
  }
  finally
  {
    yield put(setLoading(false));
  }
}

export function* removeFolder(action){
  try 
  {
    yield put(setLoading(true));
    yield call(deleteFolder, action.payload.folderToDelete);
    yield call(fetchFolders);
    yield put(setAlertStatus(204, "Folder deleted successfully"));
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

export function* watchFolders(){
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
