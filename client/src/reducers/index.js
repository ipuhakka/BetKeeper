import { combineReducers } from 'redux';
import FoldersReducer from './foldersReducer';
import AlertReducer from './alertReducer';
import BetsReducer from './betsReducer';
import LoadingReducer from './loadingReducer';
import PageReducer from './pageReducer';

const appReducer = combineReducers({
  folders: FoldersReducer,
  alert: AlertReducer,
  bets: BetsReducer,
  loading: LoadingReducer,
  pages: PageReducer
});

const rootReducer = (state, action) => 
{
  if (action.type === 'LOGOUT') 
  {
    state = undefined;
  }

  return appReducer(state, action);
}

export default rootReducer;
