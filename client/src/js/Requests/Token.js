import HttpRequest from './httpRequest';

/*
POST-request to get a token to use in the app.

data:{
  username: 'username',
  password: 'password'
}

On success, resolved with parsed token, user-id and username as parameters.
Rejects with status of received http-response.
*/
export async function postToken(username, password)
{
  const response = await new HttpRequest(
    'token', 
    'POST',
    [
      { key: 'Authorization', value: password },
      { key: 'Content-Type', value: 'application/json'}
    ],
    JSON.stringify({username: username})).sendRequest();

    const responseBody = JSON.parse(response.responseText);
    return {
      token: responseBody.tokenString, 
      owner: responseBody.owner, 
      username: username
    };
}

/*
Logout event. Delete's a token from api. Resolves on 204 No content,
rejects on other responses.
*/
export function deleteToken()
{
  return new HttpRequest(
    'token/' + localStorage.getItem('loggedUserId'), 
    'DELETE',
    [
      { key: 'Authorization', value: localStorage.getItem('token') }
    ]).sendRequest();
}

/*GET-request to check if token is already in use. If it is, it means user
is logged in, and is redirected to home page. Returns 404 if user hasn't got a token,
and a new one must be requested.

Resolves on 200 OK, rejects on any other response status*/
export function getToken(token, userId)
{
  return new HttpRequest(
    'token/' + userId, 
    'GET',
    [
      { key: 'Authorization', value: token }
    ]).sendRequest();
}
