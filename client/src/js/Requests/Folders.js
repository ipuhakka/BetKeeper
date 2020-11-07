import HttpRequest from './httpRequest';

/*
  POST-request to uri/folders. Resolves on 201 Created response,
  rejects on any other response status, with given status.
*/
export function postFolder(folder)
{
  return new HttpRequest(
    'folders', 
    'POST',
    [
      { key: 'Authorization', value: sessionStorage.getItem('token') },
      { key: 'Content-Type', value: 'application/json'}
    ],
    JSON.stringify(folder)).sendRequest();
}

/*
  DELETE-request to uri/folders?folder={folder}.
  Resolves on 204 No content response,
  rejects on any other response status, with given status.
*/
export function deleteFolder(folder)
{
  return new HttpRequest(
    'folders/' + folder, 
    'DELETE',
    [
      { key: 'Authorization', value: sessionStorage.getItem('token') }
    ]).sendRequest();
}

/*
  GET-request for getting folders for logged user.
  Resolves on 200 OK response,
  rejects on any other response status, with given status.
*/
export function getFolders()
{
  return new HttpRequest(
    'folders/', 
    'GET',
    [
      { key: 'Authorization', value: sessionStorage.getItem('token') }
    ]).sendRequest();
}

/*
GET-request to get folders of selected bet.
Resolves on 200 OK response,
rejects on any other response status, with given status.
*/
export function getFoldersOfBet(id)
{
  return new HttpRequest(
    'folders?betId=' + id, 
    'GET',
    [
      { key: 'Authorization', value: sessionStorage.getItem('token') }
    ]).sendRequest();
}
