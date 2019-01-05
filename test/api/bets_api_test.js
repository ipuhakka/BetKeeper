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
        expect(bet.bet_won).to.not.equal(null);
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
        expect(bet.bet_won).to.equal(null);
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
      expect(res.body).to.deep.equal(["rooneynBetsit"]);
      tokenLog.clear();
      tokenLog.add_token(token1);
      done();
    });
  });
});

describe('post_bets', function(){
  let token1, token2;
  var bet = {
    bet_won: -1, //-1 = not finished, 0=lost, 1=won
    name: 'string', //optional name for the bet
    odd: 3.12,
    bet: 2.21,
    folders: ["rooneynBetsit", "unexistingFolder"]
  };

  before(async function(){
    token1 = await tokenLog.create_token(5);
    token2 = await tokenLog.create_token(4);
    tokenLog.add_token(token1);
    return;
  });

  after(function(done){
    tokenLog.clear();
    done();
  });

  it('responds 201 on success', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .set('authorization', token1.token)
    .send(JSON.stringify(bet))
    .end(function(err, res){
      res.should.have.status(201);
      bets.delete_bet(10, 5);
      done();
    });
  });

  it('tells to which folders bet was added on success and bet id', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .set('authorization', token1.token)
    .send(bet)
    .end(function(err, res){
      res.should.have.status(201);
      expect(res.body.addedToFolders).to.deep.equal(["rooneynBetsit"]);
      expect(res.body.bet_id).to.equal(10);
      bets.delete_bet(10, 5);
      done();
    });
  });

  it('responds 401 on unused token', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .set('authorization', token2.token)
    .send(bet)
    .end(function(err, res){
      res.should.have.status(401)
      done();
    });
  });

  it('responds 400 on invalid odd or bet value', function(done){
    var invalidBet = {
      bet_won: -1, //-1 = not finished, 0=lost, 1=won
      name: 'string', //optional name for the bet
      odd: '3.12',
      bet: 2.21,
      folders: []
    };

    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .set('authorization', token1.token)
    .send(invalidBet)
    .end(function(err, res){
      res.should.have.status(400)
      done();
    });
  });

  it('responds 400 on missing any argument', function(done){
    var invalidBet = {
      name: 'string', //optional name for the bet
      odd: 3.12,
      bet: 2.21,
      folders: []
    };

    chai.request(server)
    .post(uri)
    .set('content-type', 'application/json')
    .set('authorization', token1.token)
    .send(invalidBet)
    .end(function(err, res){
      res.should.have.status(400)
      done();
    });
  });

  it('responds 415 on invalid content-type', function(done){
    chai.request(server)
    .post(uri)
    .set('content-type', 'application/text')
    .set('authorization', token1.token)
    .send()
    .end(function(err, res){
      res.should.have.status(415)
      done();
    });
  });
});

describe('put_bets', function(){
  let token1, token2;

  before(async function(){
    token1 = await tokenLog.create_token(5);
    token2 = await tokenLog.create_token(4);
    tokenLog.add_token(token1);
    return;
  });

  it('responds with 200 on successful modification', function(done){
    var bet = {
      bet_won: -1, //-1 = not finished, 0=lost, 1=won
      name: 'string', //optional name for the bet
      odd: 3.12,
      bet: 2.21
    };

    chai.request(server)
    .put(uri + "/7")
    .set('content-type', 'application/json')
    .set('authorization', token1.token)
    .send(JSON.stringify(bet))
    .end(function(err, res){
      res.should.have.status(200);
      expect(res.body.bet_id).to.equal(7);
      done();
    });
  });

  it('tells to which folders bet was added on success and bet id', function(done){
    var bet = {
      bet_won: -1, //-1 = not finished, 0=lost, 1=won
      name: 'string', //optional name for the bet
      odd: 3.12,
      bet: 2.21,
      folders: ["another test folder"]
    };

    chai.request(server)
    .put(uri + "/7")
    .set('content-type', 'application/json')
    .set('authorization', token1.token)
    .send(bet)
    .end(function(err, res){
      res.should.have.status(200);
      expect(res.body.addedToFolders).to.deep.equal(["another test folder"]);
      expect(res.body.bet_id).to.equal(7);
      done();
    });
  });

  it('responds with 400 if decimals are invalid', function(done){
    var bet = {
      bet_won: -1, //-1 = not finished, 0=lost, 1=won
      name: 'string', //optional name for the bet
      odd: '3,12',
      bet: 2.21
    };

    chai.request(server)
    .put(uri + "/7")
    .set('content-type', 'application/json')
    .set('authorization', token1.token)
    .send(JSON.stringify(bet))
    .end(function(err, res){
      res.should.have.status(400);
      done();
    });
  });

  it('responds with 400 if bet_won is not given in request', function(done){
    var bet = {
      name: 'string', //optional name for the bet
      odd: 3.12,
      bet: 2.21
    };

    chai.request(server)
    .put(uri + "/7")
    .set('content-type', 'application/json')
    .set('authorization', token1.token)
    .send(JSON.stringify(bet))
    .end(function(err, res){
      res.should.have.status(400);
      done();
    });
  });

  it('responds with 401 on invalid token', function(done){
    var bet = {
      bet_won: -1, //-1 = not finished, 0=lost, 1=won
      name: 'string', //optional name for the bet
      odd: 3.12,
      bet: 2.21
    };

    chai.request(server)
    .put(uri + "/7")
    .set('content-type', 'application/json')
    .set('authorization', token2.token)
    .send(JSON.stringify(bet))
    .end(function(err, res){
      res.should.have.status(401);
      done();
    });
  });

  it('responds with 401 on modifying other users bet', function(done){
    var bet = {
      bet_won: -1, //-1 = not finished, 0=lost, 1=won
      name: 'string', //optional name for the bet
      odd: 3.12,
      bet: 2.21
    };

    chai.request(server)
    .put(uri + "/1")
    .set('content-type', 'application/json')
    .set('authorization', token1.token)
    .send(JSON.stringify(bet))
    .end(function(err, res){
      res.should.have.status(401);
      done();
    });
  });

  it('responds with 404 on modifying unexisting bet', function(done){
    var bet = {
      bet_won: -1, //-1 = not finished, 0=lost, 1=won
      name: 'string', //optional name for the bet
      odd: 3.12,
      bet: 2.21
    };

    chai.request(server)
    .put(uri + "/100")
    .set('content-type', 'application/json')
    .set('authorization', token1.token)
    .send(JSON.stringify(bet))
    .end(function(err, res){
      res.should.have.status(404);
      done();
    });
  });

  it('responds with 415 on invalid content-type header', function(done){
    chai.request(server)
    .put(uri + "/1")
    .set('content-type', 'application/text')
    .set('authorization', token1.token)
    .send()
    .end(function(err, res){
      res.should.have.status(415);
      done();
    });
  });
});
