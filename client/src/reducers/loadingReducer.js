import * as loadingActions from '../actions/loadingActions';

/*
* Sets loading state. State is loading when ongoingLoads length is greater than 0.
*/
const LoadingReducer = (state = { loading: false, ongoingLoads: [] }, action ) => {
  switch(action.type){
    case loadingActions.SET_LOADING:
      let ongoingLoads = state.ongoingLoads;

      if (action.isLoading){
        ongoingLoads.push(true);
      }
      else {
        ongoingLoads.pop();
      }

      return {
        ...state,
        ongoingLoads: ongoingLoads,
        loading: ongoingLoads.length > 0
      }
    default:
      return state;
  }
};

export default LoadingReducer;
