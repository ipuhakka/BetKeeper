import ConstVars from '../Consts.js';

/*
POST-request to uri/user.

Excpected responses:
  201 Created,
  409 Conflict
*/
export function postUser(data, callback){
  var xmlHttp = new XMLHttpRequest();

  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      if (xmlHttp.status === 201)
        callback(xmlHttp.status, data);
      else {
        callback(xmlHttp.status, null);
      }
    }
  });
  xmlHttp.open("POST", ConstVars.URI + "user/");
  xmlHttp.setRequestHeader('Content-Type', 'application/json');
  xmlHttp.send(JSON.stringify(data));
}
