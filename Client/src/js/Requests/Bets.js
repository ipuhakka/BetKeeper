import ConstVars from '../Consts.js';

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

export function getBetsFromFolder(folder, callback){
  var xmlHttp = new XMLHttpRequest();

  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      if ([200, 401].includes(xmlHttp.status))
        callback(xmlHttp.status, xmlHttp.responseText);
    }
  });
  xmlHttp.open("GET", ConstVars.URI + "bets?folder=" + folder);
  xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
  xmlHttp.send();
}

export function getAllBetsByUser(callback){
  var xmlHttp = new XMLHttpRequest();

  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      if ([200, 401].includes(xmlHttp.status))
        callback(xmlHttp.status, xmlHttp.responseText);
      }
  });
  xmlHttp.open("GET", ConstVars.URI + "bets/");
  xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
  xmlHttp.send();
}
