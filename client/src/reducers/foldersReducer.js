import * as foldersActions from '../actions/foldersActions';

const FoldersReducer = (state = { folders: [], foldersOfBet: []}, action ) => {
  switch (action.type) {
    case foldersActions.FETCH_FOLDERS:
      return {
        ...state
      };
    case foldersActions.FETCH_FOLDERS_SUCCESS:
      return {
        ...state,
        folders: action.folders
      };
    case foldersActions.FETCH_FOLDERS_OF_BET:
      return {
        ...state
      }
    case foldersActions.FETCH_FOLDERS_OF_BET_SUCCESS:
      return {
        ...state,
        foldersOfBet: action.folders
      }
    default:
      return state;
  }
};

export default FoldersReducer;
