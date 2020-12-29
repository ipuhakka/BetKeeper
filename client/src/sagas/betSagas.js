import { call, put, takeLatest } from 'redux-saga/effects';
import {fetchBetsSuccess, fetchBetsFromFolderSuccess,
  fetchUnresolvedBetsSuccess, fetchBetsFromAllFoldersSuccess, fetchFinishedBetsSuccess,
  FETCH_BETS, FETCH_BETS_FROM_FOLDER,FETCH_BETS_FROM_ALL_FOLDERS, FETCH_FINISHED_BETS,
  FETCH_UNRESOLVED_BETS}
   from '../actions/betsActions';
import {
  getAllBetsByUser, getBetsFromFolder, getFinishedBets, 
  getUnresolvedBets}
  from '../js/Requests/Bets.js';
import { withErrorResponseHandler, withLoading } from './helperSagas';

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