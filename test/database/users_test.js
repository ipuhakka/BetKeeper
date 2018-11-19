var chai = require('chai');
var expect = chai.expect;
var fo = require('../../database/file_operations');
var users = require('../../database/users');

const test_init = require('../test_files').test_init;
const testDB = require('../test_files').testDB;

describe('check_password', function() {
  it('true when username and password match', function(done){
    var res = users.check_password(testDB, 1, 'salasana');
    expect(res).to.equal(true);
    done();
  });

  it('false when username and password do not match', function(done){
    var res = users.check_password(testDB, 1, 'salasana2');
    expect(res).to.equal(false);
    done();
  });

  it('false when username does not exist', function(done){
    var res = users.check_password(testDB, -1, 'salasana');
    expect(res).to.equal(false);
    done();
  });

  it('handles basic SQL-injection', function(done){
    var res = users.check_password(testDB, 1, '\' or \'1\'=\'1');
    expect(res).to.equal(false);
    done();
  });
});

describe('username_exists', function() {
  it('true when username exists', function(done){
    var res = users.username_exists(testDB, 'jannu27');
    expect(res).to.equal(true);
    done();
  });

  it('false when username does not exist', function(done){
    var res = users.username_exists(testDB, 'jannu28');
    expect(res).to.equal(false);
    done();
  });

  it('returns null when database is deleted before querying', function(done){
    fo.delete_database(testDB, function(){
      var res = users.username_exists(testDB, 'jannu27');
      expect(res).to.equal(null);
      fo.run_script(testDB, test_init, function(){
        done();
      });
    });
  });
});

describe('get_user_id', function(){
  it('returns -1 when user does not exist', function(done){
    var res = users.get_user_id(testDB, 'unexisting user');
    expect(res).to.equal(-1);
    done();
  });

  it('returns 3 when username = käyttäjä2', function(done){
    var res = users.get_user_id(testDB, 'käyttäjä2');
    expect(res).to.equal(3);
    done();
  });
});

describe('add_user', function(){
  it('returns -1 when database does not exist', function(done){
    fo.delete_database(testDB, function(){
      var res = users.add_user(testDB, 'jannu27', 'salasana');
      expect(res).to.equal(-1);
      fo.run_script(testDB, test_init, function(){
        done();
      });
    });
  });

  it('returns 0 when username already exists', function(done){
    var res = users.add_user(testDB, 'jannu27', 'password');
    expect(res).to.equal(0);
    done();
  });

  it('returns 1 when username does not exist', function(done){
    var res = users.add_user(testDB, 'new_user', 'password');
    expect(res).to.equal(1);
    done();
  });
});
