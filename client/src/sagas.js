import { all, takeLatest, call, put, select } from 'redux-saga/effects';
import {FETCH_FOLDERS, fetchFoldersSuccess, POST_FOLDER, DELETE_FOLDER, FETCH_FOLDERS_OF_BET, fetchFoldersOfBetSuccess} from './actions/foldersActions';
import {FETCH_BETS, fetchBetsSuccess, FETCH_BETS_FROM_FOLDER, fetchBetsFromFolderSuccess,
  FETCH_UNRESOLVED_BETS, fetchUnresolvedBetsSuccess, POST_BET, PUT_BET, DELETE_BET,
FETCH_BETS_FROM_ALL_FOLDERS, fetchBetsFromAllFoldersSuccess, FETCH_FINISHED_BETS, fetchFinishedBetsSuccess} from './actions/betsActions';
import {setAlertStatus, clearAlert} from './actions/alertActions';
import {getFolders, getFoldersOfBet, postFolder, deleteFolder} from './js/Requests/Folders.js';
import {getAllBetsByUser, getBetsFromFolder, getFinishedBets, getUnresolvedBets, postBet, putBet, deleteBet} from './js/Requests/Bets.js';
import {setLoading} from './actions/loadingActions';


function* fetchFolders(){
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

function* fetchFoldersOfBet(action){
  try {
    yield put(setLoading(true));
    let folders = yield call(getFoldersOfBet, action.payload.bet_id);
    yield put(fetchFoldersOfBetSuccess(folders));
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

function* createFolder(action){
  try {
    yield put(setLoading(true));
    yield call(postFolder, action.payload.newFolderName);
    yield call(fetchFolders);
    yield put(setAlertStatus(201, "Folder added successfully"));
  }
  catch(error){
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
  finally{
    yield put(setLoading(false));
  }
}

function* removeFolder(action){
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

function* fetchAllBets(){
  try {
    yield put(setLoading(true));
    let bets = yield call(getAllBetsByUser);
    yield put(fetchBetsSuccess(bets));
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

const getUsedFolder = (state) => {return state.bets.betsFromFolder.folder};

function* fetchBetsFromFolder(action){
  try {
    yield put(setLoading(true));
    let bets = yield call(getBetsFromFolder, action.payload.folder);
    yield put(fetchBetsFromFolderSuccess({folder: action.payload.folder, bets: bets}));
    if (action.callback !== undefined){
      action.callback({folder: action.payload.folder, bets: bets});
    }
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
  finally {
    yield put(setLoading(false));
  }
}

function* fetchBetsFromAllFolders(action){
  if (action.payload !== undefined){
    yield put(setLoading(true));
    let folders = action.payload.folders;
    let betFolders = [];
    try {
      let promises = folders.map((folder) => {
        return getBetsFromFolder(folder);
      });
      yield Promise.all(promises).then(function(results){
        for (var i = 0; i < results.length; i++){
          betFolders.push({folder: folders[i], bets: results[i]});
        }
      });
      yield put(fetchBetsFromAllFoldersSuccess(betFolders));
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
}

function* fetchFinishedBets(){
  try {
    yield put(setLoading(true));
    let bets = yield call(getFinishedBets);
    yield put(fetchFinishedBetsSuccess(bets));
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

function* fetchUnresolvedBets(){
  try {
    yield put(setLoading(true));
    let bets = yield call(getUnresolvedBets);
    yield put(fetchUnresolvedBetsSuccess(bets));
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
  finally {
    yield put(setLoading(false));
  }
}

function* createBet(action){
  try {
    yield put(setLoading(true));
    yield call(postBet, action.payload.bet);
    yield put(setAlertStatus(201, "Bet added successfully"));
    yield call(fetchAllBets);
    let usedFolder = yield select(getUsedFolder);
    if (usedFolder !== ""){
      yield call(fetchBetsFromFolder, usedFolder);
    }
    yield call(fetchUnresolvedBets);
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
  finally {
    yield put(setLoading(false));
  }
}

function* modifyBet(action){
  try {
    yield put(setLoading(true));
    yield call(putBet, action.payload.bet_id, action.payload.data);
    if (action.showAlert){
      yield put(setAlertStatus(204, "Updated successfully"));
    }
    yield call(fetchAllBets);
    let usedFolder = yield select(getUsedFolder);
    if (usedFolder !== ""){
      yield call(fetchBetsFromFolder, usedFolder);
    }
    yield call(fetchUnresolvedBets);
    if (action.callback !== undefined){
      action.callback();
    }
  }
  catch(error){
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
  finally{
    yield put(setLoading(false));
  }
}

function* removeBet(action){
  try {
    yield put(setLoading(true));
    let res = yield call(deleteBet, action.payload.bet_id, action.payload.folders);
    yield put(fetchFoldersOfBetSuccess([]));
    if (res === undefined){
      yield put(setAlertStatus(204, "Bet deleted successfully"));
    }
    yield call(fetchAllBets);
    let usedFolder = yield select(getUsedFolder);
    if (usedFolder !== ""){
      yield call(fetchBetsFromFolder, usedFolder);
    }
    yield call(fetchUnresolvedBets);

    if (action.callback !== undefined){
      action.callback();
    }

  }
  catch(error){
    switch(error){
      case 401:
        yield put(setAlertStatus(error, "Session expired, please login again"));
        break;
      case 0:
        yield put(setAlertStatus(error, "Connection refused, server is likely down"));
        break;
      case 404:
        yield put(setAlertStatus(error, "Bet trying to be deleted was not found in the database"));
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

function* watchFolders(){
  yield takeLatest(FETCH_FOLDERS, fetchFolders);
}

function* watchFoldersOfBet(){
  yield takeLatest(FETCH_FOLDERS_OF_BET, fetchFoldersOfBet);
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

function* watchBetsFromAllFolders(){
  yield takeLatest(FETCH_BETS_FROM_ALL_FOLDERS, fetchBetsFromAllFolders);
}

function* watchFinishedBets(){
  yield takeLatest(FETCH_FINISHED_BETS, fetchFinishedBets);
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

function* watchDeleteBet(){
  yield takeLatest(DELETE_BET, removeBet);
}

function* watchClearAlert(){
  yield call(clearAlert);
}

export default function* rootSaga() {
  yield all([
    watchFolders(),
    watchFoldersOfBet(),
    watchPostFolder(),
    watchDeleteFolder(),
    watchAllBets(),
    watchBetsFromFolder(),
    watchBetsFromAllFolders(),
    watchUnresolvedBets(),
    watchFinishedBets(),
    watchPostBet(),
    watchPutBet(),
    watchDeleteBet(),
    watchClearAlert()
  ]);
}
