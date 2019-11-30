export const LOGIN = 'LOGIN';
export const LOGIN_SUCCESS = 'LOGIN_SUCCESS';
export const LOGOUT = 'LOGOUT';
export const SIGNUP = 'SIGNUP';

export const login = (username, password, history, redirectTo) => ({
  type: 'LOGIN',
  username,
  password,
  history,
  redirectTo
});

export const loginSuccess = () => ({
  type: 'LOGIN_SUCCESS'
});

export const logOut = () => ({
  type: 'LOGOUT'
});

export const signUp = (username, password, callback) => ({
  type: 'SIGNUP',
  payload: {
    username,
    password,
    callback
  }
});

/**
 * Checks if current login information is still valid.
 * 
 * @param {*} userId 
 * @param {*} tokenString 
 */
export const checkCurrentLoginCredentials = (userId, tokenString, redirectToOnSuccess, history) => ({
  type: 'CHECK_LOGIN',
  payload: {
    userId: userId,
    tokenString: tokenString,
    redirectTo: redirectToOnSuccess,
    history: history
  }
});
