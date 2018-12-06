import * as foldersActions from '../actions/foldersActions';

const FoldersReducer = (state = { folders: [], foldersOfBet: []}, action ) => {
  switch (action.type) {
    case foldersActions.FETCH_FOLDERS:
      return {
        ...state,
        loading: true
      };
    case foldersActions.FETCH_FOLDERS_SUCCESS:
      return {
        ...state,
        loading: false,
        folders: action.folders
      };
    case foldersActions.FETCH_FOLDERS_OF_BET:
      return {
        ...state,
        loading: true
      }
    case foldersActions.FETCH_FOLDERS_OF_BET_SUCCESS:
      return {
        ...state,
        loading: false,
        foldersOfBet: action.folders
      }
    default:
      return state;
  }
};

export default FoldersReducer;
