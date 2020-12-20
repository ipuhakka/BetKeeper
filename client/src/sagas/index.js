import { all } from 'redux-saga/effects';
import * as betSagas from './betSagas';
import * as folderSagas from './folderSagas';
import * as alertSagas from './alertSagas';
import * as sessionSagas from './sessionSagas';
import * as pageSagas from './pageSagas';

export default function* rootSaga() {
  yield all([
    folderSagas.watchFolders(),
    folderSagas.watchFoldersOfBet(),
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
    sessionSagas.watchSignUp(),
    pageSagas.watchGetPage(),
    pageSagas.watchCallModalAction(),
    pageSagas.watchHandleServerDropdownUpdate()
  ]);
}
