module.exports = {

  /*
  Checks whether user with {user_id} owns a folder called
  {folder_name}.

  Returns null if an error is raised during the query.
  */
  user_has_folder: function(db_path, user_id, folder_name){
    const db = require('better-sqlite3')(db_path);
    let query = 'SELECT(EXISTS(SELECT 1 FROM bet_folders WHERE owner = ? AND folder_name = ?))';
    let result = false;
    try {
      var res = db.prepare(query).get(user_id, folder_name);
      if (JSON.stringify(res).split(':')[1].includes('1')){
        result = true;
      }
    }
    catch (err){
      result = null;
    }
    finally{
      db.close();
    }
    return result;
  },

  /*
  Gets folders for user. If bet_id is given a value,
  returns folders which contain selected bet.
  */
  get_users_folders: function(db_path, user_id, bet_id){
    const db = require('better-sqlite3')(db_path);
    let result = [];
    try {
      let stmt;
      let res;
      if (bet_id === undefined){
        stmt = db.prepare('SELECT DISTINCT folder_name FROM bet_folders WHERE owner = ?');
        res = stmt.all(user_id);
        result = res.map((item) => { return item.folder_name; });
      }
      else {
        stmt = db.prepare('SELECT DISTINCT folder from bet_in_bet_folder bf WHERE bf.owner = ? AND bet_id = ?');
        res = stmt.all(user_id, bet_id);
        result = res.map((item) => { return item.folder; });
      }
    }
    catch (err){
      result = null;
    }
    finally{
      db.close();
    }
    return result;
  },

  /*
  Deletes a folder {folder_name} from user, if user has that
  folder.

  Returns:
    true if successfully deleted,
    false if no deletion was done,
    null on error.
  */
  delete_folder: function(db_path, user_id, folder_name){
    const db = require('better-sqlite3')(db_path);
    let query = 'DELETE FROM bet_folders WHERE owner = ? AND folder_name = ?';
    let result = false;
    try {
      var res = db.prepare(query).run(user_id, folder_name);
      if (res.changes === 1){
        result = true;
      }
    }
    catch (err){
      result = null;
    }
    finally{
      db.close();
    }
    return result;
  },

  /*
  Adds a folder if user does not have a folder of that name.
  Returns true on success, false if already exists, null on
  error in statement.
  */
  add_folder: function(db_path, user_id, folder_name){
    if (this.user_has_folder(db_path, user_id, folder_name)){
      return false;
    }

    const db = require('better-sqlite3')(db_path);
    let query = 'INSERT INTO bet_folders VALUES (?, ?)';
    let result = false;
    try {
      var res = db.prepare(query).run(folder_name, user_id);
      if (res.changes === 1){
        result = true;
      }
    }
    catch (err){
      result = null;
    }
    finally{
      db.close();
    }
    return result;
  }

}
