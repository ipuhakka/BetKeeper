import ConstVars from '../consts.js';

/*
  POST-request to uri/folders. Resolves on 201 Created response,
  rejects on any other response status, with given status.
*/
export function postFolder(folder){
  return new Promise(function(resolve, reject){
    var data = {
      folder: folder
    };
    var xmlHttp = new XMLHttpRequest();

    xmlHttp.onreadystatechange =( () => {
      if (xmlHttp.readyState === 4){
        if (xmlHttp.status === 201){
          resolve();
        }
        else {
          reject(xmlHttp.status);
        }
      }
    });
    xmlHttp.open("POST", ConstVars.URI + "folders");
    xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
    xmlHttp.setRequestHeader('Content-Type', 'application/json');
    xmlHttp.send(JSON.stringify(data));
  });
}

/*
  DELETE-request to uri/folders?folder={folder}.
  Resolves on 204 No content response,
  rejects on any other response status, with given status.
*/
export function deleteFolder(folder){
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
    xmlHttp.open("DELETE", ConstVars.URI + "folders/" + folder);
    xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
    xmlHttp.send();
  });
}

/*
  GET-request for getting folders for logged user.
  Resolves on 200 OK response,
  rejects on any other response status, with given status.
*/
export function getFolders(){
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
    xmlHttp.open("GET", ConstVars.URI + "folders/");
    xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
    xmlHttp.send();
  });
}

/*
GET-request to get folders of selected bet.
Resolves on 200 OK response,
rejects on any other response status, with given status.
*/
export function getFoldersOfBet(id){
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
    xmlHttp.open("GET", ConstVars.URI + "folders?bet_id=" + id);
    xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
    xmlHttp.send();
  });
}
