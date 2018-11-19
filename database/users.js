module.exports = {
  /* Returns true if username and password match a user, false if not.
    Returns null if query fails.
  */
  check_password: function(db_path, user_id, password){
    const db = require('better-sqlite3')(db_path);
    let query = 'SELECT(EXISTS(SELECT 1 FROM users WHERE user_id = ? AND password = ?))';
    let result = false;
    try {
      var res = db.prepare(query).get(user_id, password);
      if (JSON.stringify(res).split(':')[1].includes('1')){
        result = true;
      }
    }
    catch (err){
      console.log(err);
      result = null;
    }
    finally{
      db.close();
    }
    return result;
  },

  /*
  Checks whether a username exists. If an error occurs while making the query,
  returns null.
  */
  username_exists: function(db_path, username){
    const db = require('better-sqlite3')(db_path);
    let query = 'SELECT(EXISTS(SELECT 1 FROM users WHERE username = ?))';
    let result = false;
    try {
      var res = db.prepare(query).get(username);
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
  Returns id for user with username {username}.
  Returns -1 if user does not exist,
  and null if query fails.
  */
  get_user_id: function(db_path, username){
    const db = require('better-sqlite3')(db_path);
    let query = 'SELECT user_id FROM users WHERE username = ?';
    let result = -1;
    try {
      var res = db.prepare(query).get(username);
      if (res != null){
        result = res.user_id;
      }
    }
    catch (err){
      console.log(err);
      result = null;
    }
    finally{
      db.close();
    }
    return result;
  },

  /*
  adds a user to table users.
  Returns 1 if insertion was successfull,
  0 if user already exists,
  and -1 if something went wrong with the request.
  */
  add_user: function(db_path, username, password){
    const db = require('better-sqlite3')(db_path);
    let result = -1;
    try {
      var stmt = db.prepare('INSERT INTO users (username, password) VALUES (?, ?)');
      var res = stmt.run(username, password);
      if (res != null){
        result = res.changes;
      }
    }
    catch(err){
      if (err.message.indexOf('UNIQUE') === -1){
      result = -1;
      }
      else {
        result = 0;
      }
    }
    finally{
      db.close();
    }
    return result;
  }
}
