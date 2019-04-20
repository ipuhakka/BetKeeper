import * as betsActions from '../actions/betsActions';

const BetsReducer = (state = { allBets: [], betsFromFolder: {folder: "", bets:[]}, betsFromAllFolders:[], finishedBets: [], unresolvedBets: []}, action ) => {
  switch (action.type) {
    case betsActions.FETCH_BETS:
      return {
        ...state
      };
    case betsActions.FETCH_BETS_SUCCESS:
      return {
        ...state,
        allBets: action.bets
      };
    case betsActions.FETCH_BETS_FROM_FOLDER:
      return {
        ...state
      };
    case betsActions.FETCH_BETS_FROM_FOLDER_SUCCESS:
      return {
        ...state,
        betsFromFolder: action.bets
      };
      case betsActions.FETCH_BETS_FROM_ALL_FOLDERS:
        return {
          ...state
        };
      case betsActions.FETCH_BETS_FROM_ALL_FOLDERS_SUCCESS:
        return {
          ...state,
          betsFromAllFolders: action.betFolders
        };
    case betsActions.FETCH_UNRESOLVED_BETS:
      return {
        ...state
      }
    case betsActions.FETCH_UNRESOLVED_BETS_SUCCESS:
      return {
        ...state,
        unresolvedBets: action.bets
      }
    case betsActions.FETCH_FINISHED_BETS:
      return {
        ...state,
      }
    case betsActions.FETCH_FINISHED_BETS_SUCCESS:
      return {
        ...state,
        finishedBets: action.bets
      }
    case betsActions.POST_BET:
      return {
        ...state
      }
    default:
      return state;
  }
};

export default BetsReducer;
