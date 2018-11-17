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

describe('create_bet', function(done){
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

  it('returns true on success', function(done){
    var res = bets.create_bet(testDB, 1, new Date().toString(), 4.8, 4.7, "", false);
    expect(res).to.equal(true);
    done();
  });

  it('returns false on invalid date', function(done){
    var res = bets.create_bet(testDB, 1, "2018-084-05 14:524:40" , 4.8, 4.7, "", false);
    expect(res).to.equal(false);
    done();
  });

  it('returns false when user does not exist', function(done){
    var res = bets.create_bet(testDB, -1, new Date().toString() , 4.8, 4.7, "", false);
    expect(res).to.equal(false);
    done();
  });

  it('returns null when database is empty', function(done){
    var res = bets.create_bet("testDB", -1, new Date().toString() , 4.8, 4.7, "", false);
    expect(res).to.equal(null);
    fo.delete_database("testDB", function(){
      done();
    });
  });

  it('returns false on invalid decimal values', function(done){
    var res = bets.create_bet(testDB, 1, new Date().toString() , 'not decimal', 4.7, "", false);
    expect(res).to.equal(false);
    done();
  })
});

describe('add_bet_to_folders', function(done){
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

  it('adds bet to folders when it does not exist in folder already', function(done){
    var bet = bets.get_bet(testDB, 1);
    var res = bets.add_bet_to_folders(testDB, ["liiga", "triplat"], bet.bet_id, 1);
    expect(res.length).to.equal(2);
    done();
  });

  it('does not add to folder where bet is already', function(done){
    var bet = bets.get_bet(testDB, 2);
    var res = bets.add_bet_to_folders(testDB, ["liiga", "valioliiga"], bet.bet_id, 1);
    expect(res.length).to.equal(1);
    done();
  });

  it('does not add bet to other users folders', function(done){
    var bet = bets.get_bet(testDB, 2);
    var res = bets.add_bet_to_folders(testDB, ["tuplat"], bet.bet_id, 1);
    expect(res.length).to.equal(0);
    done();
  });

  it('does not add other users bet', function(done){
    var res = bets.add_bet_to_folders(testDB, ["triplat"], 4, 1);
    expect(res.length).to.equal(0);
    done();
  });
})

describe('delete_bet_from_folders', function(){
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

  it('returns 2 when deletes from two folders', function(done){
    bets.add_bet_to_folders(testDB, ["triplat"], 1, 1);
    var res = bets.delete_bet_from_folders(testDB, ["valioliiga", "triplat"], 1, 1);
    expect(res.length).to.equal(2);
    bets.add_bet_to_folders(testDB, ["valioliiga"], 1, 1);
    done();
  });

  it('does not delete from folders where bet does not exist', function(done){
    var res = bets.delete_bet_from_folders(testDB, ["valioliiga", "triplat"], 1, 1);
    expect(res.length).to.equal(1);
    done();
  });

  it('does not delete other users bet', function(done){
    var res = bets.delete_bet_from_folders(testDB, ["tuplat"], 1, 4);
    expect(res.length).to.equal(0);
    done();
  });
});
