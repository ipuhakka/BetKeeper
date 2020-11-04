export const SET_ALERT_STATUS = "SET_ALERT_STATUS";
export const SET_ERROR_RESPONSE_ALERT_STATUS = 'SET_ERROR_RESPONSE_ALERT_STATUS';
export const CLEAR_ALERT = "CLEAR_ALERT";

export const setAlertStatus = (status, message) => ({
  type: "SET_ALERT_STATUS",
  status, message
});

/**
 * Set alert status from an error http response
 * @param {XMLHttpResponse} error 
 */
export const setErrorResponseAlertStatus = (error) => ({
  type: 'SET_ERROR_RESPONSE_ALERT_STATUS',
  payload: error
});

export const clearAlert = () => ({
  type: "CLEAR_ALERT"
});
