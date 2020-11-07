import HttpRequest from './httpRequest';

/*
POST-request to uri/users.

Excpected responses:
  201 Created,
  409 Conflict

  Resolves with data object with
  username and password.
  Rejects with status of the response.
*/
export async function postUser(username, password)
{
  await new HttpRequest(
    'users', 
    'POST',
    [
      { key: 'Authorization', value: password },
      { key: 'Content-Type', value: 'application/json' }
    ],
    JSON.stringify({username: username})).sendRequest();

    return { username: username, password: password };
}
