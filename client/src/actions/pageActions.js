import store from '../store';

export const GET_PAGE = "GET_PAGE";
export const GET_PAGE_SUCCESS = "GET_PAGE_SUCCESS";
export const CALL_ACTION = "CALL_MODAL_ACTION";
export const UPDATE_COMPONENTS = "UPDATE_COMPONENTS";
export const UPDATE_DATA = 'UPDATE_DATA';
export const HANDLE_SERVER_DROPDOWN_UPDATE = "HANDLE_SERVER_DROPDOWN_UPDATE";
export const HANDLE_SERVER_DROPDOWN_UPDATE_SUCCESS = "HANDLE_SERVER_DROPDOWN_UPDATE_SUCCESS";
export const DATA_CHANGE = "DATA_CHANGE";

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
 * 
 * @param {string} page 
 * @param {*} newData 
 */
export function updateData(page, newData)
{
    store.dispatch({
        type: UPDATE_DATA,
        payload: {
            page,
            data: newData
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
 * @param {Array} components
 */
export function handleServerDropdownUpdate(key, value, components)
{
    store.dispatch({
        type: HANDLE_SERVER_DROPDOWN_UPDATE,
        payload: {
            key,
            value,
            components
          }
      });
}

/**
 * Handle page data change.
 * @param {string} pageKey Page key.
 * @param {string} dataKeyPath Path for new data key.
 * @param {*} newValue new value to place in datakey path.
 */
export function onDataChange(pageKey, dataKeyPath, newValue)
{
    store.dispatch({
        type: DATA_CHANGE,
        payload: {
            pageKey,
            dataKeyPath,
            newValue
          }
      });
}