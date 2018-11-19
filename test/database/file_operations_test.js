var chai = require('chai');
var expect = chai.expect;
var fo = require('../../database/file_operations');
const test_init = require('../test_files').test_init;
const testDB = require('../test_files').testDB;

describe('database connection', function() {
    it('Does not throw errors on creating and deleting a database', function(done){
      expect(() => {
        fo.run_script(testDB, test_init, function(){
          fo.delete_database(testDB, function(){
            fo.run_script(testDB, test_init, function(){
              done();
            })
          });
        })
      }).to.not.throw();
    });

});
