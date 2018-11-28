const bets = require('../../database/bets');
const tokenLog = require('../tokenLog');
const isNumber = require('is-number');

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
  If a folders list is specified in query (?folders=['folder1', 'folder2']),
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
        return res.status(404).send();
      }
    }
  },

  /*
  POST-request to /api/bets. Creates a new bet to database. If folders are given,
  adds bet also to specified folders.

  request:
    headers:
      'authorization': 'token string',
      'content-type': 'application/json'
    body: {
      bet_won: 0, (-1 = bet not finished, 0 = bet lost, 1 = bet won)
      name: 'name for the bet',
      odd: 2.34,
      bet: 3.00,
      folders: ["folder1", "folder2"]
    }

  response:
    201 Created. If folders were specified, returns a list of folder names
    to which bet was added.
    400 Bad request on invalid or missing parameters,
    401 On invalid token in authorization header,
    415 on invalid content-type header.
  */
  post: function(req, res){
    if (req.get('content-type') !== 'application/json'){
      return res.status(415).send();
    }
    if (req.get('authorization') === undefined || !tokenLog.contains_token(req.get('authorization'))){
      return res.status(401).send();
    }
    let body = req.body;
    if (!isValidBetRequest(body)){
      return res.status(400).send();
    }
    let bet_result = null;
    if (body.bet_won === 1){
      bet_result = true;
    }
    else if (body.bet_won === 0){
      bet_result = false;
    }
    let owner = tokenLog.get_token_owner(req.get('authorization'));
    let id = bets.create_bet(owner, body.odd, body.bet, body.name, bet_result);

    if (id !== null && id !== -1){
      if (body.folders !== undefined && body.folders.length > 0){
        let addedToFolders = bets.add_bet_to_folders(body.folders, id, owner);
        if (addedToFolders !== null){
          res.set('content-type', 'application/json');
          return res.status(201).send({addedToFolders: addedToFolders, bet_id: id});
        }
      } else {
        return res.status(201).send({bet_id: id});
      }
    }
  }
}

function isValidBetRequest(bet){
  if (bet.bet_won === undefined || bet.bet === undefined || bet.bet === undefined || bet.odd === undefined
  || bet.name === undefined){
    return false;
  }
  if(!isNumber(bet.odd) || !isNumber(bet.bet) || typeof bet.odd === "string" || typeof bet.bet === "string"){
    return false;
  }

  return true;
}
