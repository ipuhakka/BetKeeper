import { combineReducers } from 'redux';
import FoldersReducer from './foldersReducer';
import AlertReducer from './alertReducer';

const rootReducer = combineReducers({
  folders: FoldersReducer,
  alert: AlertReducer
});

export default rootReducer;
