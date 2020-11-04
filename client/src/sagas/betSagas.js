import { call, put, select, takeLatest } from 'redux-saga/effects';
import {fetchBetsSuccess, fetchBetsFromFolderSuccess,
  fetchUnresolvedBetsSuccess, fetchBetsFromAllFoldersSuccess, fetchFinishedBetsSuccess,
  FETCH_BETS, FETCH_BETS_FROM_FOLDER,FETCH_BETS_FROM_ALL_FOLDERS, FETCH_FINISHED_BETS,
  FETCH_UNRESOLVED_BETS, POST_BET, PUT_BET, PUT_BETS, DELETE_BET}
   from '../actions/betsActions';
import {fetchFoldersOfBetSuccess} from '../actions/foldersActions';
import {
  getAllBetsByUser, getBetsFromFolder, getFinishedBets, 
  getUnresolvedBets, postBet, putBet, putBets, deleteBet}
  from '../js/Requests/Bets.js';
import {setLoading} from '../actions/loadingActions';
import {setAlertStatus, setErrorResponseAlertStatus} from '../actions/alertActions';

const getUsedFolder = (state) => {return state.bets.betsFromFolder.folder};

export function* fetchAllBets()
{
  try 
  {
    yield put(setLoading(true));
    let bets = yield call(getAllBetsByUser);
    yield put(fetchBetsSuccess(bets));
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

export function* fetchBetsFromFolder(action)
{
  try 
  {
    yield put(setLoading(true));

    let bets = yield call(getBetsFromFolder, action.payload.folder);

    yield put(fetchBetsFromFolderSuccess({folder: action.payload.folder, bets: bets}));

    if (action.callback !== undefined)
    {
      action.callback({folder: action.payload.folder, bets: bets});
    }
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

export function* fetchBetsFromAllFolders(action)
{
  if (action.payload !== undefined)
  {
    yield put(setLoading(true));
    let folders = action.payload.folders;
    let betFolders = [];

    try 
    {
      let promises = folders.map((folder) => {
        return getBetsFromFolder(folder);
      });
      yield Promise.all(promises).then(function(results){
        for (var i = 0; i < results.length; i++)
        {
          betFolders.push({folder: folders[i], bets: results[i]});
        }
      });
      yield put(fetchBetsFromAllFoldersSuccess(betFolders));
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
}

export function* fetchFinishedBets()
{
  try 
  {
    yield put(setLoading(true));
    let bets = yield call(getFinishedBets);
    yield put(fetchFinishedBetsSuccess(bets));
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

export function* fetchUnresolvedBets()
{
  try 
  {
    yield put(setLoading(true));
    let bets = yield call(getUnresolvedBets);
    yield put(fetchUnresolvedBetsSuccess(bets));
  }
  catch(error)
  {
    yield put(setErrorResponseAlertStatus(error));
  }
  finally {
    yield put(setLoading(false));
  }
}

export function* createBet(action)
{
  try 
  {
    yield put(setLoading(true));
    yield call(postBet, action.payload.bet);
    yield put(setAlertStatus(201, "Bet added successfully"));
    yield call(fetchAllBets);
    let usedFolder = yield select(getUsedFolder);
    if (usedFolder !== "")
    {
      yield call(fetchBetsFromFolder, usedFolder);
    }
    yield call(fetchUnresolvedBets);
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

export function* modifyBets(action)
{
  try 
  {
    yield put(setLoading(true));
    const response = yield call(putBets, action.payload.betIds, action.payload.betsData);

    yield put(setAlertStatus(response.status, response.responseText));

    yield call(fetchAllBets);
    let usedFolder = yield select(getUsedFolder);

    if (usedFolder !== "")
    {
      yield call(fetchBetsFromFolder, usedFolder);
    }
    yield call(fetchUnresolvedBets);
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

export function* modifyBet(action)
{
  try 
  {
    yield put(setLoading(true));
    yield call(putBet, action.payload.betId, action.payload.data);

    if (action.showAlert)
    {
      yield put(setAlertStatus(204, "Updated successfully"));
    }

    yield call(fetchAllBets);
    let usedFolder = yield select(getUsedFolder);

    if (usedFolder !== "")
    {
      yield call(fetchBetsFromFolder, usedFolder);
    }
    yield call(fetchUnresolvedBets);
    if (action.callback !== undefined)
    {
      action.callback();
    }
  }
  catch(error)
  {
    yield put(setErrorResponseAlertStatus(error));
  }
  finally{
    yield put(setLoading(false));
  }
}

export function* removeBet(action){
  try {
    yield put(setLoading(true));
    let res = yield call(deleteBet, action.payload.betId, action.payload.folders);

    yield put(fetchFoldersOfBetSuccess([]));

    if (res === undefined)
    {
      yield put(setAlertStatus(204, "Bet deleted successfully"));
    }

    yield call(fetchAllBets);
    let usedFolder = yield select(getUsedFolder);
    if (usedFolder !== ""){
      yield call(fetchBetsFromFolder, usedFolder);
    }
    yield call(fetchUnresolvedBets);

    if (action.callback !== undefined)
    {
      action.callback();
    }

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

export function* watchAllBets(){
  yield takeLatest(FETCH_BETS, fetchAllBets);
}

export function* watchBetsFromFolder(){
  yield takeLatest(FETCH_BETS_FROM_FOLDER, fetchBetsFromFolder);
}

export function* watchBetsFromAllFolders(){
  yield takeLatest(FETCH_BETS_FROM_ALL_FOLDERS, fetchBetsFromAllFolders);
}

export function* watchFinishedBets(){
  yield takeLatest(FETCH_FINISHED_BETS, fetchFinishedBets);
}

export function* watchUnresolvedBets(){
  yield takeLatest(FETCH_UNRESOLVED_BETS, fetchUnresolvedBets);
}

export function* watchPostBet(){
  yield takeLatest(POST_BET, createBet);
}

export function* watchPutBet(){
  yield takeLatest(PUT_BET, modifyBet);
}

export function* watchPutBets()
{
  yield takeLatest(PUT_BETS, modifyBets);
}

export function* watchDeleteBet(){
  yield takeLatest(DELETE_BET, removeBet);
}
