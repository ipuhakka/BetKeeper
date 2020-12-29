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
