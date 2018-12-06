import * as betsActions from '../actions/betsActions';

const BetsReducer = (state = { allBets: [], betsFromFolder: {folder: "", bets:[]}, unresolvedBets: []}, action ) => {
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
    case betsActions.FETCH_BETS_FROM_FOLDER:
      return {
        ...state,
        loading: true
      };
    case betsActions.FETCH_BETS_FROM_FOLDER_SUCCESS:
      return {
        ...state,
        loading: false,
        betsFromFolder: action.bets
      };
    case betsActions.FETCH_UNRESOLVED_BETS:
      return {
        ...state,
        loading: true
      }
    case betsActions.FETCH_UNRESOLVED_BETS_SUCCESS:
      return {
        ...state,
        loading: false,
        unresolvedBets: action.bets
      }
    default:
      return state;
  }
};

export default BetsReducer;
