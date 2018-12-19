var chai = require('chai');
var expect = chai.expect;
var should = chai.should();
var tokenLog = require('../../api/tokenLog');


describe('contains_token', function(){
  afterEach(function(done){
    tokenLog.clear();
    done();
  });

  it('returns true when tokenLog contains token', async function(){
    token = await tokenLog.create_token(1);
    tokenLog.add_token(token);
    return expect(tokenLog.contains_token(token.token)).to.equal(true);
  });

  it('returns false when tokenLog does not contain token', async function(){
    token = await tokenLog.create_token(1);
    tokenLog.add_token(token);
    token2 = await tokenLog.create_token(2);
    return expect(tokenLog.contains_token(token2.token)).to.equal(false);
  });
});

describe('get_token_owner', function(){
  let token, token2, token3;
  before(async function(){
    token = await tokenLog.create_token(1);
    tokenLog.add_token(token);
    token2 = await tokenLog.create_token(2);
    tokenLog.add_token(token2);
    token3 = await tokenLog.create_token(3);
    return;
  });

  afterEach(function(done){
    tokenLog.clear();
    done();
  });

  it('returns 1', function(done){
    expect(tokenLog.get_token_owner(token.token)).to.equal(1);
    done();
  });

  it('returns -1 when token does not exist', function(done){
    expect(tokenLog.get_token_owner(token3.token)).to.equal(-1);
    done();
  });
});

describe('delete_token', function(){
  let token, token2, token3;
  before(async function(){
    token = await tokenLog.create_token(1);
    tokenLog.add_token(token);
    token2 = await tokenLog.create_token(2);
    tokenLog.add_token(token2);
    token3 = await tokenLog.create_token(3);
    tokenLog.add_token(token3);
    return;
  });

  it('deletes token from list when owner matches a given token string', function(done){
    expect(tokenLog.delete_token(token.token, token.owner)).to.equal(true);
    done();
  });

  it('returns false when trying to delete a token of another user', function(done){
    expect(tokenLog.delete_token(token2.token, token3.owner)).to.equal(false);
    done();
  });
});
