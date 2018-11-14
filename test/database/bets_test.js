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
