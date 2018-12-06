import { all, takeLatest, call, put, select } from 'redux-saga/effects';
import {FETCH_FOLDERS, fetchFoldersSuccess, POST_FOLDER, DELETE_FOLDER} from './actions/foldersActions';
import {FETCH_BETS, fetchBetsSuccess, FETCH_BETS_FROM_FOLDER, fetchBetsFromFolderSuccess,
  FETCH_UNRESOLVED_BETS, fetchUnresolvedBetsSuccess, POST_BET, PUT_BET} from './actions/betsActions';
import {setAlertStatus, clearAlert} from './actions/alertActions';
import {getFolders, postFolder, deleteFolder} from './js/Requests/Folders.js';
import {getAllBetsByUser, getBetsFromFolder, getUnresolvedBets, postBet, putBet} from './js/Requests/Bets.js';



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

const getUsedFolder = (state) => {return state.bets.betsFromFolder.folder};

function* fetchBetsFromFolder(action){
  try {
    let bets = yield call(getBetsFromFolder, action.payload.folder);
    yield put(fetchBetsFromFolderSuccess({folder: action.payload.folder, bets: bets}));
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

function* fetchUnresolvedBets(){
  try {
    let bets = yield call(getUnresolvedBets);
    yield put(fetchUnresolvedBetsSuccess(bets));
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

function* createBet(action){
  try {
    yield call(postBet, action.payload.bet);
    yield put(setAlertStatus(201, "Bet added successfully"));
    yield call(fetchAllBets);
    let usedFolder = yield select(getUsedFolder);
    if (usedFolder !== ""){
      yield call(fetchBetsFromFolder, usedFolder);
    }
    yield call(fetchUnresolvedBets);
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

function* modifyBet(action){
  try {
    yield call(putBet, action.payload.bet_id, action.payload.data);
    yield put(setAlertStatus(204, "Result updated successfully"));
    yield call(fetchAllBets);
    let usedFolder = yield select(getUsedFolder);
    if (usedFolder !== ""){
      yield call(fetchBetsFromFolder, usedFolder);
    }
    yield call(fetchUnresolvedBets);
  } catch(error){
    switch(error){
      case 401:
        yield put(setAlertStatus(error, "Session expired, please login again"));
        break;
      case 0:
        yield put(setAlertStatus(error, "Connection refused, server is likely down"));
        break;
      case 404:
        yield put(setAlertStatus(error, "Bet trying to be modified was not found in the database"));
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

function* watchUnresolvedBets(){
  yield takeLatest(FETCH_UNRESOLVED_BETS, fetchUnresolvedBets);
}

function* watchPostBet(){
  yield takeLatest(POST_BET, createBet);
}

function* watchPutBet(){
  yield takeLatest(PUT_BET, modifyBet);
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
    watchUnresolvedBets(),
    watchPostBet(),
    watchPutBet(),
    watchClearAlert()
  ]);
}
