export const SET_ALERT_STATUS = "SET_ALERT_STATUS";
export const CLEAR_ALERT = "CLEAR_ALERT";

export const setAlertStatus = (status, message) => ({
  type: "SET_ALERT_STATUS",
  status, message
});

export const clearAlert = () => ({
  type: "CLEAR_ALERT"
});
