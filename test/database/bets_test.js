var chai = require('chai');
var expect = chai.expect;
var fo = require('../../database/file_operations');
var bets = require('../../database/bets');

const test_init = 'database/scripts/test_init.sql';
const testDB = 'database/data/testi.sqlite3';

describe('get_bets', function(){
  before(function(done){
    this.timeout(10000);
    fo.run_script(testDB, test_init, function(){
      done();
    });
  });

  after(function(done){
    fo.delete_database(testDB, function(){
      done();
    });
  });

  it('returns 4 bets by user 1', function(done){
    let results = bets.get_bets(testDB, 1);
    expect(results.length).to.equal(4);

    for(var i = 0; i < bets.length; i++){
      expect(results[i].owner).to.equal(1);
    }
    done();
  });

  it('returns 3 finished bets for user 1', function(done){
    let results = bets.get_bets(testDB, 1, true);
    expect(results.length).to.equal(3);
    done();
  });

  it('returns 1 unfinished bet for user 1', function(done){
    let results = bets.get_bets(testDB, 1, false);
    expect(results.length).to.equal(1);
    done();
  });

  it('has all bet item keys', function(done){
    let results = bets.get_bets(testDB, 1, false);
    expect(results[0]).to.have.all.keys('bet_won', 'name', 'odd', 'bet', 'date_time', 'owner', 'bet_id');
    done();
  });
});

describe('get_bets_from_folder', function(done){
  before(function(done){
    this.timeout(10000);
    fo.run_script(testDB, test_init, function(){
      done();
    });
  });

  after(function(done){
    fo.delete_database(testDB, function(){
      done();
    });
  });

  it('returns 3 bets from jannu27 folder valioliiga', function(done){
    let results = bets.get_bets_from_folder(testDB, 1, 'valioliiga');
    expect(results.length).to.equal(3);
    done();
  });

  it('returns 2 finished bets from jannu27 folder valioliiga', function(done){
    let results = bets.get_bets_from_folder(testDB, 1, 'valioliiga', true);
    expect(results.length).to.equal(2);
    done();
  });

  it('returns 1 unfinished bet from jannu27 folder valioliiga', function(done){
    let results = bets.get_bets_from_folder(testDB, 1, 'valioliiga', false);
    expect(results.length).to.equal(1);
    done();
  });
});

describe('get_bet', function(){
  before(function(done){
    this.timeout(10000);
    fo.run_script(testDB, test_init, function(){
      done();
    });
  });

  after(function(done){
    fo.delete_database(testDB, function(){
      done();
    });
  });

  it('returns null when bet_id does not exist', function(done){
    expect(bets.get_bet(testDB, 12)).to.equal(null);
    done();
  });

  it('returns correct bet', function(done){
    let bet = bets.get_bet(testDB, 7);
    expect(bet.owner).to.equal(5);
    expect(bet.bet_id).to.equal(7);
    done();
  });
});

describe('delete_bet', function(done){
  before(function(done){
    this.timeout(10000);
    fo.run_script(testDB, test_init, function(){
      done();
    });
  });

  after(function(done){
    fo.delete_database(testDB, function(){
      done();
    });
  });

  it('return false when trying to delete a bet from other user', function(done){
    var res = bets.delete_bet(testDB, 7, 1);
    expect(res).to.equal(false);
    done();
  });

  it('returns false when trying to delete an unexisting bet', function(done){
    var res = bets.delete_bet(testDB, 12, 1);
    expect(res).to.equal(false);
    done();
  });

  it('true on deleting users own bet successfully', function(done){
    var res = bets.delete_bet(testDB, 1, 1);
    expect(res).to.equal(true);
    done();
  });

});
