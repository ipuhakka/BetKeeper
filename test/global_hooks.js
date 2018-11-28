var chai = require('chai');
var fo = require('../database/file_operations');
const test_init = require('./test_files').test_init;
const testDB = require('./test_files').testDB;
var config = require('../config');

before(function(done){
  this.timeout(10000);
  config.setConfig({db_path: 'database/data/testi.sqlite3'});
  fo.run_script(testDB, test_init, function(){
    done();
  });
});

after(function(done){
  fo.delete_database(testDB, function(){
    done();
  });
});
