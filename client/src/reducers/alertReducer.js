import * as alertActions from '../actions/alertActions';

const AlertReducer = (state = { status: null, statusMessage: ""}, action ) => {
  switch (action.type) {
    case alertActions.SET_ALERT_STATUS:
      return {
        ...state,
        status: action.status,
        statusMessage: action.message
      };
    case alertActions.CLEAR_ALERT:
      return {
        ...state,
        status: null,
        statusMessage: ""
      }
    default:
      return state;
  }
};

export default AlertReducer;
