import store from '../store';

export const GET_PAGE = "GET_PAGE";
export const GET_PAGE_SUCCESS = "GET_PAGE_SUCCESS";

/**
 * Get page structure.
 */
export function getPage(pathname)
{
    store.dispatch({
        type: "GET_PAGE",
        payload: {
            pathname: pathname
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