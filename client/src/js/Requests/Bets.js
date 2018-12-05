import ConstVars from '../Consts.js';

/*
GET-request to uri/bets?finished=true.
Gets unfinished bets by logged user.
*/
export function getFinishedBets(callback){
  var xmlHttp = new XMLHttpRequest();

  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      if ([200, 401].includes(xmlHttp.status))
        callback(xmlHttp.status, xmlHttp.responseText);
    }
  });
  xmlHttp.open("GET", ConstVars.URI + 'bets?finished=true');
  xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
  xmlHttp.send();
}

/*
  GET-request to uri/bets?finished=false.
  Gets unfinished bets by logged user.
*/
export function getUnresolvedBets(callback){
  var xmlHttp = new XMLHttpRequest();

  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      if ([200, 401].includes(xmlHttp.status))
        callback(xmlHttp.status, xmlHttp.responseText);
    }
  });
  xmlHttp.open("GET", ConstVars.URI + "bets?finished=false");
  xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
  xmlHttp.send();
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
export function getAllBetsByUser(){
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
*/
export function deleteBet(bet_id, folders, callback){
  var uri = ConstVars.URI + "bets/" + bet_id;

  if (folders.length > 0){
    uri = uri + '?folders=' + JSON.stringify(folders);
  }
  var xmlHttp = new XMLHttpRequest();

  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      if ([204, 401, 404].includes(xmlHttp.status))
        callback(xmlHttp.status);
      else if (xmlHttp.status === 200){
        callback(xmlHttp.status, xmlHttp.responseText);
      }
    }
  });
  xmlHttp.open("DELETE", uri);
  xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
  xmlHttp.send();
}

/*
POST-request to create a new bet to the database.
  var data = {
    bet_won: nullable boolean, null=bet not played, false=bet lost, true=bet won
    odd: decimal value indicating odd for the bet.
    bet: decimal, stake for the bet.
    name: string, optional name to identify the bet.
    folders: optional array, names of folders to which bet is added.
  }
*/
export function postBet(data, callback){
  var xmlHttp = new XMLHttpRequest();

  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      if ([201, 400, 401].includes(xmlHttp.status))
        callback(xmlHttp.status);
    }
  });
  xmlHttp.open("POST", ConstVars.URI + "bets/");
  xmlHttp.setRequestHeader('Content-type', 'application/json');
  xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
  xmlHttp.send(JSON.stringify(data));
}

/*
PUT-request to create a new bet to the database.
  var data = {
    bet_won: number, -1: bet not resolved, 0: bet lost, 1: bet won
  }
*/
export function putBet(bet_id, data, callback){
  var xmlHttp = new XMLHttpRequest();

  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      if ([204, 400, 401, 404, 409].includes(xmlHttp.status))
        callback(xmlHttp.status);
    }
  });
  xmlHttp.open("PUT", ConstVars.URI + "bets/" + bet_id);
  xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
  xmlHttp.setRequestHeader('Content-type', 'application/json');
  xmlHttp.send(JSON.stringify(data));
}
