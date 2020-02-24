import store from '../store';

export const GET_PAGE = "GET_PAGE";
export const GET_PAGE_SUCCESS = "GET_PAGE_SUCCESS";
export const CALL_MODAL_ACTION = "CALL_MODAL_ACTION";
export const CALL_MODAL_ACTION_SUCCESS = "CALL_MODAL_ACTION_SUCCESS";

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

export function callModalAction(page, action, requestParameters)
{
    store.dispatch({
        type: CALL_MODAL_ACTION,
        payload: {
            parameters: requestParameters,
            actionName: action,
            page
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