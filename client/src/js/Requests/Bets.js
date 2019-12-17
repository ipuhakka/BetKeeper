import ConstVars from '../consts.js';
import _ from 'lodash';

/*
GET-request to uri/bets?finished=true.
Gets unfinished bets by logged user.

Resolved on response with status 200 OK with bets array as parameter,
rejects on any other response, with response
status as parameter.
*/
export function getFinishedBets()
{
  return new Promise(function(resolve, reject)
  {
    var xmlHttp = new XMLHttpRequest();

    xmlHttp.onreadystatechange =( () => 
    {
      if (xmlHttp.readyState === 4)
      {
        if (xmlHttp.status === 200)
        {
          resolve(JSON.parse(xmlHttp.responseText));
        }
        else 
        {
          reject(xmlHttp.status);
        }
      }
    });

    xmlHttp.open("GET", ConstVars.URI + 'bets?finished=true');

    xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));

    xmlHttp.send();
  });
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
  return new Promise(function(resolve, reject)
  {
    var xmlHttp = new XMLHttpRequest();

    xmlHttp.onreadystatechange =( () => 
    {
      if (xmlHttp.readyState === 4)
      {
        if (xmlHttp.status === 200)
        {
          resolve(JSON.parse(xmlHttp.responseText));
        }
        else 
        {
          reject(xmlHttp.status);
        }
      }
    });

    xmlHttp.open("GET", ConstVars.URI + "bets?finished=false");

    xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));

    xmlHttp.send();
  });
}

/*
GET-request to get bets from specific folder made by user.
Resolved on response with status 200 OK with bets array as parameter,
rejects on any other response, with response
status as parameter.
*/
export function getBetsFromFolder(folder){
  return new Promise(function(resolve, reject){
    var xmlHttp = new XMLHttpRequest();

    xmlHttp.onreadystatechange =( () => {
      if (xmlHttp.readyState === 4){
        if (xmlHttp.status === 200){
          resolve(JSON.parse(xmlHttp.responseText));
        }
        else {
          reject(xmlHttp.status);
        }
      }
    });
    xmlHttp.open("GET", ConstVars.URI + "bets?folder=" + folder);
    xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
    xmlHttp.send();
  });
}

/*
GET-request to get all bets made by user.
Resolved on response with status 200 OK with bets array as parameter,
rejects on any other response, with response
status as parameter.
*/
export function getAllBetsByUser()
{
  return new Promise(function(resolve, reject)
  {
    var xmlHttp = new XMLHttpRequest();

    xmlHttp.onreadystatechange =( () => 
    {
      if (xmlHttp.readyState === 4)
      {
        if (xmlHttp.status === 200)
        {
          resolve(JSON.parse(xmlHttp.responseText));
        }
        else 
        {
          reject(xmlHttp.status);
        }
      }
    });
    xmlHttp.open("GET", ConstVars.URI + "bets/");
    xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
    xmlHttp.send();
  });
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
export function deleteBet(betId, folders){
  return new Promise(function(resolve, reject){
    var uri = ConstVars.URI + "bets/" + betId;

    if (folders.length > 0){
      uri = uri + '?folders=' + folders;
    }
    var xmlHttp = new XMLHttpRequest();

    xmlHttp.onreadystatechange =( () => {
      if (xmlHttp.readyState === 4){
        if (xmlHttp.status === 204){
          resolve();
        }
        else if (xmlHttp.status === 200){
          resolve(JSON.parse(xmlHttp.responseText));
        }
        else {
          reject(xmlHttp.status);
        }
      }
    });
    xmlHttp.open("DELETE", uri);
    xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
    xmlHttp.send();
  });
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
  return new Promise(function(resolve, reject)
  {
    var xmlHttp = new XMLHttpRequest();

    xmlHttp.onreadystatechange =( () => 
    {
      if (xmlHttp.readyState === 4)
      {
        if (xmlHttp.status === 201)
        {
          resolve();
        }
        else 
        {
          reject(xmlHttp.status);
        }
      }
    });

    xmlHttp.open("POST", ConstVars.URI + "bets/");
    xmlHttp.setRequestHeader('Content-type', 'application/json');
    xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
    xmlHttp.send(JSON.stringify(data));
  });
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
export function putBet(betId, data){
  return new Promise(function(resolve, reject){
    var xmlHttp = new XMLHttpRequest();

    xmlHttp.onreadystatechange =( () => {
      if (xmlHttp.readyState === 4){
        if (xmlHttp.status === 200){
          resolve();
        }
        else {
          reject(xmlHttp.status);
        }
      }
    });
    xmlHttp.open("PUT", ConstVars.URI + "bets/" + betId);
    xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
    xmlHttp.setRequestHeader('Content-type', 'application/json');
    xmlHttp.send(JSON.stringify(data));
  });
}

/**
 * Mass update bets.
 * @param {array} betIds
 * @param {object} data
 */
export function putBets(betIds, data){
  return new Promise(function(resolve, reject){
    var xmlHttp = new XMLHttpRequest();

    xmlHttp.onreadystatechange =( () => {
      if (xmlHttp.readyState === 4)
      {
        if (xmlHttp.status === 200)
        {
          resolve(xmlHttp);
        }
        else 
        {
          reject(xmlHttp);
        }
      }
    });

    xmlHttp.open("PUT", `${ConstVars.URI}bets?betIds=${_.join(betIds, '&betIds=')}`);
    xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
    xmlHttp.setRequestHeader('Content-type', 'application/json');
    xmlHttp.send(JSON.stringify(data));
  });
}
