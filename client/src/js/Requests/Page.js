import HttpRequest from './httpRequest';

/** Get page structure. */
export function getPage(pathname)
{
  return new HttpRequest(
    `${pathname.substr(1)}`, 
    'GET',
    [
      { key: 'Authorization', value: sessionStorage.getItem('token') },
      { key: 'Content-Type', value: 'application/json'}
    ]).sendRequest();
}

/**
 * Execute an action
 * @param {string} page 
 * @param {string} action 
 * @param {object} parameters 
 */
export function postAction(page, action, parameters)
{
  return new HttpRequest(
    `pageaction/${page.toLowerCase()}/${action}`, 
    'POST',
    [
      { key: 'Authorization', value: sessionStorage.getItem('token') },
      { key: 'Content-Type', value: 'application/json'}
    ],
    JSON.stringify(parameters)).sendRequest();
}

/**
 * Update request based on a dropdownlist value change.
 * @param {object} requestBody 
 * @param {string} pageRoute 
 */
export function handleServerDropdownUpdate(requestBody, pageRoute)
{
  return new HttpRequest(
    `page/handleDropdownUpdate${pageRoute}`, 
    'POST',
    [
      { key: 'Authorization', value: sessionStorage.getItem('token') },
      { key: 'Content-Type', value: 'application/json'}
    ],
    JSON.stringify(requestBody)).sendRequest();
}

/**
 * Sends a request to expand a list group item
 * @param {*} requestBody 
 */
export function expandListGroupItem(requestBody)
{
  return new HttpRequest(
    `page/expandListGroupItem${window.location.pathname.replace('page/', '')}`,
    'POST',
    [
      { key: 'Authorization', value: sessionStorage.getItem('token') },
      { key: 'Content-Type', value: 'application/json'}
    ],
    JSON.stringify(requestBody)).sendRequest();
}