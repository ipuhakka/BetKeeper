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
