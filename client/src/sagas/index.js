import { all } from 'redux-saga/effects';
import * as betSagas from './betSagas.js';
import * as folderSagas from './folderSagas.js';
import * as alertSagas from './alertSagas.js';
import * as sessionSagas from './sessionSagas.js';

export default function* rootSaga() {
  yield all([
    folderSagas.watchFolders(),
    folderSagas.watchFoldersOfBet(),
    folderSagas.watchPostFolder(),
    folderSagas.watchDeleteFolder(),
    betSagas.watchAllBets(),
    betSagas.watchBetsFromFolder(),
    betSagas.watchBetsFromAllFolders(),
    betSagas.watchUnresolvedBets(),
    betSagas.watchFinishedBets(),
    betSagas.watchPostBet(),
    betSagas.watchPutBet(),
    betSagas.watchDeleteBet(),
    betSagas.watchPutBets(),
    alertSagas.watchClearAlert(),
    sessionSagas.watchLogin(),
    sessionSagas.watchLogOut(),
    sessionSagas.watchCheckLogin(),
    sessionSagas.watchSignUp()
  ]);
}
