import _ from 'lodash';
import HttpRequest from './httpRequest';

/*
GET-request to uri/bets?finished=true.
Gets unfinished bets by logged user.

Resolved on response with status 200 OK with bets array as parameter,
rejects on any other response, with response
status as parameter.
*/
export function getFinishedBets()
{
  return new HttpRequest(
    'bets?finished=true', 
    'GET',
    [
      { key: 'Authorization', value: sessionStorage.getItem('token') }
    ]).sendRequest();
}

/*
  GET-request to uri/bets?finished=false.
  Gets unfinished bets by logged user.
  Resolved on response with status 200 OK with bets array as parameter,
  rejects on any other response, with response
  status as parameter.
*/
export function getUnresolvedBets()
{
  return new HttpRequest(
    'bets?finished=false', 
    'GET',
    [
      { key: 'Authorization', value: sessionStorage.getItem('token') }
    ]).sendRequest();
}

/*
GET-request to get bets from specific folder made by user.
Resolved on response with status 200 OK with bets array as parameter,
rejects on any other response, with response
status as parameter.
*/
export function getBetsFromFolder(folder)
{
  return new HttpRequest(
    'bets?folder=' + folder, 
    'GET',
    [
      { key: 'Authorization', value: sessionStorage.getItem('token') }
    ]).sendRequest();
}

/*
GET-request to get all bets made by user.
Resolved on response with status 200 OK with bets array as parameter,
rejects on any other response, with response
status as parameter.
*/
export function getAllBetsByUser()
{
  return new HttpRequest(
    'bets/', 
    'GET',
    [
      { key: 'Authorization', value: sessionStorage.getItem('token') }
    ]).sendRequest();
}

/*
DELETE-request. Deletes bet only from selected folders if there are any,
otherwise deletes the bet completely.

If bet is deleted only from selected folders, application
returns a list of folder names from which bet was deleted.

Resolved on response with status 204 No content,
and with 200 OK if bet was only deleted from specified folders.
In such a case, returns array of folders from which bet was deleted.
Rejects on any other response, with response status as parameter.
*/
export function deleteBet(betId, folders)
{
  let url = 'bets/' + betId;

  if (folders.length > 0)
  {
    url = url + '?folders=' + folders;
  }

  return new HttpRequest(
    url, 
    'DELETE',
    [
      { key: 'Authorization', value: sessionStorage.getItem('token') }
    ]).sendRequest();
}

/*
POST-request to create a new bet to the database.
  var data = {
    beResult: number, -1: bet not resolved, 0: bet lost, 1: bet won
    odd: decimal value indicating odd for the bet.
    bet: decimal, stake for the bet.
    name: string, optional name to identify the bet.
    folders: optional array, names of folders to which bet is added.
  }

  Resolved on response with status 201 Created,
  rejects on any other response, with response
  status as parameter.
*/
export function postBet(data)
{
  return new HttpRequest(
    'bets/', 
    'POST',
    [
      { key: 'Authorization', value: sessionStorage.getItem('token') },
      { key: 'Content-type', value: 'application/json' }
    ],
    JSON.stringify(data)).sendRequest();
}

/*
PUT-request to create a new bet to the database.
  var data = {
    betResult: number, -1: bet not resolved, 0: bet lost, 1: bet won
  }

  Resolved on response with status 200 OK,
  rejects on any other response, with response
  status as parameter.
*/
export function putBet(betId, data)
{
  return new HttpRequest(
    'bets/' + betId, 
    'PUT',
    [
      { key: 'Authorization', value: sessionStorage.getItem('token') },
      { key: 'Content-type', value: 'application/json' }
    ],
    JSON.stringify(data)).sendRequest();
}

/**
 * Mass update bets.
 * @param {array} betIds
 * @param {object} data
 */
export function putBets(betIds, data)
{
  return new HttpRequest(
    `bets?betIds=${_.join(betIds, '&betIds=')}`, 
    'PUT',
    [
      { key: 'Authorization', value: sessionStorage.getItem('token') },
      { key: 'Content-type', value: 'application/json' }
    ],
    JSON.stringify(data)).sendRequest();
}
