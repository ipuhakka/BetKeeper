import { combineReducers } from 'redux';
import FoldersReducer from './foldersReducer';

const rootReducer = combineReducers({
  folders: FoldersReducer
});

export default rootReducer;
