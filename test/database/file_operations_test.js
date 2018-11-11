var chai = require('chai');
var expect = chai.expect;
var fo = require('../../database/file_operations');
const script_path = 'database/scripts/database_schema_dump.sql';
const testDB = 'database/data/testi.sqlite3';

describe('database connection', function() {
    it('Does not throw errors on creating and deleting a database', function(done){
      expect(() => {
        fo.run_script(testDB, script_path, function(){
          fo.delete_database(testDB, function(){
            done();
          });
        })
      }).to.not.throw();
    });

});
