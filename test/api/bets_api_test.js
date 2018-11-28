var chai = require('chai');
var chaiHttp = require('chai-http');
chai.use(chaiHttp);
var should = chai.should();
var server = require('../../api/index');
var tokenLog = require('../../api/tokenLog');
var expect = chai.expect;
var bets = require('../../database/bets');

const uri = '/api/bets';

describe('get_bets', function(){
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

  it('returns finished bets when finished=true', function(done){
    chai.request(server)
    .get(uri+'?finished=true')
    .set('authorization', token1.token)
    .send()
    .end(function(err, res){
      res.should.have.status(200);
      res.body.map(bet => {
        expect(bet.bet_won).to.not.equal(-1);
      });
      done();
    });
  });

  it('returns unfinished bets when finished=false', function(done){
    chai.request(server)
    .get(uri+'?finished=false')
    .set('authorization', token1.token)
    .send()
    .end(function(err, res){
      res.should.have.status(200);
      res.body.map(bet => {
        expect(bet.bet_won).to.equal(-1);
      });
      done();
    });
  });

  it('returns bets from specified folder', function(done){
    chai.request(server)
    .get(uri+'?folder=valioliiga')
    .set('authorization', token1.token)
    .send()
    .end(function(err, res){
      res.should.have.status(200);
      expect(res.body.length).to.equal(3);
      done();
    });
  });

  it('returns all bets by user when no query parameters are given', function(done){
    chai.request(server)
    .get(uri)
    .set('authorization', token1.token)
    .send()
    .end(function(err, res){
      res.should.have.status(200);
      expect(res.body.length).to.equal(4);
      done();
    });
  });

  it('responds with 401 on invalid credentials', function(done){
    chai.request(server)
    .get(uri)
    .set('authorization', token2.token)
    .send()
    .end(function(err, res){
      res.should.have.status(401);
      done();
    });
  });
});

describe('delete_bets', function(){
  let token1, token2;

  before(async function(){
    token1 = await tokenLog.create_token(4);
    token2 = await tokenLog.create_token(5);
    tokenLog.add_token(token1);
    return;
  });

  after(function(done){
    tokenLog.clear();
    done();
  });

  it('responds with 204 on successful delete', function(done){
    chai.request(server)
    .delete(uri + '/6')
    .set('authorization', token1.token)
    .send()
    .end(function(err, res){
      res.should.have.status(204);
      done();
    });
  });

  it('responds with 404 when bet does not belong to the user', function(done){
    chai.request(server)
    .delete(uri + '/1')
    .set('authorization', token1.token)
    .send()
    .end(function(err, res){
      res.should.have.status(404);
      done();
    });
  });

  it('responds with 401 when token is not in use', function(done){
    chai.request(server)
    .delete(uri + '/6')
    .set('authorization', token2.token)
    .send()
    .end(function(err, res){
      res.should.have.status(401);
      done();
    });
  });

  it('responds with 404 on deleting unexisting bet', function(done){
    chai.request(server)
    .delete(uri + '/-1')
    .set('authorization', token1.token)
    .send()
    .end(function(err, res){
      res.should.have.status(404);
      done();
    });
  });

  it('returns an array of folders where the bet was deleted', function(done){
    tokenLog.add_token(token2);
    chai.request(server)
    .delete(uri + '/7?folders=["rooneynBetsit", "tuplat"]')
    .set('authorization', token2.token)
    .send()
    .end(function(err, res){
      res.should.have.status(200)
      console.log("response: " + JSON.stringify(res.body))
      expect(res.body).to.deep.equal(["rooneynBetsit"]);
      tokenLog.clear();
      tokenLog.add_token(token1);
      done();
    });
  });
});
