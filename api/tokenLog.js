var tokenLog = [];
const TOKEN_LENGTH = 12;

/*
Returns a token object = {
  token: '12-char random string',
  owner: user_id
}
*/
function create_token(user_id){
  return new Promise(function(resolve){
    let token;
    do {
      token = "";
      var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
      for (var i = 0; i < TOKEN_LENGTH; i++){
        token += possible.charAt(Math.floor(Math.random() * possible.length));
      }
    } while(contains_token(token));
    resolve({token: token, owner: user_id});
  });
}

function add_token(token){
  tokenLog.push(token);
}

function clear(){
  tokenLog = [];
}

function contains_token(tokenString){
  return tokenLog.some(function (t) {
    return t.token === tokenString;
  });
}

/*returns owner of the token, if token is present in tokenLog.*/
function get_token_owner(tokenString){
  for (var i = 0; i < tokenLog.length; i++){
    if (tokenLog[i].token === tokenString){
      return tokenLog[i].owner;
    }
  }
  return -1;
}

module.exports = {
  create_token: create_token,
  clear: clear,
  contains_token: contains_token,
  add_token: add_token,
  get_token_owner: get_token_owner
};
