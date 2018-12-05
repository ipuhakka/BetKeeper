import { combineReducers } from 'redux';
import FoldersReducer from './foldersReducer';
import AlertReducer from './alertReducer';
import BetsReducer from './betsReducer';

const rootReducer = combineReducers({
  folders: FoldersReducer,
  alert: AlertReducer,
  bets: BetsReducer
});

export default rootReducer;
