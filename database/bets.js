var moment = require('moment');
const isNumber = require('is-number');

module.exports = {
  /*
  Returns bets from selected user. Returns only finished betsi
  if {bets_finished} is true, unfinished bets if it is false,
  and all if {bets_finished} is not given at all.

  Returns null on error in request.

  Bet format:
  var bet = {
    bet_won: integer, //-1 = not finished, 0=lost, 1=won
    name: string, //optional name for the bet
    odd: double,
    bet: double,
    date_time: string, //
    owner: integer, //id of the user who owns the bet
    bet_id: integer
  }
  */
  get_bets: function(db_path, user_id, bets_finished){
    const db = require('better-sqlite3')(db_path);
    let result = [];
    try {
      let stmt;
      let res;
      if (bets_finished === undefined){
        stmt = db.prepare('SELECT * FROM bets WHERE owner=?');
        res = stmt.all(user_id);
        result = res.map((item) => { return create_bet_object(item); });
      }
      else {
        let query = bets_finished ? 'SELECT * FROM bets WHERE owner=? AND bet_won != -1' : 'SELECT * FROM bets WHERE owner=? AND bet_won = - 1';
        stmt = db.prepare(query);
        res = stmt.all(user_id);
        result = res.map((item) => { return create_bet_object(item); });
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

  get_bet: function(db_path, bet_id){
    const db = require('better-sqlite3')(db_path);
    let bet;
    try {
      let res = db.prepare('SELECT * FROM bets WHERE bet_id=?').get(bet_id);
      bet = create_bet_object(res);
    }
    catch (err){
      bet = null;
    }
    finally{
      db.close();
    }
    return bet;
  },

  /*
  Returns bets from specific folder by user. If finished argument
  is not given, all bets from folder are returned. If it is true,
  only finished bets from folder are returned: If false, only
  unfinished bets from folder are returned.

  Returns null on error in request.
  */
  get_bets_from_folder: function(db_path, user_id, folder, bets_finished){
    const db = require('better-sqlite3')(db_path);
    let result = [];
    try {
      let stmt;
      let res;
      if (bets_finished === undefined){
        stmt = db.prepare('SELECT * FROM bet_in_bet_folder bf INNER JOIN bets b ON b.bet_id = bf.bet_id WHERE bf.owner = ? and bf.folder = ?');
      }
      else if(bets_finished) {
        let query = 'SELECT * FROM bet_in_bet_folder bf INNER JOIN bets b ON b.bet_id = bf.bet_id WHERE bf.owner = ? and bf.folder = ? and bet_won != -1';
        stmt = db.prepare(query);
      }
      else if (!bets_finished){
        let query = 'SELECT * FROM bet_in_bet_folder bf INNER JOIN bets b ON b.bet_id = bf.bet_id WHERE bf.owner = ? and bf.folder = ? and bet_won = -1';
        stmt = db.prepare(query);
      }
      res = stmt.all(user_id, folder);
      result = res.map((item) => { return create_bet_object(item); });
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
  Delete's a bet from all folders of user.
  Returns true on success, false on failure & null
  on error in request.
  */
  delete_bet: function(db_path, bet_id, user_id){
    let bet_to_delete = this.get_bet(db_path, bet_id);
    if (bet_to_delete === null || bet_to_delete.owner !== user_id){
      return false;
    }

    const db = require('better-sqlite3')(db_path);
    let query = 'DELETE FROM bets WHERE bet_id = ?';
    let result = false;
    try {
      var res = db.prepare(query).run(bet_id);
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
  Deletes a bet from selected folders. Returns an array
  with names of folders from which bet was successfully deleted.

  Returns null when a connection related error happens.
  */
  delete_bet_from_folders: function(db_path, folders, bet_id, user_id){
    if (this.get_bet(db_path, bet_id).owner !== user_id){
      return [];
    }

    const db = require('better-sqlite3')(db_path);
    let query = 'DELETE FROM bet_in_bet_folder WHERE bet_id = ? AND folder = ? ';
    let deletedFrom = [];
    let stmt = db.prepare(query);
    for (var i = 0; i < folders.length; i++){
      try {
        let res = stmt.run(bet_id, folders[i]);
        if (res.changes === 1){
          deletedFrom.push(folders[i]);
        }
      }
      catch(err){
        if (err.message.indexOf('no such table') !== -1){
          deletedFrom = null;
          break;
        }
      }
    }
    db.close();

    return deletedFrom;
  },

  /*
  Creates a new bet.

  parameters:
    db_path: Path to database file_operations.
    user_id: Id for the user who played the bet.
    datetime: Datetime when the bet was played.
    odd: Odd for the bet.
    bet: Stake for the bet.
    name: Name for the bet.
    bet_won: true if bet was correct, false if not,
      null if bet has not resolved yet.

  Returns true on success,
  false if user does not exist, or if datetime is in
  invalid format, and null if request fails because
  of database and connection issues.
  */
  create_bet: function(db_path, user_id, datetime, odd, bet, name, bet_won){
    datetime = moment(datetime, 'YYYY-MM-DD HH:MM:SS').format('YYYY-MM-DD HH:MM:SS');
    if(!moment(datetime, 'YYYY-MM-DD HH:MM:SS', true).isValid()){
      return false;
    }

    if(!isNumber(odd) || !isNumber(bet)){
      return false;
    }

    let bet_result = -1;
    if (bet_won !== null){
      bet_result = bet_won ? 1 : 0;
    }

    const db = require('better-sqlite3')(db_path);
    let query = 'INSERT INTO bets (bet_won, name, odd, bet, date_time, owner) values (?, ?, ?, ?, ?, ?)';
    let result = false;
    try {
      var res = db.prepare(query).run(bet_result, name, odd, bet, datetime, user_id);

      if (res.changes === 1){
        result = true;
      }
    }
    catch (err){
      if (err.message.indexOf('no such table') === -1){
        result = false;
      }
      else {
        result = null;
      }
    }
    finally{
      db.close();
    }
    return result;
  },

  /*
  Add's an already existing bet into folders.
  Returns an array of folder names, to which bet
  was successfully added.

  Returns null when an error regarding connection is found.
  */
  add_bet_to_folders: function(db_path, folders, bet_id, user_id){
    if (this.get_bet(db_path, bet_id).owner !== user_id){
      return [];
    }

    const db = require('better-sqlite3')(db_path);
    let query = 'INSERT INTO bet_in_bet_folder VALUES (?, ?, ?)';
    let addedTo = [];
    let stmt = db.prepare(query);
    for (var i = 0; i < folders.length; i++){
      try {
        let res = stmt.run(folders[i], user_id, bet_id);
        if (res.changes === 1){
          addedTo.push(folders[i]);
        }
      }
      catch(err){
        if (err.message.indexOf('no such table') !== -1){
          addedTo = null;
          break;
        }
      }
    }
    db.close();

    return addedTo;
  }
}

function create_bet_object(item){
  var bet = {
    bet_won: item.bet_won,
    name: item.name,
    odd: item.odd,
    bet: item.bet,
    date_time: item.date_time,
    owner: item.owner,
    bet_id: item.bet_id
  }
  return bet;
}
