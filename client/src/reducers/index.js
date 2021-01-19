import { combineReducers } from 'redux';
import AlertReducer from './alertReducer';
import LoadingReducer from './loadingReducer';
import PageReducer from './pageReducer';

const appReducer = combineReducers({
  alert: AlertReducer,
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
