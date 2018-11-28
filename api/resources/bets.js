const bets = require('../../database/bets');
const tokenLog = require('../tokenLog');

module.exports = {
  /*
  GET-request to /api/bets.
  Query params: finished=boolean, folder=string.

  Request headers:
    'authorization': 'token string'

  Returns a list of bets with status code 200.

  Responses:
    200 OK,
    401 Unauthorized
  */
  get: function(req, res, bets_finished, folder){
    if (bets_finished !== undefined){
      bets_finished = bets_finished === 'true' ? true : false;
    }

    if (req.get('authorization') === undefined || !tokenLog.contains_token(req.get('authorization'))){
      return res.status(401).send();
    }
    let owner = tokenLog.get_token_owner(req.get('authorization'));
    let result_bets = [];
    if (folder === undefined){
      result_bets = bets.get_bets(owner, bets_finished);
    }
    else {
      result_bets = bets.get_bets_from_folder(owner, folder, bets_finished);
    }
    res.set('content-type', 'application/json');
    return res.status(200).send(result_bets);
  },

  /*
  DELETE-request to /api/bets/{bet_id}.
  If a folders lsit is specified in query (?folders=['folder1', 'folder2']),
  bet is deleted only from specified folders.

  Responses:
    204 No content on successful deletion from all folders,
    200 OK with folders from which bet was deleted when folders are specified,
    401 Unauthorized if token is not in use,
    404 Not found if user does not own a bet with given id.
  */
  delete: function(req, res, bet_id, folders){
    if (folders !== undefined){
      folders = JSON.parse(folders);
    }

    if (req.get('authorization') === undefined || !tokenLog.contains_token(req.get('authorization'))){
      return res.status(401).send();
    }
    console.log("folders: " + JSON.stringify(folders));
    let owner = tokenLog.get_token_owner(req.get('authorization'));
    let result;

    if (folders === undefined){
      result = bets.delete_bet(bet_id, owner);
      if (result){
        return res.status(204).send();
      } else if (result === null){
        return res.status(500).send();
      } else {
        return res.status(404).send();
      }
    } else {
      result = bets.delete_bet_from_folders(folders, bet_id, owner);
      if (result.length > 0){
        res.set('content-type', 'application/json');
        return res.status(200).send(result);
      } else if (result === null){
        return res.status(500).send();
      } else {
        console.log("here");
        return res.status(404).send();
      }
    }
  }
}
