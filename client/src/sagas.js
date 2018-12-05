import { all, takeLatest, call, put } from 'redux-saga/effects';
import {FETCH_FOLDERS, fetchFoldersSuccess, POST_FOLDER, DELETE_FOLDER} from './actions/foldersActions';
import {FETCH_BETS, fetchBetsSuccess, FETCH_BETS_FROM_FOLDER, fetchBetsFromFolderSuccess} from './actions/betsActions';
import {setAlertStatus, clearAlert} from './actions/alertActions';
import {getFolders, postFolder, deleteFolder} from './js/Requests/Folders.js';
import {getAllBetsByUser, getBetsFromFolder} from './js/Requests/Bets.js';



function* fetchFolders(){
  try {
    let folders = yield call(getFolders);
    yield put(fetchFoldersSuccess(folders));
  } catch(error){
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
}

function* createFolder(action){
  try {
    yield call(postFolder, action.payload.newFolderName);
    yield call(fetchFolders);
    yield put(setAlertStatus(201, "Folder added successfully"));
  } catch(error){
      switch(error){
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
}

function* removeFolder(action){
  try {
    yield call(deleteFolder, action.payload.folderToDelete);
    yield call(fetchFolders);
    yield put(setAlertStatus(204, "Folder deleted successfully"));
  } catch (error){
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
}

function* fetchAllBets(){
  try {
    let bets = yield call(getAllBetsByUser);
    yield put(fetchBetsSuccess(bets));
  } catch(error){
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
}

function* fetchBetsFromFolder(action){
  try {
    let bets = yield call(getBetsFromFolder, action.payload.folder);
    yield put(fetchBetsFromFolderSuccess(bets));
  } catch(error){
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
}

function* watchFolders(){
  yield takeLatest(FETCH_FOLDERS, fetchFolders);
}

function* watchPostFolder(){
  yield takeLatest(POST_FOLDER, createFolder);
}

function* watchDeleteFolder(){
  yield takeLatest(DELETE_FOLDER, removeFolder);
}

function* watchAllBets(){
  yield takeLatest(FETCH_BETS, fetchAllBets);
}

function* watchBetsFromFolder(){
  yield takeLatest(FETCH_BETS_FROM_FOLDER, fetchBetsFromFolder);
}

function* watchClearAlert(){
  yield call(clearAlert);
}

export default function* rootSaga() {
  yield all([
    watchFolders(),
    watchPostFolder(),
    watchDeleteFolder(),
    watchAllBets(),
    watchBetsFromFolder(),
    watchClearAlert()
  ]);
}
