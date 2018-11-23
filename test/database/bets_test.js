var chai = require('chai');
var expect = chai.expect;
var fo = require('../../database/file_operations');
var bets = require('../../database/bets');
var config = require('../../api/config');

const test_init = require('../test_files').test_init;
const testDB = require('../test_files').testDB;

describe('get_bets', function(){
  it('returns 4 bets by user 1', function(done){
    let results = bets.get_bets(1);
    expect(results.length).to.equal(4);

    for(var i = 0; i < bets.length; i++){
      expect(results[i].owner).to.equal(1);
    }
    done();
  });

  it('returns 3 finished bets for user 1', function(done){
    let results = bets.get_bets(1, true);
    expect(results.length).to.equal(3);
    done();
  });

  it('returns 1 unfinished bet for user 1', function(done){
    let results = bets.get_bets(1, false);
    expect(results.length).to.equal(1);
    done();
  });

  it('has all bet item keys', function(done){
    let results = bets.get_bets(1, false);
    expect(results[0]).to.have.all.keys('bet_won', 'name', 'odd', 'bet', 'date_time', 'owner', 'bet_id');
    done();
  });
});

describe('get_bets_from_folder', function(done){
  it('returns 3 bets from jannu27 folder valioliiga', function(done){
    let results = bets.get_bets_from_folder(1, 'valioliiga');
    expect(results.length).to.equal(3);
    done();
  });

  it('returns 2 finished bets from jannu27 folder valioliiga', function(done){
    let results = bets.get_bets_from_folder(1, 'valioliiga', true);
    expect(results.length).to.equal(2);
    done();
  });

  it('returns 1 unfinished bet from jannu27 folder valioliiga', function(done){
    let results = bets.get_bets_from_folder(1, 'valioliiga', false);
    expect(results.length).to.equal(1);
    done();
  });
});

describe('get_bet', function(){
  it('returns null when bet_id does not exist', function(done){
    expect(bets.get_bet(12)).to.equal(null);
    done();
  });

  it('returns correct bet', function(done){
    let bet = bets.get_bet(7);
    expect(bet.owner).to.equal(5);
    expect(bet.bet_id).to.equal(7);
    done();
  });
});

describe('delete_bet', function(done){
  it('return false when trying to delete a bet from other user', function(done){
    var res = bets.delete_bet(7, 1);
    expect(res).to.equal(false);
    done();
  });

  it('returns false when trying to delete an unexisting bet', function(done){
    var res = bets.delete_bet(12, 1);
    expect(res).to.equal(false);
    done();
  });

  it('true on deleting users own bet successfully', function(done){
    var res = bets.delete_bet(1, 1);
    expect(res).to.equal(true);
    done();
  });
});

describe('create_bet', function(done){
  it('returns true on success', function(done){
    var res = bets.create_bet(1, new Date().toString(), 4.8, 4.7, "", false);
    expect(res).to.equal(true);
    done();
  });

  it('returns false on invalid date', function(done){
    var res = bets.create_bet(1, "2018-084-05 14:524:40" , 4.8, 4.7, "", false);
    expect(res).to.equal(false);
    done();
  });

  it('returns false when user does not exist', function(done){
    var res = bets.create_bet(-1, new Date().toString() , 4.8, 4.7, "", false);
    expect(res).to.equal(false);
    done();
  });

  it('returns null when database is empty', function(done){
    config.setConfig({db_path: 'notexisting'});
    var res = bets.create_bet(-1, new Date().toString() , 4.8, 4.7, "", false);
    expect(res).to.equal(null);
    config.setConfig({db_path: 'database/data/testi.sqlite3'});
    fo.delete_database("notexisting", function(){
      done();
    });
  });

  it('returns false on invalid decimal values', function(done){
    var res = bets.create_bet(1, new Date().toString() , 'not decimal', 4.7, "", false);
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
      fo.run_script(testDB, test_init, function(){
        done();
      });
    });
  });

  it('adds bet to folders when it does not exist in folder already', function(done){
    var bet = bets.get_bet(1);
    var res = bets.add_bet_to_folders(["liiga", "triplat"], bet.bet_id, 1);
    expect(res.length).to.equal(2);
    done();
  });

  it('does not add to folder where bet is already', function(done){
    var bet = bets.get_bet(2);
    var res = bets.add_bet_to_folders(["liiga", "valioliiga"], bet.bet_id, 1);
    expect(res.length).to.equal(1);
    done();
  });

  it('does not add bet to other users folders', function(done){
    var bet = bets.get_bet(2);
    var res = bets.add_bet_to_folders(["tuplat"], bet.bet_id, 1);
    expect(res.length).to.equal(0);
    done();
  });

  it('does not add other users bet', function(done){
    var res = bets.add_bet_to_folders(["triplat"], 4, 1);
    expect(res.length).to.equal(0);
    done();
  });
})

describe('delete_bet_from_folders', function(){

  it('returns 2 when deletes from two folders', function(done){
    bets.add_bet_to_folders(["triplat"], 1, 1);
    var res = bets.delete_bet_from_folders(["valioliiga", "triplat"], 1, 1);
    expect(res.length).to.equal(2);
    bets.add_bet_to_folders(["valioliiga"], 1, 1);
    done();
  });

  it('does not delete from folders where bet does not exist', function(done){
    var res = bets.delete_bet_from_folders(["valioliiga", "triplat"], 1, 1);
    expect(res.length).to.equal(1);
    done();
  });

  it('does not delete other users bet', function(done){
    var res = bets.delete_bet_from_folders(["tuplat"], 1, 4);
    expect(res.length).to.equal(0);
    done();
  });
});

describe('modify_bet', function(){
  it('returns true on success', function(done){
    let res = bets.modify_bet(5, 1, true, 100, 2, "test name");
    expect(res).to.equal(true);
    done();
  });

  it('does not modify null bet, odd & name', function(done){
    let res = bets.modify_bet(5, 1, false, 500, null, null);
    let bet = bets.get_bet(5);
    expect(bet.bet).to.equal(500);
    expect(bet.name).to.equal('test name');
    expect(bet.odd).to.equal(2);
    done();
  });

  it('returns false on modifying unknown bet', function(done){
    let res = bets.modify_bet(19, 1, true, 100, 2, "");
    expect(res).to.equal(false);
    done();
  });

  it('returns false on modifying another users bet', function(done){
    let res = bets.modify_bet(4, 1, true, 100, 2, "");
    expect(res).to.equal(false);
    done();
  });
});
