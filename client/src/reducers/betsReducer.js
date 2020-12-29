import _ from 'lodash';
import * as betsActions from '../actions/betsActions';
import { formatDateTime } from '../js/utils';

/**
 * Processes played date values. Converts utc date to local time.
 * @param {*} bets 
 */
function processBets(bets)
{
  return _.forEach(bets, bet => {
    bet.playedDate = formatDateTime(bet.playedDate);

    return bet;
  });
}

const BetsReducer = (state = { allBets: [], betsFromFolder: {folder: "", bets:[]}, betsFromAllFolders:[], finishedBets: [], unresolvedBets: []}, action ) => {
  switch (action.type) 
  {
    case betsActions.FETCH_BETS:
      return {
        ...state
      };

    case betsActions.FETCH_BETS_SUCCESS:
      return {
        ...state,
        allBets: processBets(action.bets)
      };

    case betsActions.FETCH_BETS_FROM_FOLDER:
      return {
        ...state
      };

    case betsActions.FETCH_BETS_FROM_FOLDER_SUCCESS:
      return {
        ...state,
        betsFromFolder: {
          folder: action.bets.folder,
          bets: processBets(action.bets.bets)
        }
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
        unresolvedBets: processBets(action.bets)
      }

    case betsActions.FETCH_FINISHED_BETS:
      return {
        ...state,
      }

    case betsActions.FETCH_FINISHED_BETS_SUCCESS:
      return {
        ...state,
        finishedBets: processBets(action.bets)
      }

    default:
      return state;
  }
};

export default BetsReducer;
