import ConstVars from '../Consts.js';

/*
POST-request to get a token to use in the app.

data:{
  username: 'username',
  password: 'password'
}
*/
export function postToken(username, password, callback){
  var xmlHttp = new XMLHttpRequest();

  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      if (xmlHttp.status === 200){
        console.log("got response: " + xmlHttp.responseText);
        callback(xmlHttp.status, JSON.parse(xmlHttp.responseText).token, JSON.parse(xmlHttp.responseText).owner, username);
      }
      else {
        callback(xmlHttp.status, null, null);
      }
    }
  });
  xmlHttp.open("POST", ConstVars.URI + "token");
  xmlHttp.setRequestHeader('Content-Type', 'application/json');
  xmlHttp.setRequestHeader('Authorization', password);
  xmlHttp.send(JSON.stringify({username: username}));
}

/*GET-request to check if token is already in use. If it is, it means user
is logged in, and is redirected to home page. Returns 404 if user hasn't got a token,
and a new one must be requested.*/
export function getToken(token, user_id, callback){
  var xmlHttp = new XMLHttpRequest();
  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      if (xmlHttp.status === 200){
        callback(xmlHttp.status);
      }
    }
  });
  xmlHttp.open("GET", ConstVars.URI + "token?user_id=" + user_id);
  xmlHttp.setRequestHeader('Content-Type', 'application/json');
  xmlHttp.setRequestHeader('Authorization', token);
  xmlHttp.send();
}
