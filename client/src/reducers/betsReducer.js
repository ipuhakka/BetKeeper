import * as betsActions from '../actions/betsActions';

const BetsReducer = (state = { allBets: []}, action ) => {
  switch (action.type) {
    case betsActions.FETCH_BETS:
      return {
        ...state,
        loading: true
      };
    case betsActions.FETCH_BETS_SUCCESS:
      return {
        ...state,
        loading: false,
        allBets: action.bets
      };
    default:
      return state;
  }
};

export default BetsReducer;
