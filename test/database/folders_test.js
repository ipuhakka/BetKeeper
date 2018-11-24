var chai = require('chai');
var expect = chai.expect;
var fo = require('../../database/file_operations');
var folders = require('../../database/folders');
var config = require('../../config');

const test_init = require('../test_files').test_init;
const testDB = require('../test_files').testDB;

describe('user_has_folder', function(){
  it('returns false when folder exists but not for specified user', function(done){
    var res = folders.user_has_folder(1, 'tuplat');
    expect(res).to.equal(false);
    done();
  });

  it('returns false when nobody has specified folder', function(done){
    var res = folders.user_has_folder(1, 'unexisting folder name');
    expect(res).to.equal(false);
    done();
  });

  it('returns true when user owns specified folder', function(done){
    var res = folders.user_has_folder(1, 'triplat');
    expect(res).to.equal(true);
    done();
  });
});

describe('get_users_folders', function(){
  it('returns 1 folder for user_id 3', function(done){
    var res = folders.get_users_folders(3);
    expect(res.length).to.equal(1);
    done();
  });

  it('returns 3 folders for user_id 1', function(done){
    var res = folders.get_users_folders(1);
    expect(res.length).to.equal(3);
    done();
  });

  it('returns valioliiga folder with user_id 1 and bet_id 5', function(done){
    var res = folders.get_users_folders(1, 5);
    expect(res[0]).to.equal('valioliiga');
    done();
  });

  it('returns liiga folder with user_id 1 and bet_id 3', function(done){
    var res = folders.get_users_folders(1, 3);
    expect(res[0]).to.equal('liiga');
    done();
  });

  it('returns empty array when user does not exist', function(done){
    var res = folders.get_users_folders(-1);
    expect(res.length).to.equal(0);
    done();
  });
});

describe('delete_folder', function(){
  it('lets user delete their own folder', function(done){
    let res = folders.delete_folder(1, 'valioliiga');
    expect(res).to.equal(true);
    folders.add_folder(1, 'valioliiga');
    done();
  });

  it('returns null on error in statement', function(done){
    config.setConfig({db_path: 'notexisting'});
    let res = folders.delete_folder(1, 'valioliiga');
    expect(res).to.equal(null);
    config.setConfig({db_path: 'database/data/testi.sqlite3'});
    fo.delete_database('notexisting', function(){
      done();
    });
  });

  it('returns false on deleting folder which user does not have', function(done){
    let res = folders.delete_folder(testDB, 1, 'tuplat');
    expect(res).to.equal(false);
    done();
  });

});

describe('add_folder', function(done){
  it('returns true when folder name is not in use by user', function(done){
    let res = folders.add_folder(1, 'tuplat');
    expect(res).to.equal(true);
    folders.delete_folder( 1, 'tuplat');
    done();
  });

  it('returns false when user already has folder of that name', function(done){
    let res = folders.add_folder(1, 'valioliiga');
    expect(res).to.equal(false);
    done();
  });

  it('returns null on error in statement', function(done){
    config.setConfig({db_path: 'notexisting'});
    let res = folders.add_folder(1, 'valioliiga');
    expect(res).to.equal(null);
    config.setConfig({db_path: 'database/data/testi.sqlite3'});
    fo.delete_database('notexisting', function(){
      done();
    });
  })
});
