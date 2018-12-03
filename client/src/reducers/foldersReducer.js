import * as foldersActions from '../actions/foldersActions';

const FoldersReducer = (state = { folders: [] }, action ) => {
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
    default:
      return state;
  }
};

export default FoldersReducer;
