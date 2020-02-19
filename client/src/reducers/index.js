import { combineReducers } from 'redux';
import FoldersReducer from './foldersReducer';
import AlertReducer from './alertReducer';
import BetsReducer from './betsReducer';
import LoadingReducer from './loadingReducer';
import PageReducer from './pageReducer';

const rootReducer = combineReducers({
  folders: FoldersReducer,
  alert: AlertReducer,
  bets: BetsReducer,
  loading: LoadingReducer,
  pages: PageReducer
});

export default rootReducer;
