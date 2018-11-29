const users = require('../../database/users');

module.exports = {
  /*
  POST-request to /api/users  create a new user to the database.
  request:
    headers: {
      content-type: 'application/json',
      authorization: 'password'
    }

    body: {
      username: 'username'
    }
  responses:
    201 Created,
    400 Bad request, on missing arguments in request,
    409 Conflict, when user already exists,
    415 Unsupported media type, on invalid media type header value.
  */
  post: function(req, res){
    if (req.get('content-type') !== 'application/json'){
      return res.status(415).send(JSON.stringify({error: "Request didn't containg application/json header"}));
    }

    if (req.body.username === undefined || req.get('authorization') === undefined){
      return res.status(400).send(JSON.stringify({error: "Request had missing parameters"}));
    }

    var result = users.add_user(req.body.username, req.get('authorization'));

    if (result === 1){
      return res.status(201).send();
    }
    else if (result === 0){
      return res.status(409).send();
    }

    return res.status(500).send();
  }
};
