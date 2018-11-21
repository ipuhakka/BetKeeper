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
    return res.status(500).send();
  }
};
