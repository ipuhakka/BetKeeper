import { call } from 'redux-saga/effects';
import {clearAlert} from '../actions/alertActions';

export function* watchClearAlert(){
  yield call(clearAlert);
}
