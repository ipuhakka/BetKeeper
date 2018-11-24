const users = require('./resources/users');
const token = require('./resources/token');
const bodyParser = require('body-parser');

module.exports = function(app){
  app.use(bodyParser.urlencoded({
    extended: true
  }));

  app.use(bodyParser.json());

  app.use(function(req, res, next) {
    res.header("Access-Control-Allow-Origin", "*");
    res.header('Access-Control-Allow-Methods', 'GET,POST,OPTIONS,MODIFY,DELETE');
    next();
  });

  app.post('/api/users', function(req, res){
    users.post(req, res);
  });

  app.post('/api/token', function(req, res){
    token.post(req, res);
  });

  app.get('/api/token/:user_id', function(req, res){
    token.get(req, res, parseInt(req.params.user_id));
  });
}
