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
import {setAlertStatus} from '../actions/alertActions';
import { withErrorResponseHandler, withLoading } from './helperSagas';

const getUsedFolder = (state) => {return state.bets.betsFromFolder.folder};

export function* fetchAllBets()
{
  yield call(withLoading, function*() 
  { 
    yield call(withErrorResponseHandler, function*()
    {
      const response = yield call(getAllBetsByUser);
      const bets = JSON.parse(response.responseText);
      yield put(fetchBetsSuccess(bets));
    }); 
  });
}

export function* fetchBetsFromFolder(action)
{
  yield call(withLoading, function*() 
  { 
    yield call(withErrorResponseHandler, function*()
    {
      const response = yield call(getBetsFromFolder, action.payload.folder);
      const bets = JSON.parse(response.responseText);

      yield put(fetchBetsFromFolderSuccess({folder: action.payload.folder, bets: bets}));
  
      if (action.callback !== undefined)
      {
        action.callback({folder: action.payload.folder, bets: bets});
      }
    }); 
  });
}

export function* fetchBetsFromAllFolders(action)
{

  if (action.payload === undefined || action.payload === null)
  {
    return;
  }

  yield call(withLoading, function*() 
  { 
    yield call(withErrorResponseHandler, function*()
    {
      let folders = action.payload.folders;
      let betFolders = [];
  
      let promises = folders.map((folder) => {
        return getBetsFromFolder(folder);
      });

      yield Promise.all(promises).then(function(results)
      {
        for (var i = 0; i < results.length; i++)
        {
          const bets = JSON.parse(results[i].responseText);
          betFolders.push({folder: folders[i], bets: bets});
        }
      });
      yield put(fetchBetsFromAllFoldersSuccess(betFolders));
    }); 
  });
}

export function* fetchFinishedBets()
{
  yield call(withLoading, function*() 
  { 
    yield call(withErrorResponseHandler, function*()
    {
      const response = yield call(getFinishedBets);
      const bets = JSON.parse(response.responseText);

      yield put(fetchFinishedBetsSuccess(bets));
    }); 
  });
}

export function* fetchUnresolvedBets()
{
  yield call(withLoading, function*() 
  { 
    yield call(withErrorResponseHandler, function*()
    {
      const response = yield call(getUnresolvedBets);
      const bets = JSON.parse(response.responseText);

      yield put(fetchUnresolvedBetsSuccess(bets));
    }); 
  });
}

export function* createBet(action)
{
  yield call(withLoading, function*() 
  { 
    yield call(withErrorResponseHandler, function*()
    {
      yield call(postBet, action.payload.bet);
      yield put(setAlertStatus(201, "Bet added successfully"));
      yield call(fetchAllBets);
      let usedFolder = yield select(getUsedFolder);
      if (usedFolder !== "")
      {
        yield call(fetchBetsFromFolder, usedFolder);
      }
      yield call(fetchUnresolvedBets);
    }); 
  });
}

export function* modifyBets(action)
{
  yield call(withLoading, function*() 
  { 
    yield call(withErrorResponseHandler, function*()
    {
      const response = yield call(putBets, action.payload.betIds, action.payload.betsData);

      yield put(setAlertStatus(response.status, response.responseText));
  
      yield call(fetchAllBets);
      let usedFolder = yield select(getUsedFolder);
  
      if (usedFolder !== "")
      {
        yield call(fetchBetsFromFolder, usedFolder);
      }
      yield call(fetchUnresolvedBets);
    }); 
  });
}

export function* modifyBet(action)
{
  yield call(withLoading, function*() 
  { 
    yield call(withErrorResponseHandler, function*()
    {
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
    }); 
  });
}

export function* removeBet(action)
{
  yield call(withLoading, function*() 
  { 
    yield call(withErrorResponseHandler, function*()
    {
      yield call(deleteBet, action.payload.betId, action.payload.folders);

      yield put(fetchFoldersOfBetSuccess([]));
  
      yield put(setAlertStatus(204, "Bet deleted successfully"));
  
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
    }); 
  });
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
