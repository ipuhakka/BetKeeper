import { all, takeLatest, call, put } from 'redux-saga/effects';
import {FETCH_FOLDERS, fetchFoldersSuccess} from './actions/foldersActions';
import {getFolders} from './js/Requests/Folders.js';


function* fetchFolders(action){
  let folders = yield call(getFolders);
  yield put(fetchFoldersSuccess(folders));
  action.callback();
}

function* watchFolders(){
  yield takeLatest(FETCH_FOLDERS, fetchFolders);
}

export default function* rootSaga() {
  yield all([
    watchFolders()
  ]);
}
