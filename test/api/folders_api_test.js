var chai = require('chai');
var chaiHttp = require('chai-http');
chai.use(chaiHttp);
var should = chai.should();
var server = require('../../api/index');
var tokenLog = require('../../api/tokenLog');
var expect = chai.expect;
var folders = require('../../database/folders');

const uri = '/api/folders';

describe('get_folders', function(){
  let token1, token2;

  before(async function(){
    token1 = await tokenLog.create_token(1);
    token2 = await tokenLog.create_token(2);
    tokenLog.add_token(token1);
    return;
  });

  after(function(done){
    tokenLog.clear();
    done();
  });

  it('gets folders where bet exists', function(done){
    chai.request(server)
    .get(uri+'?bet_id=1')
    .set('authorization', token1.token)
    .send()
    .end(function(err, res){
      res.should.have.status(200);
      expect(res.body).to.deep.equal(['valioliiga']);
      done();
    });
  });

  it('gets all users folders when bet is not queried', function(done){
    chai.request(server)
    .get(uri)
    .set('authorization', token1.token)
    .send()
    .end(function(err, res){
      res.should.have.status(200);
      let results = res.body;
      results.map(folder => {
        expect(['valioliiga', 'liiga', 'triplat'].includes(folder));
      });
      done();
    });
  });

  it('responds 401 on token not found', function(done){
    chai.request(server)
    .get(uri)
    .set('authorization', token2.token)
    .send()
    .end(function(err, res){
      res.should.have.status(401);
      done();
    });
  });

  it('responds 401 on no authorization header', function(done){
    chai.request(server)
    .get(uri)
    .send()
    .end(function(err, res){
      res.should.have.status(401);
      done();
    });
  });
});

describe('post_folders', function(){
  let token1, token2;

  before(async function(){
    token1 = await tokenLog.create_token(3);
    token2 = await tokenLog.create_token(2);
    tokenLog.add_token(token1);
    return;
  });

  after(function(done){
    tokenLog.clear();
    done();
  });

  it('responds with 401 on not used token', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .set('authorization', token2.token)
    .send(JSON.stringify({folder: 'someFolderName'}))
    .end(function(err, res){
      res.should.have.status(401);
      done();
    });
  });

  it('responds with 400 on not giving folder parameter in body', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .set('authorization', token1.token)
    .send()
    .end(function(err, res){
      res.should.have.status(400);
      done();
    });
  });

  it('responds with 409 on user already having folder name in use', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .set('authorization', token1.token)
    .send(JSON.stringify({folder: 'tuplat'}))
    .end(function(err, res){
      res.should.have.status(409);
      done();
    });
  });

  it('responds with 415 on invalid content-type header', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/text')
    .set('authorization', token2.token)
    .send(JSON.stringify({folder: 'tuplat'}))
    .end(function(err, res){
      res.should.have.status(415);
      done();
    });
  });

  it('responds with 201 on success', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .set('authorization', token1.token)
    .send(JSON.stringify({folder: 'new folder name'}))
    .end(function(err, res){
      res.should.have.status(201);
      folders.delete_folder(3, 'new folder name');
      done();
    });
  });
});

describe('delete_folder', function(){
  let token1, token2;

  before(async function(){
    token1 = await tokenLog.create_token(3);
    token2 = await tokenLog.create_token(2);
    tokenLog.add_token(token1);
    return;
  });

  after(function(done){
    tokenLog.clear();
    done();
  });

  it('responds with 204 on successful delete', function(done){
    chai.request(server)
    .delete(uri + '/tuplat')
    .set('authorization', token1.token)
    .send()
    .end(function(err, res){
      res.should.have.status(204);
      folders.add_folder(3, 'tuplat');
      done();
    });
  });

  it('responds with 404 on user not having folder of specified name', function(done){
    chai.request(server)
    .delete(uri + '/valioliiga')
    .set('authorization', token1.token)
    .send()
    .end(function(err, res){
      res.should.have.status(404);
      done();
    });
  });

  it('responds with 401 on invalid authorization', function(done){
    chai.request(server)
    .delete(uri + '/tuplat')
    .set('authorization', token2.token)
    .send()
    .end(function(err, res){
      res.should.have.status(401);
      done();
    });
  });
});
