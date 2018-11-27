const users = require('./resources/users');
const folders = require('./resources/folders');
const bets = require('./resources/bets');
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

  app.get('/api/folders', function(req, res){
    folders.get(req, res, req.query.bet_id);
  });

  app.post('/api/folders', function(req, res){
    folders.post(req, res);
  });

  app.delete('/api/folders/:folder', function(req, res){
    folders.delete(req, res, req.params.folder);
  });

  app.get('/api/bets', function(req, res){
    bets.get(req, res, req.query.finished, req.query.folder);
  });
}
