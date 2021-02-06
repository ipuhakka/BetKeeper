import * as alertActions from '../actions/alertActions';

function handleErrorResponseAlert(state, errorResponse)
{
  let status, message;

  switch (errorResponse.status)
  {
      case 400:
        status = errorResponse.status;
        message = errorResponse.responseText || 'Bad request';
        break;
      case 401:
        status = errorResponse.status;
        message = errorResponse.responseText || "Session expired, please login again";
        break;
      case 0:
        status = errorResponse.status;
        message = 'Server error';
        break;
      case 404:
        status = errorResponse.status;
        message = errorResponse.responseText || "Not found";
        break;
      case 409:
        status = errorResponse.status;
        message = errorResponse.responseText || 'Conflict';
        break;
      default:
        status = errorResponse.status;
        message = 'Unexpected error occurred';
        break;
  }

  return {
    ...state,
    status: status,
    statusMessage: message
  }
}

const AlertReducer = (state = { status: null, statusMessage: ""}, action ) => {
  switch (action.type) {
    case alertActions.SET_ALERT_STATUS:
      return {
        ...state,
        status: action.status,
        statusMessage: action.message
      };

    case alertActions.SET_ERROR_RESPONSE_ALERT_STATUS:
      return handleErrorResponseAlert(state, action.payload);

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
