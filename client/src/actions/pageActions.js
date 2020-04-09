import store from '../store';

export const GET_PAGE = "GET_PAGE";
export const GET_PAGE_SUCCESS = "GET_PAGE_SUCCESS";
export const CALL_ACTION = "CALL_MODAL_ACTION";
export const UPDATE_COMPONENTS = "UPDATE_COMPONENTS";
export const UPDATE_OPTIONS = "UPDATE_OPTIONS";
export const UPDATE_OPTIONS_SUCCESS = "UPDATE_OPTIONS_SUCCESS";

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
 * Update page components.
 * @param {string} page 
 * @param {object} components 
 */
export function updateComponents(page, components)
{
    store.dispatch({
        type: UPDATE_COMPONENTS,
        payload: {
            page,
            components
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

/**
 * Updates dropdown opions tied to key field.
 * @param {string} key 
 * @param {string} value
 */
export function updateOptions(key, value)
{
    store.dispatch({
        type: UPDATE_OPTIONS,
        payload: {
            key,
            value
          }
      });
}

export function updateOptionsSuccess(response)
{
    store.dispatch({
        type: UPDATE_OPTIONS_SUCCESS,
        payload: {
            response
        }
    })
}