const tokenLog = require('../tokenLog');
const users = require('../../database/users');

module.exports = {
  /*
  POST-request to access the system.

  Request{
    headers:
      "authorization": "password",
      "content-type": "application/json"
    body: {
      username: "username"
    }
  }

  Responses:
    200 OK:
    headers: {
      "content-type": "application/json"
    }
    body:{
      token: "12-char string",
      user_id: 1
    },
    400 Bad request,
    401 Unauthorized
  */
  post: function(req, res){
    let body = req.body;
    if(body.username === undefined || req.get('authorization') === undefined){
      return res.status(400).send();
    }

    let id = users.get_user_id(body.username);
    let authorized = users.check_password(id, req.get('authorization'));

    if (!authorized){
      return res.status(401).send();
    }
    if (authorized){
      token = tokenLog.create_token(id);
      tokenLog.add_token(token);
      res.set('content-type', 'application/json');
      return res.status(200).send(token);
    }
    return res.status(500).send();
  },

  /*
  GET-request to address /api/token/{user_id} to check if token is still valid.

  request:
    headers:
      "authorization":"token string"

    responds:
      200 OK if token is still in use and user_id owns it
      401 If user_id does not own token
      404 If token is not in use
  */
  get: function(req, res, user_id){
    token = req.get('authorization');
    if (token === undefined){
      return res.status(400).send();
    }

    if (tokenLog.contains_token(token)){
      tokenOwner = tokenLog.get_token_owner(token);
      if (tokenOwner === user_id){
        return res.status(200).send();
      }
      else {
        return res.status(401).send();
      }
    }
    return res.status(404).send();
  }
};
