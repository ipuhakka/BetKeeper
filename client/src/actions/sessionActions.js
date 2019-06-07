export const LOGIN = 'LOGIN';
export const LOGIN_SUCCESS = 'LOGIN_SUCCESS';
export const LOGOUT = 'LOGOUT';

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
