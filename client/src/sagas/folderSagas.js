import { call, put, takeLatest} from 'redux-saga/effects';
import {FETCH_FOLDERS, POST_FOLDER, DELETE_FOLDER, FETCH_FOLDERS_OF_BET,
  fetchFoldersSuccess, fetchFoldersOfBetSuccess} from '../actions/foldersActions';
import {getFolders, getFoldersOfBet, postFolder, deleteFolder} from '../js/Requests/Folders.js';
import {setLoading} from '../actions/loadingActions';
import {setAlertStatus} from '../actions/alertActions';

export function* fetchFolders(){
  try {
    yield put(setLoading(true));
    let folders = yield call(getFolders);
    yield put(fetchFoldersSuccess(folders));
  }
  catch(error){
    switch(error){
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
  finally{
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
    switch(error)
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
    switch(error)
    {
      case 400:
        yield put(setAlertStatus(error, "Folder name contained over 50 characters"));
        break;
      case 401:
        yield put(setAlertStatus(error, "Session expired, please login again"));
        break;
      case 409:
        yield put(setAlertStatus(error, "You already have a folder with given name, please select another name or delete old folder."));
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
    yield put(setLoading(false));
  }
}

export function* removeFolder(action){
  try {
    yield put(setLoading(true));
    yield call(deleteFolder, action.payload.folderToDelete);
    yield call(fetchFolders);
    yield put(setAlertStatus(204, "Folder deleted successfully"));
  }
  catch (error){
      switch(error){
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
  finally{
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
