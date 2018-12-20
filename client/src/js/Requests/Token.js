import ConstVars from '../Consts.js';

/*
POST-request to get a token to use in the app.

data:{
  username: 'username',
  password: 'password'
}

On success, resolved with parsed token, user-id and username as parameters.
Rejects with status of received http-response.
*/
export function postToken(username, password, callback){
  return new Promise(function(resolve, reject){
    var xmlHttp = new XMLHttpRequest();

    xmlHttp.onreadystatechange =( () => {
      if (xmlHttp.readyState === 4){
        if (xmlHttp.status === 200){
          resolve({token:JSON.parse(xmlHttp.responseText).token, owner: JSON.parse(xmlHttp.responseText).owner, username: username});
        }
        else {
          reject(xmlHttp.status);
        }
      }
    });
    xmlHttp.open("POST", ConstVars.URI + "token");
    xmlHttp.setRequestHeader('Content-Type', 'application/json');
    xmlHttp.setRequestHeader('Authorization', password);
    xmlHttp.send(JSON.stringify({username: username}));
  });
}

/*
Logout event. Delete's a token from api. Resolves on 204 No content,
rejects on other responses.
*/
export function deleteToken(){
  return new Promise(function(resolve, reject){
    var xmlHttp = new XMLHttpRequest();

    xmlHttp.onreadystatechange =( () => {
      if (xmlHttp.readyState === 4){
        if (xmlHttp.status === 204){
          resolve();
        }
        else {
          reject(xmlHttp.status);
        }
      }
    });
    xmlHttp.open("DELETE", ConstVars.URI + "token/" + sessionStorage.getItem("loggedUserID"));
    xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
    xmlHttp.send();
  });
}

/*GET-request to check if token is already in use. If it is, it means user
is logged in, and is redirected to home page. Returns 404 if user hasn't got a token,
and a new one must be requested.

Resolves on 200 OK, rejects on any other response status*/
export function getToken(token, user_id){
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
    xmlHttp.open("GET", ConstVars.URI + "token/" + user_id);
    xmlHttp.setRequestHeader('Authorization', token);
    xmlHttp.send();
  });
}
