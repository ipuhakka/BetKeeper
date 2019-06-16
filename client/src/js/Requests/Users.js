import ConstVars from '../consts.js';

/*
POST-request to uri/users.

Excpected responses:
  201 Created,
  409 Conflict

  Resolves with data object with
  username and password.
  Rejects with status of the response.
*/
export function postUser(username, password, callback){
  return new Promise(function(resolve, reject){
    var xmlHttp = new XMLHttpRequest();

    xmlHttp.onreadystatechange =( () => {
      if (xmlHttp.readyState === 4){
        if (xmlHttp.status === 201)
          resolve({username: username, password: password});
        else {
          reject(xmlHttp.status);
        }
      }
    });
    xmlHttp.open("POST", ConstVars.URI + "users");
    xmlHttp.setRequestHeader('Content-Type', 'application/json');
    xmlHttp.setRequestHeader('Authorization', password);
    xmlHttp.send(JSON.stringify({username: username}));
  });
}
