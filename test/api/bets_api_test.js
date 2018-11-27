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
