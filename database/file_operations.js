const fs = require('fs');

module.exports = {
  /*
  Function performs all statements in file {path_to_script}.
  If {database_path} does not exist, it is created.
  */
  run_script: function(database_path, script_path, callback){
    const db = require('better-sqlite3')(database_path);

    fs.readFile(script_path, 'utf-8', function read(err, data) {
      if (err) {
        throw err;
      }
      db.exec(data);

      db.close();
      callback();
    });
  },

  delete_database: function(path, callback){
    fs.unlink(path, (err) => {
      if (err) throw err;
      callback();
    });
  }
}
