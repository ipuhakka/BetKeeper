import ConstVars from '../Consts.js';

/*
  POST-request to uri/folders.
*/
export function postFolder(folder, callback){
  var data = {
    folder: folder
  };
  var xmlHttp = new XMLHttpRequest();

  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      let status = xmlHttp.status;
      if ([201, 400, 401, 409].includes(status))
        callback(xmlHttp.status);
    }
  });
  xmlHttp.open("POST", ConstVars.URI + "folders");
  xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
  xmlHttp.setRequestHeader('Content-Type', 'application/json');
  xmlHttp.send(JSON.stringify(data));
}

/*
  DELETE-request to uri/folders?folder={folder}.
*/
export function deleteFolder(folder, callback){
  var xmlHttp = new XMLHttpRequest();

  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      if ([204, 400, 401, 404].includes(xmlHttp.status))
        callback(xmlHttp.status);
    }
  });
  xmlHttp.open("DELETE", ConstVars.URI + "folders/" + folder);
  xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
  xmlHttp.send();
}

/*
  GET-request for getting folders for logged user.
*/
export function getFolders(callback){
  var xmlHttp = new XMLHttpRequest();

  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      if (xmlHttp.status === 200)
        callback(xmlHttp.status, xmlHttp.responseText);
      else if (xmlHttp.status === 401)
        callback(xmlHttp.status, null);
    }

  });
  xmlHttp.open("GET", ConstVars.URI + "folders/");
  xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
  xmlHttp.send();
}

/*
GET-request to get folders of selected bet.
*/
export function getFoldersOfBet(id, callback){
  var xmlHttp = new XMLHttpRequest();

  xmlHttp.onreadystatechange =( () => {
    if (xmlHttp.readyState === 4){
      if (xmlHttp.status === 200)
        callback(xmlHttp.status, xmlHttp.responseText);
      else if (xmlHttp.status === 401)
        callback(xmlHttp.status, null);
    }
  });
  xmlHttp.open("GET", ConstVars.URI + "folders?bet_id=" + id);
  xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
  xmlHttp.send();
}
