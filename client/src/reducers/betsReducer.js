import * as betsActions from '../actions/betsActions';

const BetsReducer = (state = { allBets: [], betsFromFolder: {folder: "", bets:[]}, betsFromAllFolders:[], finishedBets: [], unresolvedBets: []}, action ) => {
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
      case betsActions.FETCH_BETS_FROM_ALL_FOLDERS:
        return {
          ...state,
          loading: true
        };
      case betsActions.FETCH_BETS_FROM_ALL_FOLDERS_SUCCESS:
        return {
          ...state,
          loading: false,
          betsFromAllFolders: action.betFolders
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
    case betsActions.FETCH_FINISHED_BETS:
      return {
        ...state,
        loading: true
      }
      case betsActions.FETCH_FINISHED_BETS_SUCCESS:
        return {
          ...state,
          loading: false,
          finishedBets: action.bets
        }
    default:
      return state;
  }
};

export default BetsReducer;
