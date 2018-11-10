const sqlite3 = require('sqlite3').verbose();
const fs = require('fs');

module.exports = {
  /*
  Function performs all statements in file {path_to_script}.
  If {database_path} does not exist, it is created.
  */
  run_script: function(database_path, script_path, callback){
    let db = new sqlite3.Database(database_path, (err) => {
      if (err) {
        console.error(err.message);
        throw err;
      }
    });

    fs.readFile(script_path, 'utf-8', function read(err, data) {
      if (err) {
        throw err;
      }
      db.exec(data);

      db.close((err) => {
        if (err) {
          console.error(err.message);
          throw err;
        }
        callback();
      });
    });
  },


  delete_database: function(path){

    fs.unlink(path, (err) => {
      if (err) throw err;
    });
  }
}
