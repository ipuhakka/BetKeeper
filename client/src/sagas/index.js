import { all } from 'redux-saga/effects';
import * as alertSagas from './alertSagas';
import * as sessionSagas from './sessionSagas';
import * as pageSagas from './pageSagas';

export default function* rootSaga() {
  yield all([
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
