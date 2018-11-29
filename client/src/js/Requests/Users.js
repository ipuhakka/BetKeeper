import ConstVars from '../Consts.js';

/*
POST-request to uri/users.

Excpected responses:
  201 Created,
  409 Conflict
*/
export function postUser(username, password, callback){
  var xmlHttp = new XMLHttpRequest();

  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      if (xmlHttp.status === 201)
        callback(xmlHttp.status, {username: username, password: password});
      else {
        callback(xmlHttp.status, null);
      }
    }
  });
  xmlHttp.open("POST", ConstVars.URI + "users");
  xmlHttp.setRequestHeader('Content-Type', 'application/json');
  xmlHttp.setRequestHeader('Authorization', password);
  xmlHttp.send(JSON.stringify({username: username}));
}
