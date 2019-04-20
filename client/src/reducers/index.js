import { combineReducers } from 'redux';
import FoldersReducer from './foldersReducer';
import AlertReducer from './alertReducer';
import BetsReducer from './betsReducer';
import LoadingReducer from './loadingReducer';

const rootReducer = combineReducers({
  folders: FoldersReducer,
  alert: AlertReducer,
  bets: BetsReducer,
  loading: LoadingReducer
});

export default rootReducer;
