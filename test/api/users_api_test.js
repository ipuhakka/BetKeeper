var chai = require('chai');
var chaiHttp = require('chai-http');
chai.use(chaiHttp);
var expect = chai.expect;
var should = chai.should();
var server = require('../../api/index');

describe('users_post', function(){
  const uri = '/api/users';

  after(function(done){
    //require('../../api/index').close();
    done();
  });

  it('responds with 201 on creating new user', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .set('authorization', 'password')
    .send(JSON.stringify({username: 'unused username'}))
    .end(function(err, res){
      res.should.have.status(201);
      done();
    });
  });

  it('responds with 409 on creating existing username', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .set('authorization', 'password')
    .send(JSON.stringify({username: 'jannu27'}))
    .end(function(err, res){
      res.should.have.status(409);
      done();
    });
  });

  it('responds with 415 on invalid content-type', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/text')
    .set('authorization', 'password')
    .send(JSON.stringify({username: 'unused username'}))
    .end(function(err, res){
      res.should.have.status(415);
      done();
    });
  });

  it('responds with 415 on missing content-type', function(done){
    chai.request(server)
    .post(uri)
    .set('authorization', 'password')
    .send(JSON.stringify({username: 'unused username'}))
    .end(function(err, res){
      res.should.have.status(415);
      done();
    });
  });

  it('responds with 400 on missing username', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .set('authorization', 'password')
    .send()
    .end(function(err, res){
      res.should.have.status(400);
      done();
    });
  });

  it('responds with 400 on missing password', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .send(JSON.stringify({username: 'unused username2'}))
    .end(function(err, res){
      res.should.have.status(400);
      done();
    });
  });
});
