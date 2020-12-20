import HttpRequest from './httpRequest';

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
