import ConstVars from '../Consts.js';

/*
POST-request to get a token to use in the app.

data:{
  username: 'username',
  password: 'password'
}
*/
export function postToken(data, callback){
  var xmlHttp = new XMLHttpRequest();

  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      if (xmlHttp.status === 200){
        callback(xmlHttp.status, JSON.parse(xmlHttp.responseText).token, data.username);
      }
      else {
        callback(xmlHttp.status, null, null);
      }
    }
  });
  xmlHttp.open("POST", ConstVars.URI + "token");
  xmlHttp.setRequestHeader('Content-Type', 'application/json');
  xmlHttp.send(JSON.stringify(data));
}

/*GET-request to check if token is already in use. If it is, it means user
is logged in, and is redirected to home page. Returns 404 if user hasn't got a token,
and a new one must be requested.*/
export function getToken(token, callback){
  var xmlHttp = new XMLHttpRequest();
  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      if (xmlHttp.status === 200){
        callback(xmlHttp.status);
      }
    }
  });
  xmlHttp.open("GET", ConstVars.URI + "token/?token=" + token);
  xmlHttp.setRequestHeader('Content-Type', 'application/json');
  xmlHttp.send();
}
