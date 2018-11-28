const folders = require('../../database/folders');
const tokenLog = require('../tokenLog');

module.exports = {
  /*
  GET-request to /api/folders. {bet_id}
  can be given in query /api/folders?bet_id={1}.

  Returns an array of folder names, either all get_folders
  of user or folders which contain specified bet.

  Responses:
  200 OK,
  401 Unauthorized, if no token or unvalid token has been given
  in authorization header.
  */
  get: function(req, res, bet_id){
    if (req.get('authorization') === undefined){
      return res.status(401).send();
    }

    var tokenOwner = tokenLog.get_token_owner(req.get('authorization'));
    if (tokenOwner === -1){
      return res.status(401).send();
    }

    let results = folders.get_users_folders(tokenOwner, bet_id);
    res.set('content-type', 'application/json');
    return res.status(200).send(results);
  },

  /*
  POST-request to create a new folder for user.

  Request:
    headers:
      'content-type': 'application/json',
      'authorization': 'token'
    body: {
      folder: 'new folder name'
    }

  responses:
    201 Created,
    400 Bad request, on missing folder value
    401 Unauthorized, on missing or invalid authorization header value,
    409 Conflict, on user already using folder name.
  */
  post: function(req, res){
    if (req.get('content-type') !== 'application/json'){
      return res.status(415).send();
    }
    if (req.get('authorization') === undefined || !tokenLog.contains_token(req.get('authorization'))){
      return res.status(401).send();
    }
    let folder = req.body.folder;

    if (folder === undefined){
      return res.status(400).send();
    }

    let result = folders.add_folder(tokenLog.get_token_owner(req.get('authorization')), folder);
    if (result){
      return res.status(201).send();
    } else {
      return res.status(409).send();
    }
  },

  /*
  DELETE-request to /api/folders/:folder. Deletes a folder from database,
  if user has a folder of specified name.

  Request:
    headers:
      "authorization": "token string"

  Response:
    204 No content, on successfull deletion,
    401 Unauthorized, if token is not in use.
    404 Not found when user doesn't have a folder of specified name.

  */
  delete: function(req, res, folderToDelete){
    if (req.get('authorization') === undefined || !tokenLog.contains_token(req.get('authorization'))){
      return res.status(401).send();
    }

    let owner = tokenLog.get_token_owner(req.get('authorization'));
    let result = folders.delete_folder(owner, folderToDelete);

    if (result){
      return res.status(204).send();
    }
    else {
      return res.status(404).send();
    }
  }
}
