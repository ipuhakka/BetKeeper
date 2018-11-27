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
  }
}
