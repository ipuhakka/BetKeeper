var chai = require('chai');
var chaiHttp = require('chai-http');
chai.use(chaiHttp);
var should = chai.should();
var server = require('../../api/index');
var tokenLog = require('../../api/tokenLog');
const uri = '/api/token';

describe('post_token', function(){
  after(function(done){
    tokenLog.clear();
    require('../../api/index').close();
    done();
  });

  it('responds with 401 when username and password dont match', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .set('authorization', 'salasana3')
    .send(JSON.stringify({username: 'testi'}))
    .end(function(err, res){
      res.should.have.status(401);
      done();
    });
  });

  it('responds with 200 on authorization success', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .set('authorization', 'salasana')
    .send(JSON.stringify({username: 'testi'}))
    .end(function(err, res){
      res.should.have.status(200);
      done();
    });
  });

  it('responds with 400 on missing authorization header', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .send(JSON.stringify({username: 'testi'}))
    .end(function(err, res){
      res.should.have.status(400);
      done();
    });
  });

  it('responds with 400 on missing username', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .set('authorization', 'salasana')
    .send()
    .end(function(err, res){
      res.should.have.status(400);
      done();
    });
  });

  it('responds with 415 on invalid content-type', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/text')
    .set('authorization', 'salasana')
    .send()
    .end(function(err, res){
      res.should.have.status(400);
      done();
    });
  });
});

describe('get_token', function(){
  after(function(done){
    require('../../api/index').close();
    done();
  });

  it('responds with 400 on missing authorization header', function(done){
    chai.request(server)
    .get(uri + '/1')
    .send()
    .end(function(err, res){
      res.should.have.status(400);
      done();
    });
  });

  it('responds with 200 on giving correct token for user', async function(){
    token = await tokenLog.create_token(1);
    tokenLog.add_token(token);

    chai.request(server)
    .get(uri + '/1')
    .set('authorization', token.token)
    .send()
    .end(function(err, res){
      res.should.have.status(200);
    });
  });

  it('responds with 404 on token not found', async function(){
    token = await tokenLog.create_token(1);
    tokenLog.add_token(token);
    tokenLog.clear();

    chai.request(server)
    .get(uri + '/1')
    .set('authorization', token.token)
    .send()
    .end(function(err, res){
      res.should.have.status(404);
    });
  });

  it('responds with 401 on token belonging to another user', async function(){
    token = await tokenLog.create_token(1);
    tokenLog.add_token(token);

    chai.request(server)
    .get(uri + '/2')
    .set('authorization', token.token)
    .send()
    .end(function(err, res){
    tokenLog.clear();
      res.should.have.status(401);
    });
  });
});
