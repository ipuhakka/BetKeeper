import store from '../store';

export const GET_PAGE = "GET_PAGE";
export const GET_PAGE_SUCCESS = "GET_PAGE_SUCCESS";
export const CALL_ACTION = "CALL_MODAL_ACTION";
export const CALL_ACTION_SUCCESS = "CALL_MODAL_ACTION_SUCCESS";

/**
 * Get page structure.
 */
export function getPage(pathname)
{
    store.dispatch({
        type: GET_PAGE,
        payload: {
            pathname: pathname
          }
      });
}

export function callAction(page, action, requestParameters, callback)
{
    store.dispatch({
        type: CALL_ACTION,
        payload: {
            parameters: requestParameters,
            actionName: action,
            page,
            callback
          }
      });
}

/**
 * Success action for getting page structure.
 * @param {object} structure 
 */
export function getPageSuccess(structure)
{
    store.dispatch({
        type: "GET_PAGE_SUCCESS",
        payload: {
            structure: structure
          }
      });
}